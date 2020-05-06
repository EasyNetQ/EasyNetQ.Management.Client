using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace EasyNetQ.Management.Client.IntegrationTests
{
    [CollectionDefinition("Rabbitmq collection")]
    public class RabbitMqFixtureCollection : ICollectionFixture<RabbitMqFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }

    public class RabbitMqFixture : IAsyncLifetime, IDisposable
    {
        private static readonly TimeSpan InitializationTimeout = TimeSpan.FromMinutes(2);

        private readonly DockerProxy dockerProxy;
        private OSPlatform dockerEngineOsPlatform;
        private string dockerNetworkName;

        public RabbitMqFixture()
        {
            dockerProxy = new DockerProxy(new Uri(Configuration.DockerHttpApiUri));
            RabbitHostForManagement = "localhost";
        }

        public string RabbitHostForManagement { get; private set; }

        public async Task InitializeAsync()
        {
            using var timeoutCts = new CancellationTokenSource(InitializationTimeout);
            dockerEngineOsPlatform = await dockerProxy.GetDockerEngineOsAsync(timeoutCts.Token).ConfigureAwait(false);
            dockerNetworkName = dockerEngineOsPlatform == OSPlatform.Windows ? null : "bridgeWhaleNet";
            await DisposeAsync(timeoutCts.Token).ConfigureAwait(false);
            await CreateNetworkAsync(timeoutCts.Token).ConfigureAwait(false);
            var rabbitMQDockerImage = await PullImageAsync(timeoutCts.Token).ConfigureAwait(false);
            var containerId = await RunContainerAsync(rabbitMQDockerImage, timeoutCts.Token).ConfigureAwait(false);
            if (dockerEngineOsPlatform == OSPlatform.Windows)
                RabbitHostForManagement = await dockerProxy.GetContainerIpAsync(containerId, timeoutCts.Token)
                    .ConfigureAwait(false);

            await WaitForRabbitMqReadyAsync(timeoutCts.Token);
        }

        public async Task DisposeAsync()
        {
            await DisposeAsync(default).ConfigureAwait(false);
        }

        public void Dispose()
        {
            dockerProxy.Dispose();
        }

        private async Task DisposeAsync(CancellationToken cancellationToken)
        {
            await dockerProxy.StopContainerAsync(Configuration.RabbitMqHostName, cancellationToken)
                .ConfigureAwait(false);
            await dockerProxy.RemoveContainerAsync(Configuration.RabbitMqHostName, cancellationToken)
                .ConfigureAwait(false);
            if (dockerEngineOsPlatform == OSPlatform.Linux || dockerEngineOsPlatform == OSPlatform.OSX)
                await dockerProxy.DeleteNetworkAsync(dockerNetworkName, cancellationToken).ConfigureAwait(false);
        }

        private async Task CreateNetworkAsync(CancellationToken cancellationToken)
        {
            if (dockerEngineOsPlatform == OSPlatform.Linux || dockerEngineOsPlatform == OSPlatform.OSX)
                await dockerProxy.CreateNetworkAsync(dockerNetworkName, cancellationToken).ConfigureAwait(false);
        }

        private async Task<string> PullImageAsync(CancellationToken cancellationToken)
        {
            var rabbitMQDockerImageName = Configuration.RabbitMQDockerImageName(dockerEngineOsPlatform);
            var rabbitMQDockerImageTag = Configuration.RabbitMQDockerImageTag(dockerEngineOsPlatform);
            await dockerProxy.PullImageAsync(rabbitMQDockerImageName, rabbitMQDockerImageTag, cancellationToken)
                .ConfigureAwait(false);
            return $"{rabbitMQDockerImageName}:{rabbitMQDockerImageTag}";
        }

        private async Task<string> RunContainerAsync(string rabbitMQDockerImage, CancellationToken cancellationToken)
        {
            var portMappings = new Dictionary<string, ISet<string>>
            {
                {"4369", new HashSet<string> {"4369"}},
                {"5671", new HashSet<string> {"5671"}},
                {"5672", new HashSet<string> {"5672"}},
                {"15671", new HashSet<string> {"15671"}},
                {"15672", new HashSet<string> {"15672"}},
                {"25672", new HashSet<string> {"25672"}}
            };
            var envVars = new List<string> {$"RABBITMQ_DEFAULT_VHOST={Configuration.RabbitMqVirtualHostName}"};
            var containerId = await dockerProxy
                .CreateContainerAsync(rabbitMQDockerImage, Configuration.RabbitMqHostName, portMappings,
                    dockerNetworkName, envVars, cancellationToken)
                .ConfigureAwait(false);
            await dockerProxy.StartContainerAsync(containerId, cancellationToken).ConfigureAwait(false);
            return containerId;
        }

        private async Task WaitForRabbitMqReadyAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (await IsRabbitMqReadyAsync(cancellationToken).ConfigureAwait(false))
                    return;
                await Task.Delay(500, cancellationToken).ConfigureAwait(false);
            }
        }

        private async Task<bool> IsRabbitMqReadyAsync(CancellationToken cancellationToken)
        {
            var rabbitMqManagementApi = new ManagementClient(RabbitHostForManagement, Configuration.RabbitMqUser,
                Configuration.RabbitMqPassword, Configuration.RabbitMqManagementPort);

            try
            {
                return await rabbitMqManagementApi.IsAliveAsync(Configuration.RabbitMqVirtualHost, cancellationToken)
                    .ConfigureAwait(false);
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
}
