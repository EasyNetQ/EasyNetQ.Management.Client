using System.Collections.Generic;

namespace EasyNetQ.Management.Client.Model
{
    public class User
    {
        public string Name { get; set; }
        public string PasswordHash { get; set; }
        public List<string> Tags { get; set; }
    }
}