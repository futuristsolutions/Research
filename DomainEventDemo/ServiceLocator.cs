using System;
using System.Collections.Generic;

namespace DomainEventDemo
{
    public abstract class ServiceLocator
    {
        public T GetService<T>()
        {
            return (T)GetService(typeof(T));
        }

        public object GetService(Type serviceType)
        {
            Func<object> factory;

            if (!TypeFactories.TryGetValue(serviceType, out factory))
                return null;

            return factory();
        }

        public void AddService<T>(Func<T> instanceBuilder)
        {
            TypeFactories.Add(typeof(T), instanceBuilder as Func<object>);
        }

        /// The collection of factory functions.
        protected Dictionary<Type, Func<object>> TypeFactories = new Dictionary<Type, Func<object>>();
    }

    public class DomainDependencyResolver : ServiceLocator
    {
        private DomainDependencyResolver()
        {
            
        }

        public static T Resolve<T>()
        {
            return Instance.GetService<T>();
        }

        private static DomainDependencyResolver Instance = new DomainDependencyResolver();
    }
}
