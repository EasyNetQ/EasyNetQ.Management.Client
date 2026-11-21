using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using EasyNetQ.Management.Client.Serialization;
using Refit;

#if NET
using HttpHandler = System.Net.Http.SocketsHttpHandler;
#else
using System.Collections.Concurrent;
using System.Reflection;

using HttpHandler = System.Net.Http.HttpClientHandler;
#endif

namespace EasyNetQ.Management.Client;

/// <summary>
///     A client for the RabbitMQ management HTTP API. Its <see cref="IManagementClient"/> methods are generated.
/// </summary>
public partial class ManagementClient : IManagementClient
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(20);

    internal static readonly JsonSerializerOptions SerializerOptions = CreateSerializerOptions();

    private readonly HttpClient httpClient;
    private readonly bool disposeHttpClient;
    private readonly IManagementClient api;

#if NETFRAMEWORK
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

        var httpHandler = new HttpHandler();
        configureHttpHandler?.Invoke(httpHandler);
        HttpMessageHandler handler = configureHttpRequestMessage == null
            ? httpHandler
            : new ConfiguringHandler(configureHttpRequestMessage, httpHandler);
        httpClient = new HttpClient(handler) { Timeout = timeout ?? DefaultTimeout, BaseAddress = endpoint };
        disposeHttpClient = true;
        api = CreateApi(httpClient, username, password);
    }

    public ManagementClient(HttpClient httpClient, string username, string password)
    {
        if (httpClient.BaseAddress == null)
            throw new ArgumentNullException(nameof(httpClient.BaseAddress), "Endpoint should be specified");

        if (!httpClient.BaseAddress.IsAbsoluteUri)
            throw new ArgumentOutOfRangeException(nameof(httpClient.BaseAddress), httpClient.BaseAddress, "Endpoint should be absolute");

        this.httpClient = httpClient;
        disposeHttpClient = false;
        api = CreateApi(httpClient, username, password);
    }

    /// <summary>
    ///     The endpoint that the client is using.
    /// </summary>
    public Uri Endpoint => httpClient.BaseAddress!;

    public void Dispose()
    {
        if (disposeHttpClient)
            httpClient.Dispose();
    }

    private static IManagementClient CreateApi(HttpClient httpClient, string username, string password)
    {
#if NETFRAMEWORK
        LeaveDotsAndSlashesEscaped(httpClient.BaseAddress!.Scheme);
#endif
        var basicAuth = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));
        var settings = new RefitSettings(new SystemTextJsonContentSerializer(SerializerOptions))
        {
            AuthorizationHeaderValueGetter = (_, _) => new ValueTask<string>(basicAuth),
            ExceptionFactory = async response => response.IsSuccessStatusCode
                ? null
                : await UnexpectedHttpStatusCodeException.FromHttpResponseMessageAsync(response).ConfigureAwait(false),
            DeserializationExceptionFactory = async (response, exception) =>
            {
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return new JsonException($"Response content doesn't conform to the JSON schema: {content}", exception);
            },
            // RabbitMQ needs a Content-Length, not a chunked body.
            Buffered = true,
        };
        return RestService.ForGenerated<IManagementClient>(httpClient, settings);
    }

    // Reflection serializes values the context does not list, such as a parameter's own type, unless trimmed or Native AOT.
#if NET
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Only when JsonSerializer.IsReflectionEnabledByDefault, which trimming turns off")]
    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "Only when JsonSerializer.IsReflectionEnabledByDefault, which Native AOT turns off")]
#endif
    private static JsonSerializerOptions CreateSerializerOptions() =>
        new(ManagementJsonContext.Default.Options)
        {
            TypeInfoResolver = JsonSerializer.IsReflectionEnabledByDefault
                ? JsonTypeInfoResolver.Combine(ManagementJsonContext.Default, new DefaultJsonTypeInfoResolver())
                : ManagementJsonContext.Default
        };

    private sealed class ConfiguringHandler(Action<HttpRequestMessage> configure, HttpMessageHandler inner) : DelegatingHandler(inner)
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            configure(request);
            return base.SendAsync(request, cancellationToken);
        }
    }
}
