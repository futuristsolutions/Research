using System.Linq;

namespace DomainEventDemo
{
    public class DomainEvents
    {
        private static readonly DomainEvents Instance = new DomainEvents(new DomainEventHandlerContainer());

        private readonly IDomainEventHandlerContainer _container;

        private DomainEvents(IDomainEventHandlerContainer container)
        {
            _container = container;
        }

        public static void Raise<T>(T domainEvent) where T : IDomainEvent
        {
            Instance.InstanceRaise(domainEvent);
        }

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
        }
    }
}