using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DomainEventDemo
{
    public class DomainEventHandlerContainer : IDomainEventHandlerContainer
    {
        private static readonly Dictionary<Type, List<IDomainEventHandler>> EventHandlerCache;

        static DomainEventHandlerContainer()
        {
            EventHandlerCache = BuildEventHandlerCache();
        }

        public IEnumerable<IDomainEventHandler<T>> GetEventHandlers<T>(T domainEvent)
            where T : IDomainEvent
        {
            if (!EventHandlerCache.ContainsKey(typeof(T)))
            {
                return Enumerable.Empty<IDomainEventHandler<T>>();
            }

            return EventHandlerCache[typeof(T)].Cast<IDomainEventHandler<T>>().ToArray();
        }

        private static Dictionary<Type, List<IDomainEventHandler>> BuildEventHandlerCache()
        {
            var eventHandlerCache = GetAllDomainEventHandlers(typeof(IDomainEvent).Assembly);
            DisplayEventHandlerInfo(eventHandlerCache);
            return eventHandlerCache;
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
                    var handler = DomainEventHandlerResolver.Resolve(e.DomainEventHandlerType);
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

    public interface IDomainEventHandlerContainer
    {
        IEnumerable<IDomainEventHandler<T>> GetEventHandlers<T>(T domainEvent) where T : IDomainEvent;
    }
}
