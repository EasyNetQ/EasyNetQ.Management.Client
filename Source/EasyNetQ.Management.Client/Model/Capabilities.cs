using System;
using System.Collections.Generic;
using EasyNetQ.Management.Client.Dynamic;

namespace EasyNetQ.Management.Client.Model
{
    public class Capabilities : PropertyExpando
    {
        public Capabilities(IDictionary<string, object> properties) : base(properties)
        {
        }

        public bool BasicNack => GetPropertyOrDefault<bool>("BasicNack");
        public bool PublisherConfirms => GetPropertyOrDefault<bool>("PublisherConfirms");
        public bool ConsumerCancelNotify => GetPropertyOrDefault<bool>("ConsumerCancelNotify");
        public bool ExchangeExchangeBindings => GetPropertyOrDefault<bool>("ExchangeExchangeBindings");
    }
}
