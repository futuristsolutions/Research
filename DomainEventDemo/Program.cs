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


            DomainEvents.RegisterCallbackForUnitTesting<OrderPlaced>((op) =>
            {
                Console.WriteLine("Unit testing {0}", op.GetType());
            });

            Order order = new Order();
            order.PlaceOrder();
            order.ProcessOrder();
            order.DispatchOrder();
            order.CancelOrder();
            order.Confirm();
        }

        public static void Dispatch<T>(T domainEvent) where T : IDomainEvent
        {
            DomainEvents.Raise(domainEvent);
        }
    }
}
