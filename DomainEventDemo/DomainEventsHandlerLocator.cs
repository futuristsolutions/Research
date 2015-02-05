using System;
using System.Collections.Generic;

namespace DomainEventDemo
{
    public static class DomainEventsHandlerLocator
    {
        static DomainEventsHandlerLocator()
        {
            DomainEventHandlerInstances = new Dictionary<Type, Func<IDomainEventHandler>>
            {
                {typeof(NotificationEmailDomainEventHandler), ()=> new NotificationEmailDomainEventHandler()},
                {typeof(NotificationSmsDomainEventHandler), ()=> new NotificationSmsDomainEventHandler()}
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