using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace DomainEventDemo
{
    public class DomainEvents
    {
        private static readonly DomainEvents Instance = new DomainEvents(ServiceLocator.Resolve<IDomainEventHandlerContainer>());
        private readonly IDomainEventHandlerContainer _container;

        [ThreadStatic] 
        private static List<Delegate> _actions;

        private DomainEvents(IDomainEventHandlerContainer container)
        {
            _container = container;
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
        }

        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        private void InstanceRaise<T>(T domainEvent) where T : IDomainEvent
        {
            var eventHandlers = _container.GetEventHandlers(domainEvent);

            if (eventHandlers.Any())
            {
                eventHandlers.ForEach(eh => eh.Handle(domainEvent));
            }
            else
            {
                Log.Error("No domain event handler for {0}", typeof(T));
            }

            if (_actions != null)
            {
                foreach (var action in _actions)
                    if (action is Action<T>)
                        ((Action<T>)action)(domainEvent);
            }
        }
    }
}