namespace DomainEventDemo
{
    public interface IDomainEvent
    {
        void Process(IDomainEventEngine engine);
    }

    public interface IDomainEventEngine
    {
        void Process(OrderPlaced orderPlaced);
        void Process(OrderProcessed orderProcessed);
        void Process(OrderCancelled orderCancelled);
        void Process(OrderDispatched orderDispatched);
    }
}