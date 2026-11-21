[![CI](https://github.com/EasyNetQ/EasyNetQ.Management.Client/actions/workflows/ci.yml/badge.svg?branch=master)](https://github.com/EasyNetQ/EasyNetQ.Management.Client/actions/workflows/ci.yml)

[![NuGet Status](https://img.shields.io/nuget/v/EasyNetQ.Management.Client)](https://www.nuget.org/packages/EasyNetQ.Management.Client)
[![Nuget Status](https://img.shields.io/nuget/vpre/EasyNetQ.Management.Client)](https://www.nuget.org/packages/EasyNetQ.Management.Client)
[![Nuget Status](https://img.shields.io/nuget/dt/EasyNetQ.Management.Client)](https://www.nuget.org/packages/EasyNetQ.Management.Client)
----------

## EasyNetQ.Management.Client

A .NET client for the [RabbitMQ management HTTP API](https://www.rabbitmq.com/docs/management#http-api): create virtual
hosts, users and permissions; declare exchanges, queues, bindings and policies; monitor queues, connections, channels
and nodes; run health checks.

> **Built on [Refit](https://github.com/reactiveui/refit).** Since 4.0, `IManagementClient` is a Refit interface: each
> management endpoint is one annotated method, and Refit's source generator writes the HTTP requests at build time.
> JSON goes through a System.Text.Json source-generated context. No reflection at run time, so the client works in
> trimmed and **Native AOT** apps.

```shell
dotnet add package EasyNetQ.Management.Client
```

```csharp
using var client = new ManagementClient(new Uri("http://localhost:15672"), "guest", "guest");

var queues = await client.GetQueuesAsync("/");
await client.CreateQueueAsync("/", "orders", new QueueInfo(Arguments: new Dictionary<string, object?> { ["x-queue-type"] = "quorum" }));
var orders = await client.GetQueueAsync("/", "orders");
await client.PurgeAsync(orders);
```

- **Targets** .NET 10, .NET 11 and .NET Framework 4.8
- **Your own `HttpClient`**: `new ManagementClient(httpClient, user, password)` uses its `BaseAddress`, handlers and timeout, and leaves it open on dispose
- **Errors**: a failed call throws `UnexpectedHttpStatusCodeException` with the `StatusCode` and the response body
- **Overloads taking the model**: methods with a vhost, queue, exchange or user name also accept a `Vhost`, `QueueName`, `ExchangeName` or `User`
- **Testing**: depend on `IManagementClient` and mock it, or give `ManagementClient` an `HttpClient` with a fake handler

See the **[documentation](https://github.com/EasyNetQ/EasyNetQ/wiki/Management-Client-v4)**.

### Upgrading to 4.0

- Every call is async; the generated synchronous overloads are gone
- Classic queue mirroring, removed in RabbitMQ 4.0, is gone: `HaMode`, `HaParams`, the `ha-*` policy fields and `HaveAnyClassicQueuesWithoutSynchronisedMirrorsAsync`
- The raw `GetAsync`/`PutAsync`/`PostAsync`/`DeleteAsync` over `RelativePath` and the obsolete host-name constructor are gone; `Endpoint` is on `ManagementClient`
- A parameter's value is a `ParameterValue`; under Native AOT it must be a shovel or federation value, a string, number or bool
- The assembly is no longer strong-named

## Contributors

Thanks to all the people who already contributed!

<a href="https://github.com/EasyNetQ/EasyNetQ.Management.Client/graphs/contributors">
  <img src="https://contributors-img.web.app/image?repo=EasyNetQ/EasyNetQ.Management.Client" />
</a>
