namespace DomainEventDemo
{
    public class NotificationEmailDomainEventHandler : IDomainEventHandler<OrderPlaced>
                                                        ,IDomainEventHandler<OrderDispatched>
    {
        public void Handle(OrderPlaced domainEvent)
        {
            Log.Info("Handling {0} by {1}", domainEvent.GetType(), GetType());
        }

        public void Handle(OrderDispatched domainEvent)
        {
            Log.Info("Handling {0} by {1}", domainEvent.GetType(), GetType());
        }
    }

    public class NotificationSmsDomainEventHandler : IDomainEventHandler<OrderPlaced>
                                                        , IDomainEventHandler<OrderCancelled>
    {
        public void Handle(OrderPlaced domainEvent)
        {
            Log.Info("Handling {0} by {1}", domainEvent.GetType(), GetType());
        }

        public void Handle(OrderCancelled domainEvent)
        {
            Log.Info("Handling {0} by {1}", domainEvent.GetType(), GetType());
        }
    }


    public class NotificationEmailDomainEventHandler3 : IDomainEventHandler<OrderProcessed>
    {
        public void Handle(OrderProcessed domainEvent)
        {
            Log.Info("Handling {0} by {1}", domainEvent.GetType(), GetType());
        }
    }
}