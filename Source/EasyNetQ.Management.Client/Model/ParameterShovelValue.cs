namespace EasyNetQ.Management.Client.Model
{
    using Newtonsoft.Json;

    /// <summary>
    /// About shovel parameters: https://www.rabbitmq.com/shovel-dynamic.html
    /// </summary>
    public class ParameterShovelValue
    {
        /// <summary>
        /// The AMQP URI(s) for the source. Mandatory. See the AMQP URI reference for information on how RabbitMQ treats AMQP URIs in general, and the query parameter reference for the Erlang client's extensions (including those for SSL and SASL) which are available to the shovel.
        /// Note that this field can either be a string, or a list of strings. If more than one string is provided, the shovel will randomly pick one URI from the list. This can be used to connect to a cluster and ensure the link will eventually find another node in the event that one fails. It's probably not a great idea to use multiple URIs that do not point to the same cluster.
        /// </summary>
        [JsonProperty(PropertyName = "src-uri")]
        public string srcuri { get; set; }

        /// <summary>
        /// The exchange from which to consume. Either this or src-queue (but not both) must be set.
        /// The shovel will declare an exclusive queue and bind it to the named exchange with src-exchange-key before consuming from the queue.
        /// If the source exchange does not exist on the source broker, it will be not declared; the shovel will fail to start.
        /// </summary>
        [JsonProperty(PropertyName = "src-exchange")]
        public string srcexchange { get; set; }

        /// <summary>
        /// Routing key when using src-exchange
        /// </summary>
        [JsonProperty(PropertyName = "src-exchange-key")]
        public string srcexchangekey { get; set; }

        /// <summary>
        /// The queue from which to consume. Either this or src-exchange (but not both) must be set.
        /// If the source queue does not exist on the source broker, it will be declared as a durable queue with no arguments.
        /// </summary>
        [JsonProperty(PropertyName = "src-queue")]
        public string srcqueue { get; set; }

        /// <summary>
        /// The AMQP URI(s) for the destination. Mandatory. See src-uri above.
        /// </summary>
        [JsonProperty(PropertyName = "dest-uri")]
        public string desturi { get; set; }

        /// <summary>
        /// The exchange to which messages should be published. Either this or dest-queue (but not both) may be set.
        /// If the destination exchange does not exist on the source broker, it will be not declared; the shovel will fail to start.
        /// </summary>
        [JsonProperty(PropertyName = "dest-exchange")]
        public string destexchange { get; set; }

        /// <summary>
        /// The queue to which messages should be published. Either this or dest-exchange (but not both) may be set. If neither is set then messages are republished with their original exchange and routing key.
        /// If the destination queue does not exist on the source broker, it will be declared as a durable queue with no arguments.
        /// </summary>
        [JsonProperty(PropertyName = "dest-queue")]
        public string destqueue { get; set; }

        /// <summary>
        /// Determines how the shovel should acknowledge messages. If set to on-confirm (the default), messages are acknowledged to the source broker after they have been confirmed by the destination. This handles network errors and broker failures without losing messages, and is the slowest option.
        /// If set to on-publish, messages are acknowledged to the source broker after they have been published at the destination. This handles network errors without losing messages, but may lose messages in the event of broker failures.
        /// If set to no-ack, message acknowledgements are not used. This is the fastest option, but may lose messages in the event of network or broker failures.
        /// </summary>
        [JsonProperty(PropertyName = "ack-mode")]
        public string ackmode { get; set; }

        /// <summary>
        /// Whether to add x-shovelled headers to the shovelled messages indicating where they have been shovelled from and to. Default is false.
        /// </summary>
        [JsonProperty(PropertyName = "add-forward-headers")]
        public string addforwardheaders { get; set; }

        /// <summary>
        /// Determines when (if ever) the shovel should delete itself. This can be useful if the shovel is being treated as more of a move operation - i.e. being used to move messages from one queue to another on an ad hoc basis.
        /// The default is never, meaning the shovel should never delete itself.
        /// If set to queue-length then the shovel will measure the length of the source queue when starting up, and delete itself after it has transfered that many messages.
        /// If set to an integer, then the shovel will transfer that number of messages before deleting itself.
        /// </summary>
        [JsonProperty(PropertyName = "delete-after")]
        public string deleteafter { get; set; }
    }
}