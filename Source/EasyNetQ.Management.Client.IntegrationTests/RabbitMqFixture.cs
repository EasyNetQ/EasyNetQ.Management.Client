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

        public string RabbitContainerHostForManagement { get; private set; }

        public string RabbitContainerAndHostName => "rmq";

        public RabbitMqFixture()
        {
            dockerProxy = new DockerProxy(new Uri(Configuration.DockerHttpApiUri));
        }

        public async Task InitializeAsync()
        {
            dockerEngineOsPlatform = await dockerProxy.GetDockerEngineOsAsync();
            dockerNetworkName = dockerEngineOsPlatform == OSPlatform.Windows ? null : "bridgeWhaleNet";
            var rabbitMQDockerImage = Configuration.RabbitMQDockerImage(dockerEngineOsPlatform);
            await DisposeAsync().ConfigureAwait(false);
            if (dockerEngineOsPlatform == OSPlatform.Linux || dockerEngineOsPlatform == OSPlatform.OSX)
                await dockerProxy.CreateNetworkAsync(dockerNetworkName).ConfigureAwait(false);
            
            await dockerProxy.PullImageAsync(rabbitMQDockerImage, RabbitImageTag).ConfigureAwait(false);
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
                .CreateContainerAsync(rabbitMQDockerImage, RabbitContainerAndHostName, portMappings, dockerNetworkName, envVars)
                .ConfigureAwait(false);
            await dockerProxy.StartContainerAsync(containerId).ConfigureAwait(false);
            RabbitContainerHostForManagement = "localhost";
            if (dockerEngineOsPlatform == OSPlatform.Windows)
                RabbitContainerHostForManagement = await dockerProxy.GetContainerIpAsync(containerId).ConfigureAwait(false);
            var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(DefaultTimeoutSeconds));
            await WaitForRabbitMqReady(timeoutCts.Token).ConfigureAwait(false);
        }

        public async Task DisposeAsync()
        {
            await dockerProxy.StopContainerAsync(RabbitContainerAndHostName).ConfigureAwait(false);
            await dockerProxy.RemoveContainerAsync(RabbitContainerAndHostName).ConfigureAwait(false);
            if (dockerEngineOsPlatform == OSPlatform.Linux || dockerEngineOsPlatform == OSPlatform.OSX)
                await dockerProxy.DeleteNetworkAsync(dockerNetworkName).ConfigureAwait(false);
        }

        public void Dispose()
        {
            dockerProxy.Dispose();
        }

        private async Task WaitForRabbitMqReady(CancellationToken token)
        {
            while (true)
            {
                token.ThrowIfCancellationRequested();
                if (await IsRabbitMqReady().ConfigureAwait(false))
                    return;
                await Task.Delay(500, token).ConfigureAwait(false);
            }
        }

        private async Task<bool> IsRabbitMqReady()
        {
            var rabbitMqManagementApi = new ManagementClient(RabbitContainerHostForManagement, Configuration.RabbitMqUser, Configuration.RabbitMqPassword, Configuration.RabbitMqManagementPort);

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