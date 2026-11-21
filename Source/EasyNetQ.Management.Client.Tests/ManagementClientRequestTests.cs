#nullable enable
using System.Net;
using System.Reflection;
using System.Text;
using EasyNetQ.Management.Client.Model;
using EasyNetQ.Management.Client.Serialization;
using Refit;
#if NETFRAMEWORK
using System.Net.Http;
#endif

namespace EasyNetQ.Management.Client.Tests;

public class ManagementClientRequestTests
{
    private sealed record SentRequest(HttpMethod Method, string Uri, string? Authorization, string? Body, long? ContentLength);

    private sealed class FakeHandler(HttpStatusCode statusCode, string content) : HttpMessageHandler
    {
        public List<SentRequest> Requests { get; } = [];

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var body = request.Content == null ? null : await request.Content.ReadAsStringAsync().ConfigureAwait(false);
            Requests.Add(new SentRequest(
                request.Method,
                request.RequestUri!.AbsoluteUri,
                request.Headers.Authorization?.ToString(),
                body,
                request.Content?.Headers.ContentLength));
            return new HttpResponseMessage(statusCode)
            {
                RequestMessage = request,
                Content = new StringContent(content, Encoding.UTF8, "application/json")
            };
        }
    }

    private static (ManagementClient Client, FakeHandler Handler) Create(
        HttpStatusCode statusCode = HttpStatusCode.OK, string content = "{}", string baseAddress = "http://localhost:15672")
    {
        var handler = new FakeHandler(statusCode, content);
        var client = new ManagementClient(new HttpClient(handler) { BaseAddress = new Uri(baseAddress) }, "guest", "secret");
        return (client, handler);
    }

    private const string QueueJson = """{"name":"my queue","vhost":"/","type":"quorum","arguments":{}}""";

    [Fact]
    public async Task Escapes_the_default_vhost_and_names_in_the_path()
    {
        var (client, handler) = Create(content: QueueJson);

        var queue = await client.GetQueueAsync("/", "my queue");

        queue.Type.Should().Be(QueueType.Quorum);
        handler.Requests.Single().Should().Be(new SentRequest(
            HttpMethod.Get, "http://localhost:15672/api/queues/%2F/my%20queue", "Basic Z3Vlc3Q6c2VjcmV0", null, null));
    }

    [Theory]
    [InlineData("http://localhost:15672/rabbitmq")]
    [InlineData("http://localhost:15672/rabbitmq/")]
    public async Task Keeps_the_base_address_path(string baseAddress)
    {
        var (client, handler) = Create(content: "[]", baseAddress: baseAddress);

        await client.GetNodesAsync();

        handler.Requests.Single().Uri.Should().Be("http://localhost:15672/rabbitmq/api/nodes");
    }

    [Fact]
    public async Task Sends_page_criteria_as_query_parameters()
    {
        var (client, handler) = Create(content: """{"filtered_count":0,"item_count":0,"items":[],"page":2,"page_count":0,"page_size":10,"total_count":0}""");

        await client.GetQueuesByPageAsync("/", new PageCriteria(2, 10, "orders", UseRegex: true));

        handler.Requests.Single().Uri.Should().Be(
            "http://localhost:15672/api/queues/%2F?page=2&page_size=10&name=orders&use_regex=true&pagination=true");
    }

    [Fact]
    public async Task Sends_every_criteria_argument()
    {
        var (client, handler) = Create(content: QueueJson);

        await client.GetQueueAsync("/", "q", new LengthsCriteria(60, 5), new RatesCriteria(600, 10));

        handler.Requests.Single().Uri.Should().Be(
            "http://localhost:15672/api/queues/%2F/q?lengths_age=60&lengths_incr=5&msg_rates_age=600&msg_rates_incr=10");
    }

    [Fact]
    public async Task Sends_no_query_without_criteria()
    {
        var (client, handler) = Create(statusCode: HttpStatusCode.NoContent, content: "");

        await client.DeleteQueueAsync("/", "q");
        await client.DeleteQueueAsync("/", "q", DeleteQueueCriteria.IfUnusedAndEmpty);

        handler.Requests.Select(r => r.Uri).Should().Equal(
            "http://localhost:15672/api/queues/%2F/q",
            "http://localhost:15672/api/queues/%2F/q?if-unused=true&if-empty=true");
    }

    [Fact]
    public async Task Sends_a_snake_case_body_with_a_content_length()
    {
        var (client, handler) = Create(statusCode: HttpStatusCode.Created, content: "");

        await client.CreateQueueAsync("/", "q", new QueueInfo(AutoDelete: true, Durable: false));

        var request = handler.Requests.Single();
        request.Method.Should().Be(HttpMethod.Put);
        request.Body.Should().Be("""{"auto_delete":true,"durable":false}""");
        request.ContentLength.Should().Be(request.Body!.Length);
    }

    [Fact]
    public async Task Enables_tracing_through_the_vhost()
    {
        var (client, handler) = Create(statusCode: HttpStatusCode.NoContent, content: "");

        await client.EnableTracingAsync("/");

        handler.Requests.Single().Should().Be(new SentRequest(
            HttpMethod.Put, "http://localhost:15672/api/vhosts/%2F", "Basic Z3Vlc3Q6c2VjcmV0", """{"tracing":true}""", 16));
    }

    [Fact]
    public async Task Wraps_a_parameter_value()
    {
        var (client, handler) = Create(statusCode: HttpStatusCode.Created, content: "");

        await client.CreateParameterAsync("custom", "/", "limit", 5);

        handler.Requests.Single().Body.Should().Be("""{"value":5}""");
    }

    [Fact]
    public async Task Throws_the_status_code_of_an_unexpected_response()
    {
        var (client, _) = Create(statusCode: HttpStatusCode.NotFound, content: """{"error":"Object Not Found"}""");

        var exception = await Assert.ThrowsAsync<UnexpectedHttpStatusCodeException>(() => client.GetQueueAsync("/", "missing"));

        exception.StatusCode.Should().Be(HttpStatusCode.NotFound);
        exception.Message.Should().Contain("Object Not Found");
    }

    [Theory]
    [InlineData(HttpStatusCode.OK, true)]
    [InlineData(HttpStatusCode.ServiceUnavailable, false)]
    public async Task Reads_a_health_check(HttpStatusCode statusCode, bool alive)
    {
        var (client, handler) = Create(statusCode: statusCode, content: """{"status":"ok"}""");

        (await client.IsAliveAsync("/")).Should().Be(alive);
        (await client.HaveAnyClusterAlarmsAsync()).Should().Be(!alive);

        handler.Requests.Select(r => r.Uri).Should().Equal(
            "http://localhost:15672/api/aliveness-test/%2F",
            "http://localhost:15672/api/health/checks/alarms");
    }

    [Fact]
    public async Task Throws_when_a_health_check_fails_otherwise()
    {
        var (client, _) = Create(statusCode: HttpStatusCode.InternalServerError, content: "boom");

        var exception = await Assert.ThrowsAsync<UnexpectedHttpStatusCodeException>(() => client.IsAliveAsync("/"));

        exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Leaves_a_supplied_http_client_open()
    {
        var handler = new FakeHandler(HttpStatusCode.OK, "[]");
        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("http://localhost:15672") };

        new ManagementClient(httpClient, "guest", "guest").Dispose();

        using var response = await httpClient.GetAsync("/api/nodes");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Sends_the_certificate_expiration_unit_in_lower_case()
    {
        var (client, handler) = Create(statusCode: HttpStatusCode.ServiceUnavailable, content: """{"status":"failed"}""");

        (await client.HaveAnyCertificatesExpiringAsync(3, CertificateExpirationUnit.Weeks)).Should().BeTrue();

        handler.Requests.Single().Uri.Should().Be("http://localhost:15672/api/health/checks/certificate-expiration/3/weeks");
    }

    [Fact]
    public async Task Bulk_deletes_users_by_name()
    {
        var (client, handler) = Create(statusCode: HttpStatusCode.NoContent, content: "");

        await client.DeleteUsersAsync(["alice", "bob"]);

        handler.Requests.Single().Should().Be(new SentRequest(
            HttpMethod.Post, "http://localhost:15672/api/users/bulk-delete", "Basic Z3Vlc3Q6c2VjcmV0", """{"users":["alice","bob"]}""", 25));
    }

    [Fact]
    public async Task Reads_vhost_limits_with_dashed_names()
    {
        var (client, handler) = Create(content: """[{"vhost":"/","value":{"max-connections":10,"max-queues":5}}]""");

        var limits = await client.GetVhostLimitsAsync("/");

        limits.Single().Should().Be(new VhostLimits("/", new VhostLimitValues(MaxConnections: 10, MaxQueues: 5)));
        handler.Requests.Single().Uri.Should().Be("http://localhost:15672/api/vhost-limits/%2F");
    }

    [Fact]
    public async Task Escapes_the_user_of_a_connection_listing()
    {
        var (client, handler) = Create(content: "[]");

        await client.GetUserConnectionsAsync("ci/deploy user");

        handler.Requests.Single().Uri.Should().Be("http://localhost:15672/api/connections/username/ci%2Fdeploy%20user");
    }

    // Native AOT serializes only what the context lists.
    [Fact]
    public void Json_context_lists_every_request_and_response_type()
    {
        var types = typeof(IManagementClient).GetMethods()
            .SelectMany(method => method.GetParameters()
                .Where(parameter => parameter.GetCustomAttribute<BodyAttribute>() != null)
                .Select(parameter => parameter.ParameterType)
                .Append(method.ReturnType))
            .Select(type => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>) ? type.GetGenericArguments()[0] : type)
            .Where(type => type != typeof(Task) && type != typeof(IApiResponse))
            .Distinct();

        types.Where(type => ManagementJsonContext.Default.GetTypeInfo(type) == null).Should().BeEmpty();
    }
}
