using System.Collections.Generic;
using System.Linq;
using Contact.Monitoring.Web.Models;

namespace Contact.Monitoring.Web.Services
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