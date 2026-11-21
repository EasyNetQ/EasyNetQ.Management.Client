using System.Runtime.InteropServices;
using EasyNetQ.Management.Client.Model;

namespace EasyNetQ.Management.Client.IntegrationTests;

public sealed class RabbitMqFixture : IAsyncLifetime, IDisposable
{
    private static readonly Vhost VirtualHost = new(Name: "/");
    private static readonly TimeSpan InitializationTimeout = TimeSpan.FromMinutes(2);

    private const string ContainerName = "easynetq.management.tests";
    private const string Image = "rabbitmq";

    private readonly string tag;
    private readonly DockerProxy dockerProxy;
    private OSPlatform dockerEngineOsPlatform;
    private string? dockerNetworkName;

    public RabbitMqFixture()
    {
        dockerProxy = new DockerProxy();
        tag = $"{Environment.GetEnvironmentVariable("RABBITMQ_VERSION") ?? "4.0"}-management";
    }

    private const string AmqpPort = "5672";
    private const string ManagementPort = "15672";

    public Uri Endpoint { get; private set; } = new("http://localhost:15672");
    public string AmqpConnectionString { get; private set; } = "host=localhost";
    public string User { get; private set; } = "guest";
    public string Password { get; private set; } = "guest";

    private IManagementClient? managementClient;
    public IManagementClient ManagementClient => managementClient!;

    private Version? rabbitmqVersion;
    public Version RabbitmqVersion => rabbitmqVersion!;

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
        {
            var containerIp = await dockerProxy.GetContainerIpAsync(containerId, cts.Token);
            Endpoint = new Uri($"http://{containerIp}:{ManagementPort}");
            AmqpConnectionString = $"host={containerIp}";
        }
        else
        {
            var hostPorts = await dockerProxy.GetHostPortsAsync(containerId, cts.Token);
            Endpoint = new Uri($"http://localhost:{hostPorts[ManagementPort]}");
            AmqpConnectionString = $"host=localhost:{hostPorts[AmqpPort]}";
        }

        managementClient = new ManagementClient(Endpoint, User, Password);
        await WaitForRabbitMqReadyAsync(cts.Token);

        var overview = await ManagementClient.GetOverviewAsync();
        rabbitmqVersion = new Version(overview.RabbitmqVersion);
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
        if (dockerNetworkName != null)
            await dockerProxy.DeleteNetworkAsync(dockerNetworkName, cancellationToken);
    }

    private async Task CreateNetworkAsync(CancellationToken cancellationToken)
    {
        if (dockerNetworkName != null)
            await dockerProxy.CreateNetworkAsync(dockerNetworkName, cancellationToken);
    }

    private async Task PullImageAsync(CancellationToken cancellationToken)
    {
        await dockerProxy.PullImageAsync(Image, tag, cancellationToken);
    }

    private async Task<string> RunNewContainerAsync(CancellationToken cancellationToken)
    {
        // An empty host port lets Docker pick a free one, so a broker already on 5672/15672 doesn't clash.
        var portMappings = new Dictionary<string, ISet<string>>
        {
            { AmqpPort, new HashSet<string> { "" } },
            { ManagementPort, new HashSet<string> { "" } },
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
