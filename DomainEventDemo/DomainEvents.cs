using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace DomainEventDemo
{
    public class DomainEvents
    {
        private static readonly DomainEvents Instance = new DomainEvents(DomainEventsLocator.Resolve<IDomainEventHandlerRegistry>);
        private readonly Func<IDomainEventHandlerRegistry> _handlerRegistry;

        [ThreadStatic] 
        private static List<Delegate> _actions;

        private DomainEvents(Func<IDomainEventHandlerRegistry> handlerRegistry)
        {
            _handlerRegistry = handlerRegistry;
        }

        public static void RegisterCallbackForUnitTesting<T>(Action<T> callback) where T : IDomainEvent
        {
            if (_actions == null)
            _actions = new List<Delegate>();
   
            _actions.Add(callback);
        }

        public static void ClearCallbackUsedForUnitTesting()
        {
            _actions = null;
        }

        public static void Raise<T>(T domainEvent) where T : IDomainEvent
        {
            Instance.InstanceRaise(domainEvent);
            RaiseEventsForUnitTesting(domainEvent);
        }

        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        private void InstanceRaise<T>(T domainEvent) where T : IDomainEvent
        {
            var registry = _handlerRegistry();
            if (registry != null)
            {
                var eventHandlers = registry.GetEventHandlers(domainEvent);
                if (eventHandlers.Any())
                {
                    eventHandlers.ForEach(eh => eh.Handle(domainEvent));
                }
                else
                {
                    Log.Error("No domain event handler for {0}", typeof (T));
                }
            }
            else
            {
                Log.Warn("Domain event registry not found");
            }
        }

        private static void RaiseEventsForUnitTesting<T>(T domainEvent) where T : IDomainEvent
        {
            if (_actions != null)
            {
                foreach (var action in _actions)
                    if (action is Action<T>)
                        ((Action<T>) action)(domainEvent);
            }
        }
    }
}