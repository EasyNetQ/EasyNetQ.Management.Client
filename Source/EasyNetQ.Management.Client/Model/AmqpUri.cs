namespace EasyNetQ.Management.Client.Model
{
    public class AmqpUri
    {
        public string HostName { get; set; }
        public int? Port { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }

        public AmqpUri(string hostName)
        {
            HostName = hostName;
        }

        public AmqpUri(string hostName, int? port) : this(hostName)
        {
            Port = port;
        }

        public AmqpUri(string hostName, int? port, string? username, string? password) : this(hostName, port)
        {
            Username = username;
            Password = password;
        }

        public override string ToString()
        {
            var userInfo = "";
            if (string.IsNullOrEmpty(Username) && string.IsNullOrEmpty(Password))
            {
                userInfo = $"{Username}:{Password}@";
            }

            var portInfo = "";
            if (Port.GetValueOrDefault(0) > 0)
            {
                portInfo = $":{Port}";
            }

            return $"amqp://{userInfo}{HostName}{portInfo}";
        }
    }
}
