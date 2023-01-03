using System.Text.Json.Serialization;
using EasyNetQ.Management.Client.Serialization;

namespace EasyNetQ.Management.Client.Model;

public record Connection(
    long RecvOct,
    long RecvCnt,
    long SendOct,
    long SendCnt,
    long SendPend,
    string State,
    string? LastBlockedBy,
    string? LastBlockedAge,
    long Channels,
    string Type,
    string Node,
    string Name,
    string? Address,
    int Port,
    string PeerHost,
    int PeerPort,
    bool Ssl,
    string? PeerCertSubject,
    string? PeerCertIssuer,
    string? PeerCertValidity,
    string AuthMechanism,
    string? SslProtocol,
    string? SslKeyExchange,
    string? SslCipher,
    string? SslHash,
    string Protocol,
    string User,
    string Vhost,
    long Timeout,
    long FrameMax,
    [property: JsonConverter(typeof(StringObjectReadOnlyDictionaryConverter))]
    IReadOnlyDictionary<string, object?> ClientProperties
);
