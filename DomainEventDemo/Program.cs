namespace DomainEventDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceLocator.AddService<IDomainEventHandlerContainer>(()=>new DomainEventHandlerContainer());

            DomainEvents.Raise(new OrderPlaced());
            DomainEvents.Raise(new OrderProcessed());
            DomainEvents.Raise(new OrderDispatched());
            DomainEvents.Raise(new OrderCancelled());
        }
    }
}
