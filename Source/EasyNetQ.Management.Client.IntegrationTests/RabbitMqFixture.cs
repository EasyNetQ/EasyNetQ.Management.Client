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
        private const string RabbitImageTag = "latest";
        private const int DefaultTimeoutSeconds = 600;

        private readonly DockerProxy dockerProxy;
        private OSPlatform dockerEngineOsPlatform;
        private string dockerNetworkName;

        public string RabbitHostForManagement { get; private set; }

        public RabbitMqFixture()
        {
            dockerProxy = new DockerProxy(new Uri(Configuration.DockerHttpApiUri));
            RabbitHostForManagement = "localhost";
        }

        public async Task InitializeAsync()
        {
            if (Configuration.TestAgainstContainers)
            {
                dockerEngineOsPlatform = await dockerProxy.GetDockerEngineOsAsync();
                dockerNetworkName = dockerEngineOsPlatform == OSPlatform.Windows ? null : "bridgeWhaleNet";
                await DisposeAsync().ConfigureAwait(false);
                await CreateNetworkAsync().ConfigureAwait(false);
                var rabbitMQDockerImage = await PullImageAsync().ConfigureAwait(false);
                var containerId = await RunContainerAsync(rabbitMQDockerImage).ConfigureAwait(false);
                if (dockerEngineOsPlatform == OSPlatform.Windows)
                    RabbitHostForManagement = await dockerProxy.GetContainerIpAsync(containerId).ConfigureAwait(false);
            }
            await WaitForRabbitMqReadyAsync();
        }

        public async Task DisposeAsync()
        {
            if (!Configuration.TestAgainstContainers)
                return;

            await dockerProxy.StopContainerAsync(Configuration.RabbitMqHostName).ConfigureAwait(false);
            await dockerProxy.RemoveContainerAsync(Configuration.RabbitMqHostName).ConfigureAwait(false);
            if (dockerEngineOsPlatform == OSPlatform.Linux || dockerEngineOsPlatform == OSPlatform.OSX)
                await dockerProxy.DeleteNetworkAsync(dockerNetworkName).ConfigureAwait(false);
        }

        public void Dispose()
        {
            dockerProxy.Dispose();
        }

        private async Task CreateNetworkAsync()
        {
            if (dockerEngineOsPlatform == OSPlatform.Linux || dockerEngineOsPlatform == OSPlatform.OSX)
                await dockerProxy.CreateNetworkAsync(dockerNetworkName).ConfigureAwait(false);
        }

        private async Task<string> PullImageAsync()
        {
            var rabbitMQDockerImageName = Configuration.RabbitMQDockerImageName(dockerEngineOsPlatform);
            var rabbitMQDockerImageTag = Configuration.RabbitMQDockerImageTag(dockerEngineOsPlatform);
            await dockerProxy.PullImageAsync(rabbitMQDockerImageName, rabbitMQDockerImageTag).ConfigureAwait(false);
            return string.Format("{0}:{1}", rabbitMQDockerImageName, rabbitMQDockerImageTag);
        }

        private async Task<string> RunContainerAsync(string rabbitMQDockerImage)
        {
            var portMappings = new Dictionary<string, ISet<string>>
            {
                { "4369", new HashSet<string>(){ "4369" } },
                { "5671", new HashSet<string>(){ "5671" } },
                { "5672", new HashSet<string>(){ "5672" } } ,
                { "15671",new HashSet<string>(){ "15671" } },
                { "15672",new HashSet<string>(){ "15672" } },
                { "25672",new HashSet<string>(){ "25672" } }
            };
            var envVars = new List<string> { $"RABBITMQ_DEFAULT_VHOST={Configuration.RabbitMqVirtualHostName}" };
            var containerId = await dockerProxy
                .CreateContainerAsync(rabbitMQDockerImage, Configuration.RabbitMqHostName, portMappings, dockerNetworkName, envVars)
                .ConfigureAwait(false);
            await dockerProxy.StartContainerAsync(containerId).ConfigureAwait(false);
            return containerId;
        }

        private async Task WaitForRabbitMqReadyAsync()
        {
            var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(DefaultTimeoutSeconds));
            await WaitForRabbitMqReadyAsync(timeoutCts.Token).ConfigureAwait(false);
        }

        private async Task WaitForRabbitMqReadyAsync(CancellationToken token)
        {
            while (true)
            {
                token.ThrowIfCancellationRequested();
                if (await IsRabbitMqReadyAsync().ConfigureAwait(false))
                    return;
                await Task.Delay(500, token).ConfigureAwait(false);
            }
        }

        private async Task<bool> IsRabbitMqReadyAsync()
        {
            var rabbitMqManagementApi = new ManagementClient(RabbitHostForManagement, Configuration.RabbitMqUser, Configuration.RabbitMqPassword, Configuration.RabbitMqManagementPort);

            try
            {
                return await rabbitMqManagementApi.IsAliveAsync(Configuration.RabbitMqVirtualHost).ConfigureAwait(false);
            }
            catch
            {
                return false;
            }
        }
    }
}