using System;
using System.Collections.Generic;
using System.Reflection;

namespace DomainEventDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            DomainEventHandlerRegistry.BuildEventHandlerCache(Assembly.GetExecutingAssembly());
            DomainEventsLocator.Register<IDomainEventHandlerRegistry>(() => new DomainEventHandlerRegistry());

            var domainEvents = new List<IDomainEvent>
            {
                new OrderPlaced(), new OrderProcessed(), new OrderDispatched(), new OrderCancelled()
            };

            DomainEvents.RegisterCallbackForUnitTesting<OrderPlaced>((op) =>
            {
                Console.WriteLine("Unit testing {0}", op.GetType());
            });

            DomainEvents.Raise(new OrderPlaced());
            DomainEvents.Raise(new OrderProcessed());
            DomainEvents.Raise(new OrderDispatched());
            DomainEvents.Raise(new OrderCancelled());

            foreach (var domainEvent in domainEvents)
            {
               // Dispatch(domainEvent);
            }
        }

        public static void Dispatch<T>(T domainEvent) where T : IDomainEvent
        {
            DomainEvents.Raise(domainEvent);
        }
    }
}
