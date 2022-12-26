namespace EasyNetQ.Management.Client.IntegrationTests;

[CollectionDefinition("RabbitMQ")]
public class RabbitMqFixtureCollection : ICollectionFixture<RabbitMqFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
