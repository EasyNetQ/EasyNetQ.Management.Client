using System;

namespace EasyNetQ.Management.Client.Model
{
    public class Connection
    {
        public long RecvOct { get; set; }
        public long RecvCnt { get; set; }
        public long SendOct { get; set; }
        public long SendCnt { get; set; }
        public long SendPend { get; set; }
        public string State { get; set; }
        public string LastBlockedBy { get; set; }
        public string LastBlockedAge { get; set; }
        public long Channels { get; set; }
        public string Type { get; set; }
        public string Node { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public int Port { get; set; }
        //PeerAddress is the connected Peer IP Address for RabbitMQ Version <3.0
        public string PeerAddress { get; set; }
        //PeerAddress is the connected Peer IP Address for RabbitMQ Version >3.0
        public string PeerHost { get; set; }
        public int PeerPort { get; set; }
        public bool Ssl { get; set; }
        public string PeerCertSubject { get; set; }
        public string PeerCertIssuer { get; set; }
        public string PeerCertValidity { get; set; }
        public string AuthMechanism { get; set; }
        public string SslProtocol { get; set; }
        public string SslKeyExchange { get; set; }
        public string SslCipher { get; set; }
        public string SslHash { get; set; }
        public string Protocol { get; set; }
        public string User { get; set; }
        public string Vhost { get; set; }
        public long Timeout { get; set; }
        public long FrameMax { get; set; }
        public ClientProperties ClientProperties { get; set; }
    }
}