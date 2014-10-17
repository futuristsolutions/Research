using System.Collections.Generic;
using System.Linq;
using Contact.Monitoring.Models;

namespace Contact.Monitoring.Services
{
    public class SystemDataProvider
    {
        public List<SystemUpTime> GetAllSystemUpTimes()
        {
            using (var context = new MonitoringContext())
            {
                return context.SystemUpTimes
                    .ToList();
            }
        }

        public List<ServiceStatu> GetAllServiceStatus()
        {
            using (var context = new MonitoringContext())
            {
                return context.ServiceStatus
                    .ToList();
            }
        }
        
        public List<SystemDiskSpace> GetAllSystemDiskUsage()
        {
            using (var context = new MonitoringContext())
            {
                return context.SystemDiskSpaces
                    .ToList();
            }
        }

        public List<SchedulerQueue> GetLastSchedulerQueueCount()
        {
            using (var context = new MonitoringContext())
            {
                return context.SchedulerQueues
                    .OrderByDescending(o => o.Id)
                    .Take(1)
                    .ToList();
            }
        }
        
        public List<SchedulerQueuePending> GetSchedulerQueuePending()
        {
            using (var context = new MonitoringContext())
            {
                return context.SchedulerQueuePendings
                     .OrderBy(o => o.PendingFrom)
                     .ToList();
            }
        }
    }
}