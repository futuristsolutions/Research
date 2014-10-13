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

        internal List<SchedulerQueueCount> GetLastSchedulerQueueCount()
        {
            using (var context = new MonitoringContext())
            {
                return context.SchedulerQueueCounts
                    .OrderByDescending(o => o.Id)
                    .Take(1)
                    .ToList();
            }
        }
    }
}