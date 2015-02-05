using System;

namespace DomainEventDemo
{
    public class DomainEventsLocator : ServiceLocator
    {
        private DomainEventsLocator()
        {
            
        }

        public static void Register<T>(Func<T> instanceBuilder)
        {
            Instance.AddService(instanceBuilder);
        }

        public static T Resolve<T>()
        {
            return Instance.GetService<T>();
        }

        private static readonly DomainEventsLocator Instance = new DomainEventsLocator();
    }
}