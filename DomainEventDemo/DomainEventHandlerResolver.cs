using System;
using System.Collections.Generic;

namespace DomainEventDemo
{
    public static class DomainEventHandlerResolver
    {
        static DomainEventHandlerResolver()
        {
            DomainEventHandlerInstances = new Dictionary<Type, Func<IDomainEventHandler>>
            {
                {typeof(NotificationEmailDomainEventHandler), ()=> new NotificationEmailDomainEventHandler()},
                {typeof(NotificationEmailDomainEventHandler2), ()=> new NotificationEmailDomainEventHandler2()}
            };
        }
        public static IDomainEventHandler Resolve(Type domainEventType) 
        {
            if (DomainEventHandlerInstances.ContainsKey(domainEventType))
            {
                return DomainEventHandlerInstances[domainEventType]();
            }
            return null;
        }

        private static readonly IDictionary<Type, Func<IDomainEventHandler>> DomainEventHandlerInstances;
    }
}