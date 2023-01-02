using System.Text.Json.Serialization;
using EasyNetQ.Management.Client.Serialization;

namespace EasyNetQ.Management.Client.Model;


public class Connection
{
    public long RecvOct { get; set; }
    public long RecvCnt { get; set; }
    public long SendOct { get; set; }
    public long SendCnt { get; set; }
    public long SendPend { get; set; }
    public string State { get; set; } = null!;
    public string? LastBlockedBy { get; set; }
    public string? LastBlockedAge { get; set; }
    public long Channels { get; set; }
    public string Type { get; set; } = null!;
    public string Node { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Address { get; set; }
    public int Port { get; set; }
    public string PeerHost { get; set; } = null!;
    public int PeerPort { get; set; }
    public bool Ssl { get; set; }
    public string? PeerCertSubject { get; set; }
    public string? PeerCertIssuer { get; set; }
    public string? PeerCertValidity { get; set; }
    public string AuthMechanism { get; set; } = null!;
    public string? SslProtocol { get; set; }
    public string? SslKeyExchange { get; set; }
    public string? SslCipher { get; set; }
    public string? SslHash { get; set; }
    public string Protocol { get; set; } = null!;
    public string User { get; set; } = null!;
    public string Vhost { get; set; } = null!;
    public long Timeout { get; set; }
    public long FrameMax { get; set; }
    [JsonConverter(typeof(StringObjectReadOnlyDictionaryConverter))]
    public IReadOnlyDictionary<string, object?> ClientProperties { get; set; } = null!;
}
