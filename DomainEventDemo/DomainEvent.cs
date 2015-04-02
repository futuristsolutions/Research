using System;

namespace DomainEventDemo
{
    public class OrderPlaced : IDomainEvent
    {
        public void Process(IDomainEventEngine engine)
        {
            Console.WriteLine("Process in {0}",GetType().Name);
            engine.Process(this);
        }
    }

    public class OrderCancelled : IDomainEvent
    {
        public void Process(IDomainEventEngine engine)
        {
            Console.WriteLine("Process in {0}", GetType().Name);
            engine.Process(this);
        }
    }

    public class OrderDispatched : IDomainEvent
    {
        public void Process(IDomainEventEngine engine)
        {
            Console.WriteLine("Process in {0}", GetType().Name);
            engine.Process(this);
        }
    }

    public class OrderProcessed : IDomainEvent
    {
        public void Process(IDomainEventEngine engine)
        {
            Console.WriteLine("Process in {0}", GetType().Name);
            engine.Process(this);
        }
    }
}