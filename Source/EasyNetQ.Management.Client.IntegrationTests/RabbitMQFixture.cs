using System.Runtime.InteropServices;
using EasyNetQ.Management.Client.Model;
using Xunit;

namespace EasyNetQ.Management.Client.IntegrationTests;

public sealed class RabbitMqFixture : IAsyncLifetime, IDisposable
{
    private static readonly Vhost VirtualHost = new() { Name = "/", Tracing = false };
    private static readonly TimeSpan InitializationTimeout = TimeSpan.FromMinutes(2);

    private const string ContainerName = "easynetq.management.tests";
    private const string Image = "rabbitmq";

    private readonly string tag;
    private readonly DockerProxy dockerProxy;
    private OSPlatform dockerEngineOsPlatform;
    private string dockerNetworkName;

    public RabbitMqFixture()
    {
        dockerProxy = new DockerProxy();
        tag = $"{Environment.GetEnvironmentVariable("RABBITMQ_VERSION") ?? "3.11"}-management";
    }

    public string Host { get; private set; } = "localhost";
    public string User { get; private set; } = "guest";
    public string Password { get; private set; } = "guest";

    public IManagementClient ManagementClient { get; private set; }

    public async Task InitializeAsync()
    {
        using var cts = new CancellationTokenSource(InitializationTimeout);
        dockerEngineOsPlatform = await dockerProxy.GetDockerEngineOsAsync(cts.Token);
        dockerNetworkName = dockerEngineOsPlatform == OSPlatform.Windows ? null : "bridgeWhaleNet";
        await DisposeAsync(cts.Token);
        await CreateNetworkAsync(cts.Token);
        await PullImageAsync(cts.Token);
        var containerId = await RunNewContainerAsync(cts.Token);
        if (dockerEngineOsPlatform == OSPlatform.Windows)
            Host = await dockerProxy.GetContainerIpAsync(containerId, cts.Token);
        ManagementClient = new ManagementClient(new Uri($"http://{Host}:15672"), User, Password);
        await WaitForRabbitMqReadyAsync(cts.Token);
    }

    public async Task DisposeAsync()
    {
        await DisposeAsync(default);
    }

    public void Dispose()
    {
        dockerProxy.Dispose();
    }

    private async Task DisposeAsync(CancellationToken cancellationToken)
    {
        await dockerProxy.StopContainerAsync(ContainerName, cancellationToken);
        await dockerProxy.RemoveContainerAsync(ContainerName, cancellationToken);
        if (dockerEngineOsPlatform == OSPlatform.Linux || dockerEngineOsPlatform == OSPlatform.OSX)
            await dockerProxy.DeleteNetworkAsync(dockerNetworkName, cancellationToken);
    }

    private async Task CreateNetworkAsync(CancellationToken cancellationToken)
    {
        if (dockerEngineOsPlatform == OSPlatform.Linux || dockerEngineOsPlatform == OSPlatform.OSX)
            await dockerProxy.CreateNetworkAsync(dockerNetworkName, cancellationToken);
    }

    private async Task PullImageAsync(CancellationToken cancellationToken)
    {
        await dockerProxy.PullImageAsync(Image, tag, cancellationToken);
    }

    private async Task<string> RunNewContainerAsync(CancellationToken cancellationToken)
    {
        var portMappings = new Dictionary<string, ISet<string>>
        {
            {"15671", new HashSet<string> {"15671"}},
            {"15672", new HashSet<string> {"15672"}},
        };
        var envVars = new List<string> { "RABBITMQ_DEFAULT_VHOST=/" };
        var containerId = await dockerProxy.CreateContainerAsync(
            $"{Image}:{tag}",
            ContainerName,
            portMappings,
            dockerNetworkName,
            envVars,
            cancellationToken
        );
        await dockerProxy.StartContainerAsync(containerId, cancellationToken);
        return containerId;
    }

    private async Task WaitForRabbitMqReadyAsync(CancellationToken cancellationToken)
    {
        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (await IsRabbitMqReadyAsync(cancellationToken))
                return;
            await Task.Delay(500, cancellationToken);
        }
    }

    private async Task<bool> IsRabbitMqReadyAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await ManagementClient.IsAliveAsync(VirtualHost, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
