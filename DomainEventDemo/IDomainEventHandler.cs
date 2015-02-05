namespace DomainEventDemo
{
    public interface IDomainEventHandler
    {

    }
    public interface IDomainEventHandler<in T> : IDomainEventHandler 
    {
        void Handle(T domainEvent);
    }
}