using System;
using System.Collections.Concurrent;

namespace DomainEventDemo
{
    public class Order
    {
        public Order()
        {
            _events = new ConcurrentQueue<Action>();
        }

        public void PlaceOrder()
        {
            AddDomainEvent(new OrderPlaced());
        }
        
        public void ProcessOrder()
        {
            AddDomainEvent(new OrderProcessed());
        }
        
        public void DispatchOrder()
        {
            AddDomainEvent(new OrderDispatched());
        }
        
        public void CancelOrder()
        {
            AddDomainEvent(new OrderCancelled());
        }

        public void Confirm()
        {
            Action dispatch;
            while (_events.TryDequeue(out dispatch))
            {
                dispatch();
            }
        }

        protected void AddDomainEvent<T>(T domainEvent) where T : IDomainEvent
        {
            _events.Enqueue(() => DomainEvents.Raise(domainEvent));
        }

        private readonly ConcurrentQueue<Action> _events;
    }
}
