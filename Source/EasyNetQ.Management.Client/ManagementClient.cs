using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using EasyNetQ.Management.Client.Internals;
using EasyNetQ.Management.Client.Serialization;

#if NET6_0
using HttpHandler = System.Net.Http.SocketsHttpHandler;
#else
using System.Collections.Concurrent;
using System.Reflection;

using HttpHandler = System.Net.Http.HttpClientHandler;
#endif

namespace EasyNetQ.Management.Client;

public class ManagementClient : IManagementClient
{
    private static readonly MediaTypeWithQualityHeaderValue JsonMediaTypeHeaderValue = new("application/json");
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(20);

    internal static readonly JsonSerializerOptions SerializerOptions;

    private readonly HttpClient httpClient;
    private readonly Action<HttpRequestMessage>? configureHttpRequestMessage;
    private readonly bool disposeHttpClient;
    private readonly AuthenticationHeaderValue basicAuthHeader;

    static ManagementClient()
    {
        var namingPolicy = new JsonSnakeCaseNamingPolicy();
        SerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = namingPolicy,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters =
            {
                new JsonStringEnumConverter(namingPolicy),
                new HaParamsConverter()
            }
        };
    }

#if NETSTANDARD2_0
    private static bool DisableUriParserLegacyQuirks(string scheme)
    {
        string uriWithEscapedDotsAndSlashes = $"{scheme}://localhost/{Uri.EscapeDataString("/.")}";
        string uriParsed = new Uri(uriWithEscapedDotsAndSlashes).ToString();
        if (uriParsed == uriWithEscapedDotsAndSlashes)
        {
            return false;
        }

        // https://mikehadlow.blogspot.com/2011/08/how-to-stop-systemuri-un-escaping.html
        var getSyntaxMethod = typeof(UriParser).GetMethod("GetSyntax", BindingFlags.Static | BindingFlags.NonPublic);
        if (getSyntaxMethod == null)
        {
            throw new MissingMethodException(nameof(UriParser), "GetSyntax");
        }
        var uriParser = getSyntaxMethod.Invoke(null, new object[] { scheme })!;

        var setUpdatableFlagsMethod = typeof(UriParser).GetMethod("SetUpdatableFlags", BindingFlags.Instance | BindingFlags.NonPublic);
        if (setUpdatableFlagsMethod == null)
        {
            throw new MissingMethodException(nameof(UriParser), "SetUpdatableFlags");
        }
        setUpdatableFlagsMethod.Invoke(uriParser, new object[] { 0 });

        uriParsed = new Uri(uriWithEscapedDotsAndSlashes).ToString();
        if (uriParsed != uriWithEscapedDotsAndSlashes)
        {
            string? targetFrameworkName = "unknown";
            var setupInformationProperty = typeof(AppDomain).GetProperty("SetupInformation");
            if (setupInformationProperty != null)
            {
                var setupInformation = setupInformationProperty.GetValue(AppDomain.CurrentDomain);
                var targetFrameworkNameProperty = setupInformation.GetType().GetProperty("TargetFrameworkName");
                if (targetFrameworkNameProperty != null)
                {
                    targetFrameworkName = targetFrameworkNameProperty.GetValue(setupInformation) as string;
                }
            }
            throw new NotImplementedException($"Preserving slashes and dots escaped in System.Uri is not supported in TargetFramework={targetFrameworkName}. Expected={uriWithEscapedDotsAndSlashes}, actual={uriParsed}.");
        }

        return true;
    }

    private static ConcurrentDictionary<string, bool> s_fixedSchemes = new ConcurrentDictionary<string, bool>();
    private static void LeaveDotsAndSlashesEscaped(string scheme)
    {
        s_fixedSchemes.GetOrAdd(scheme, DisableUriParserLegacyQuirks);
    }
#endif

    [Obsolete("Please use another constructor")]
    public ManagementClient(
        string hostUrl,
        string username,
        string password,
        int portNumber = 15672,
        TimeSpan? timeout = null,
        Action<HttpRequestMessage>? configureHttpRequestMessage = null,
        bool ssl = false,
        Action<HttpHandler>? configureHttpHandler = null
    ) : this(
        LegacyEndpointBuilder.Build(hostUrl, portNumber, ssl),
        username,
        password,
        timeout,
        configureHttpRequestMessage,
        configureHttpHandler
    )
    {
    }

    public ManagementClient(
        Uri endpoint,
        string username,
        string password,
        TimeSpan? timeout = null,
        Action<HttpRequestMessage>? configureHttpRequestMessage = null,
        Action<HttpHandler>? configureHttpHandler = null
    )
    {
        if (!endpoint.IsAbsoluteUri) throw new ArgumentOutOfRangeException(nameof(endpoint), endpoint, "Endpoint should be absolute");

        this.configureHttpRequestMessage = configureHttpRequestMessage;

        var httpHandler = new HttpHandler();
        configureHttpHandler?.Invoke(httpHandler);
        basicAuthHeader = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}")));
        httpClient = new HttpClient(httpHandler) { Timeout = timeout ?? DefaultTimeout, BaseAddress = endpoint };
        disposeHttpClient = true;

#if NETSTANDARD2_0
        LeaveDotsAndSlashesEscaped(Endpoint.Scheme);
#endif
    }

    public ManagementClient(HttpClient httpClient, string username, string password)
    {
        if (httpClient.BaseAddress == null)
            throw new ArgumentNullException(nameof(httpClient.BaseAddress), "Endpoint should be specified");

        if (!httpClient.BaseAddress.IsAbsoluteUri)
            throw new ArgumentOutOfRangeException(nameof(httpClient.BaseAddress), httpClient.BaseAddress, "Endpoint should be absolute");

        this.httpClient = httpClient;
        basicAuthHeader = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}")));
        configureHttpRequestMessage = null;
        disposeHttpClient = false;

#if NETSTANDARD2_0
        LeaveDotsAndSlashesEscaped(Endpoint.Scheme);
#endif
    }

    public Uri Endpoint => httpClient.BaseAddress!;

    public void Dispose()
    {
        if (disposeHttpClient)
            httpClient.Dispose();
    }

    public async Task<bool> CheckAsync(
        RelativePath path,
        CancellationToken cancellationToken = default
    )
    {
        using var request = CreateRequest(HttpMethod.Get, path);
        using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

        if (response.StatusCode == HttpStatusCode.OK)
            return true;

        if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
            return false;

        throw new UnexpectedHttpStatusCodeException(response);
    }

    public async Task<TResult> GetAsync<TResult>(
        RelativePath path,
        IEnumerable<KeyValuePair<string, string>>? queryParameters,
        CancellationToken cancellationToken = default
    )
    {
        using var request = CreateRequest(HttpMethod.Get, path, queryParameters);
        using var response = await httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

        response.EnsureExpectedStatusCode(statusCode => statusCode == HttpStatusCode.OK);

        try
        {
            return (await response.Content.ReadFromJsonAsync<TResult>(SerializerOptions, cancellationToken).ConfigureAwait(false))!;
        }
        catch (Exception e) when (e is ArgumentNullException || e is JsonException)
        {
            string stringContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            throw new JsonException($"Response content doesn't conform to the JSON schema: {stringContent}", e);
        }
    }

    public async Task<TResult> PostAsync<TItem, TResult>(
        RelativePath path,
        TItem item,
        CancellationToken cancellationToken = default
    )
    {
        using var requestContent = JsonContent.Create(item, JsonMediaTypeHeaderValue, SerializerOptions);
        using var request = CreateRequest(HttpMethod.Post, path, null, requestContent);
        using var response = await httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

        response.EnsureExpectedStatusCode(statusCode => statusCode is HttpStatusCode.OK or HttpStatusCode.Created or HttpStatusCode.NoContent);

        try
        {
            return (await response.Content.ReadFromJsonAsync<TResult>(SerializerOptions, cancellationToken).ConfigureAwait(false))!;
        }
        catch (Exception e) when (e is ArgumentNullException || e is JsonException)
        {
            string stringContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            throw new JsonException($"Response content doesn't conform to the JSON schema: {stringContent}", e);
        }
    }

    public async Task PostAsync<TItem>(
        RelativePath path,
        TItem item,
        CancellationToken cancellationToken = default
    )
    {
        using var requestContent = JsonContent.Create(item, JsonMediaTypeHeaderValue, SerializerOptions);
        using var request = CreateRequest(HttpMethod.Post, path, null, requestContent);
        using var response = await httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

        response.EnsureExpectedStatusCode(statusCode => statusCode is HttpStatusCode.OK or HttpStatusCode.Created or HttpStatusCode.NoContent);
    }

    public async Task PostAsync(
        RelativePath path,
        CancellationToken cancellationToken = default
    )
    {
        using var request = CreateRequest(HttpMethod.Post, path);
        using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

        response.EnsureExpectedStatusCode(statusCode => statusCode is HttpStatusCode.OK or HttpStatusCode.Created or HttpStatusCode.NoContent);
    }

    public async Task DeleteAsync(RelativePath path, CancellationToken cancellationToken = default)
    {
        using var request = CreateRequest(HttpMethod.Delete, path);
        using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

        response.EnsureExpectedStatusCode(statusCode => statusCode == HttpStatusCode.NoContent);
    }

    public async Task PutAsync<TBody>(
        RelativePath path,
        TBody item,
        CancellationToken cancellationToken = default
    )
    {
        using var requestContent = JsonContent.Create(item, JsonMediaTypeHeaderValue, SerializerOptions);
        using var request = CreateRequest(HttpMethod.Put, path, null, requestContent);
        using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

        response.EnsureExpectedStatusCode(statusCode => statusCode is HttpStatusCode.Created or HttpStatusCode.NoContent);
    }

    public async Task PutAsync(RelativePath path, CancellationToken cancellationToken = default)
    {
        using var request = CreateRequest(HttpMethod.Put, path);
        using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

        response.EnsureExpectedStatusCode(statusCode => statusCode is HttpStatusCode.Created or HttpStatusCode.NoContent);
    }

    private HttpRequestMessage CreateRequest(
        HttpMethod httpMethod,
        in RelativePath path,
        IEnumerable<KeyValuePair<string, string>>? queryParameters = null,
        HttpContent? content = null
    )
    {
        var request = new HttpRequestMessage(httpMethod, QueryStringHelpers.AddQueryString(path.Build(), queryParameters));
        request.Headers.Authorization = basicAuthHeader;
        request.Content = content;
        configureHttpRequestMessage?.Invoke(request);
        return request;
    }
}
