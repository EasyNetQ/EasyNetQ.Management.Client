using System;
using System.Collections.Generic;
using EasyNetQ.Management.Client.Dynamic;

namespace EasyNetQ.Management.Client.Model
{
    public class Capabilities : PropertyExpando
    {
        public Capabilities(IDictionary<string, object> properties) : base (properties)
        {            
        }

        public bool BasicNack => GetPropertyOrDefault<Boolean>("BasicNack");
        public bool PublisherConfirms => GetPropertyOrDefault<Boolean>("PublisherConfirms");
        public bool ConsumerCancelNotify => GetPropertyOrDefault<Boolean>("ConsumerCancelNotify");
        public bool ExchangeExchangeBindings => GetPropertyOrDefault<Boolean>("ExchangeExchangeBindings");
    }
}