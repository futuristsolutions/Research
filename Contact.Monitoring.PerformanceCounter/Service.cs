using System;
using System.Collections.Generic;
using System.Linq;
using Tesco.Services.Contact.Mechanisms.Logging;
using Tesco.Services.Contact.Mechanisms.Performance;
using Tesco.Services.Contact.Mechanisms.Process;

namespace Contact.Monitoring.PerformanceMonitor
{

    class Service : IService
    {
        private readonly string _category;

        public Service(string category)
        {
            _category = category;
        }

        public void Start()
        {
            Log.Info("Initialize - {0}",_category);
            PerformanceCounterInitializer.Create(_category);
        }

        public void Stop()
        {
            Log.Info("Uninitalize - {0}", _category);
            PerformanceCounterInitializer.Dispose(_category);
        }
    }

    static class PerformanceCounterInitializer
    {
        private static readonly Dictionary<string, Func<IDisposable>> Categories = new Dictionary<string, Func<IDisposable>>
        {
            { PerformanceCounterInfo.ContactServiceListener, () => new GlobalPerformanceCounter(PerformanceCounterInfo.ContactServiceListener) },
            { PerformanceCounterInfo.ContactServiceScheduler, () => new GlobalPerformanceCounter(PerformanceCounterInfo.ContactServiceScheduler) },
            { PerformanceCounterInfo.ContactServiceApi, () => new WebApiPerformanceCounter(PerformanceCounterInfo.ContactServiceApi)}
        };

        internal static void Create(string category)
        {
            if (Categories.ContainsKey(category))
            {
                Categories[category].Invoke();
            }

        }

        internal static void Dispose(string category)
        {
            if (Categories.ContainsKey(category))
            {
                var initializer = Categories[category].Invoke();
                initializer.Dispose();
            } 
        }
    }
}