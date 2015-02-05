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

            Console.WriteLine("");
            Console.WriteLine("****************************************");

            var order = new Order();
            order.PlaceOrder();
            order.ProcessOrder();
            order.DispatchOrder();
            order.CancelOrder();
            order.Confirm();
        }
    }
}
