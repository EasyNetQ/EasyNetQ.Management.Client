using System;
using System.Collections.Generic;

namespace EasyNetQ.Management.Client.Model;

public class PublishInfo
{
    public IDictionary<string, object> Properties { get; private set; }
    public string RoutingKey { get; private set; }
    public string Payload { get; private set; }
    public string PayloadEncoding { get; private set; }

    private readonly ISet<string> payloadEncodings = new HashSet<string> { "string", "base64" };

    public PublishInfo(IDictionary<string, object> properties, string routingKey, string payload, string payloadEncoding)
    {
        if (properties == null)
        {
            throw new ArgumentNullException(nameof(properties));
        }
        if (routingKey == null)
        {
            throw new ArgumentNullException(nameof(routingKey));
        }
        if (payload == null)
        {
            throw new ArgumentNullException(nameof(payload));
        }
        if (payloadEncoding == null)
        {
            throw new ArgumentNullException(nameof(payloadEncoding));
        }
        if (!payloadEncodings.Contains(payloadEncoding))
        {
            throw new ArgumentException($"payloadEncoding must be one of: '{string.Join(", ", payloadEncodings)}'");
        }

        Properties = properties;
        RoutingKey = routingKey;
        Payload = payload;
        PayloadEncoding = payloadEncoding;
    }

    public PublishInfo(string routingKey, string payload) :
        this(new Dictionary<string, object>(), routingKey, payload, "string")
    {
    }
}
