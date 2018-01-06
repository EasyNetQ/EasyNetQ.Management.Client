using System;

namespace EasyNetQ.Management.Client.Model
{
    public class QueueInfo
    {
        private readonly string name;

        public bool AutoDelete { get; private set; }
        public bool Durable { get; private set; }
        public InputArguments Arguments { get; private set; }

        public QueueInfo(string name, bool autoDelete, bool durable, InputArguments arguments)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (arguments == null)
            {
                throw new ArgumentNullException(nameof(arguments));
            }

            this.name = name;
            AutoDelete = autoDelete;
            Durable = durable;
            Arguments = arguments;
        }

        public QueueInfo(string name) :
            this(name, false, true, new InputArguments())
        {
        }

        public string GetName()
        {
            return name;
        }
    }
}