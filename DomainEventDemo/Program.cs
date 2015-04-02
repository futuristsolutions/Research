using System;
using System.Collections.Generic;
using System.Reflection;

namespace DomainEventDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            DomainEventEngine engine = new DomainEventEngine();
            engine.Run();
        }

    }

    class DomainEventEngine : IDomainEventEngine
    {
        public void Run()
        {
            DomainEventHandlerRegistry.BuildEventHandlerCache(Assembly.GetExecutingAssembly());
            DomainEventsLocator.Register<IDomainEventHandlerRegistry>(() => new DomainEventHandlerRegistry());
            DomainEvents.RegisterCallbackForUnitTesting<OrderPlaced>(
                (op) => { Console.WriteLine("Unit testing {0}", op.GetType()); });

            Console.WriteLine("");
            Console.WriteLine("****************************************");

            var order = new Order();
            order.PlaceOrder();
            order.ProcessOrder();
            order.DispatchOrder();
            order.CancelOrder();

            var events = new List<IDomainEvent>();
            events.Add(new OrderCancelled());
            events.Add(new OrderProcessed());

            foreach (var domainEvent in events)
            {
                domainEvent.Process(this);
            }

            //order.Confirm();
        }

        public void Process(OrderPlaced orderPlaced)
        {
            Console.WriteLine("Called Process with {0}", orderPlaced.GetType().Name);
        }

        public void Process(OrderProcessed orderProcessed)
        {
            Console.WriteLine("Called Process with {0}", orderProcessed.GetType().Name);
        }

        public void Process(OrderCancelled orderCancelled)
        {
            Console.WriteLine("Called Process with {0}", orderCancelled.GetType().Name);
        }

        public void Process(OrderDispatched orderDispatched)
        {
            Console.WriteLine("Called Process with {0}", orderDispatched.GetType().Name);
        }
    }
}
