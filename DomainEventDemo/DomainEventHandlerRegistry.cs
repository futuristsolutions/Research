using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DomainEventDemo
{
    public class DomainEventHandlerRegistry : IDomainEventHandlerRegistry
    {
        private static Dictionary<Type, List<IDomainEventHandler>> _eventHandlerCache = new Dictionary<Type, List<IDomainEventHandler>>();
        
        public IEnumerable<IDomainEventHandler<T>> GetEventHandlers<T>(T domainEvent)
            where T : IDomainEvent
        {
            if (!_eventHandlerCache.ContainsKey(typeof(T)))
            {
                return Enumerable.Empty<IDomainEventHandler<T>>();
            }

            return _eventHandlerCache[typeof(T)].Cast<IDomainEventHandler<T>>().ToArray();
        }

        public static void BuildEventHandlerCache(Assembly assembly)
        {
            var eventHandlerCache = GetAllDomainEventHandlers(typeof(IDomainEvent).Assembly);
            DisplayEventHandlerInfo(eventHandlerCache);
            _eventHandlerCache = eventHandlerCache;
        }


        private static Dictionary<Type, List<IDomainEventHandler>> GetAllDomainEventHandlers(Assembly assembly)
        {
            var eventHandlerCache = new Dictionary<Type, List<IDomainEventHandler>>();
            var openDomainEventHandlerType = typeof (IDomainEventHandler<>);
               (
                from implementedType in assembly.GetTypes()
                from baseInterface in implementedType.GetInterfaces()
                let baseType = implementedType.BaseType
                where
                    (baseType != null && baseType.IsGenericType &&
                     openDomainEventHandlerType.IsAssignableFrom(baseType.GetGenericTypeDefinition())) ||
                    (baseInterface.IsGenericType &&
                     openDomainEventHandlerType.IsAssignableFrom(baseInterface.GetGenericTypeDefinition()))
                select new
                {
                    HandledDomainEventType = baseInterface.GenericTypeArguments.First(),
                    DomainEventHandlerType = implementedType
                }).ForEach(e =>
                {
                    var handler = DomainEventsHandlerLocator.Resolve(e.DomainEventHandlerType);
                    if (handler != null)
                    {
                        if (!eventHandlerCache.ContainsKey(e.HandledDomainEventType))
                        {
                            eventHandlerCache.Add(e.HandledDomainEventType, new List<IDomainEventHandler>());
                        }
                        eventHandlerCache[e.HandledDomainEventType].Add(handler);
                    }
                });

            return eventHandlerCache;
        }
        private static void DisplayEventHandlerInfo(Dictionary<Type, List<IDomainEventHandler>> eventHandlerCache)
        {
            if (!eventHandlerCache.Any())
            {
                Log.Warn("No EventHandlers available in the system");
            }
            else
            {
                eventHandlerCache.ForEach(eh =>
                {
                    Log.Info("{0} event has {1} handler(s) [{2}]", eh.Key, eh.Value.Count, string.Join(",",eh.Value.Select(s => s.GetType()).AsEnumerable()));
                });
            }
        }
    }

    public interface IDomainEventHandlerRegistry
    {
        IEnumerable<IDomainEventHandler<T>> GetEventHandlers<T>(T domainEvent) where T : IDomainEvent;        
    }
}
