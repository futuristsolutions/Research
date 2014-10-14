using System;
using System.Collections.Generic;
using System.Linq;
using Contact.Monitoring.Web.Models;

namespace Contact.Monitoring.Web.Repository
{
    public class MonitoringRepository
    {
        public List<SystemUpTime> GetAllSystemUpTimes()
        {
            using (var context = new MonitoringContext())
            {
                return context.SystemUpTimes
                    .ToList();
            }
        }

        public List<PerformanceCounterData> GetPeakCounterValueByService(string service, string counter)
        {
            using (var context = new MonitoringContext())
            {
                return context.PerformanceCounterDatas
                    .Where(p => p.Service.Equals(service) && p.Counter.Equals(counter))
                    .GroupBy(g => new {g.MachineName, g.Service},
                        (key, group) => new
                        {
                            key.Service,
                            Timestamp = group.Max(p => p.Timestamp),
                            key.MachineName,
                            Value = group.Max(p => p.CounterValue)
                        })
                    .ToList()
                    .Select(r => new PerformanceCounterData
                    {
                        Service = r.Service,
                        Timestamp = r.Timestamp,
                        MachineName = r.MachineName,
                        Counter = counter,
                        CounterValue = r.Value
                    }).ToList();
            }
        }

        public List<PerformanceCounterData> GetLastCounterValueByService(string service, string counter)
        {
            using (var context = new MonitoringContext())
            {
                return context.PerformanceCounterDatas
                    .Where(p => p.Service.Equals(service) && p.Counter.Equals(counter))
                    .GroupBy(g => new { g.MachineName, g.Service },
                        (key, group) => new
                        {
                            key.Service,
                            Timestamp = group.Max(p => p.Timestamp),
                            key.MachineName
                        })
                    .ToList()
                    .Select(r => new PerformanceCounterData
                    {
                        Service = r.Service,
                        Timestamp = r.Timestamp,
                        MachineName = r.MachineName,
                        Counter = counter
                    }).ToList();
            }
        }

        public List<PerformanceCounterData> GetCounterValuesByMachine(string machine, string counter)
        {
            using (var context = new MonitoringContext())
            {
                return context.PerformanceCounterDatas
                    .Where(c => c.Counter == counter && c.MachineName == machine)
                    .ToList();
            }
        }

        public List<PerformanceCounterData> GetAllCounterValues(DateTime recordedAfterDateTime)
        {
            using (var context = new MonitoringContext())
            {
                return context.PerformanceCounterDatas
                    .Where(p => p.Timestamp > recordedAfterDateTime)
                    .OrderByDescending(o => o.Id)
                    .ToList();
            }
        }

        public List<PerformanceCounterData> GetCounterValues(IEnumerable<string> counter, DateTime recordedAfterDateTime)
        {
            using (var context = new MonitoringContext())
            {
                return context.PerformanceCounterDatas
                    .Where(c => counter.Contains(c.Counter) && c.Timestamp > recordedAfterDateTime)
                    .ToList();
            }
        }

        public List<PerformanceCounterData> GetCounterValues(string counter, string service, DateTime recordedAfterDateTime)
        {
            using (var context = new MonitoringContext())
            {
                return context.PerformanceCounterDatas
                    .Where(c => counter.Contains(c.Counter) && c.Service == service &&  c.Timestamp > recordedAfterDateTime)
                    .ToList();
            }
        }

        public PerformanceCounterData GetLastCounterValue(string service, string counter)
        {
            using (var context = new MonitoringContext())
            {
                return context.PerformanceCounterDatas
                    .OrderBy(o => o.Id)
                    .Where(c => counter == c.Counter && c.Service == service)
                    .Take(1)
                    .First();
            }
        }

        public List<PerformanceCounterData> GetLastCounterValues(string counter)
        {
            using (var context = new MonitoringContext())
            {
                var result = (from counters in context.PerformanceCounterDatas
                        where counters.Counter == counter
                        group counters by new {counters.Service, counters.MachineName, counters.Counter}
                        into countersGroup
                        select new
                        {
                            countersGroup.Key.Counter, 
                            countersGroup.Key.MachineName, 
                            countersGroup.Key.Service,
                            CounterValue = countersGroup.Max(c => c.CounterValue)
                        })
                        .ToList()
                        .Select(s => new PerformanceCounterData
                        {
                            Counter = s.Counter,
                            MachineName = s.MachineName,
                            Service = s.Service,
                            CounterValue = s.CounterValue
                        })
                        .ToList();
                return result;
            }
        }

        internal List<ServiceStatu> GetAllServiceStatus()
        {
            using (var context = new MonitoringContext())
            {
                return context.ServiceStatus
                    .ToList();
            }
        }

        internal List<SystemDiskSpace> GetAllSystemDiskUsage()
        {
            using (var context = new MonitoringContext())
            {
                return context.SystemDiskSpaces
                    .ToList();
            }
        }

        internal List<SchedulerQueue> GetLastSchedulerQueueCount()
        {
            using (var context = new MonitoringContext())
            {
                return context.SchedulerQueues
                    .OrderByDescending(o => o.Id)
                    .Take(1)
                    .ToList();
            }
        }
    }
}