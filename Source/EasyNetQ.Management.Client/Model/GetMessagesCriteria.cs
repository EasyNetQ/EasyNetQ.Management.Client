using System.Collections.Generic;

namespace EasyNetQ.Management.Client.Model
{
    /// <summary>
    /// The criteria for retrieving messages from a queue
    /// </summary>
    public class GetMessagesCriteria
    {
        public long Count { get; private set; }
        public string Ackmode { get; private set; }
        public string Encoding { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="count">
        /// Controls the number of messages to get. You may get fewer messages than this if the queue cannot immediately provide them.
        /// </param>
        /// <param name="ackmode">
        /// Determines if the message(s) should be placed back into the queue.
        /// </param>
        public GetMessagesCriteria(long count, Ackmodes ackmode)
        {
            Ackmode = ackmode.ToString();
            Count = count;
            Encoding = "auto";
        }

        public IReadOnlyDictionary<string, string> ToQueryParameters()
        {
            return new Dictionary<string, string>
            {
                {nameof(Count), Count.ToString()},
                {nameof(Ackmode), Ackmode},
                {nameof(Encoding), Encoding}
            };
        }
    }
}
