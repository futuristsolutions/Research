namespace DomainEventDemo
{
    public interface IDomainEventHandler
    {

    }
    public interface IDomainEventHandler<in T> : IDomainEventHandler  where T : IDomainEvent
    {
        void Handle(T domainEvent);
    }
}