using System;
using System.Collections.Generic;
using System.Linq;
using Contact.Monitoring.Web.Controllers;
using Contact.Monitoring.Web.Repository;
using Contact.Monitoring.Web.ViewModel;

namespace Contact.Monitoring.Web.Services
{
    public class OverviewService
    {
        public OverviewViewModel GetOverview()
        {
            var diskThresholdViolation = GetDiskFreeSpaceBelowThreshold(MinDiskFreeSpaceThresholdPercentage);
            var servicesNotRunning = GetServicesNotRunning();
            var lastContactedAfter = DateTime.UtcNow.AddMinutes(-ServerLastContactMinutes);
            var serversNotContactable = GetServerNotContactable(lastContactedAfter);
            var last1Hour = DateTime.UtcNow.AddHours(-1);
            var memoryThresholdViolations = GetLastCounterValueViolatesThreshold("available mbytes",
                         (p) => (double)p.CounterValue < MinAvailableMemoryThreshold, last1Hour);
            var cpuThresholdViolations = GetLastCounterValueViolatesThreshold("% processor time",
                        (p) => (double)p.CounterValue > MaxCpuUsageThreshold, last1Hour);
           
            var listenerSummary = GetSummary("Listener", "Listener", "_all", "requests/second", "requests served");
            var schedulerSummary = GetSummary("Push","Scheduler","_all", "requests/second", "requests served");
            var registrationSummary = GetSummary("Registration", "WebApi", "manage push addresses for a contact", "requests/second", "requests served");
            var preferenceSummary = GetSummary("Preference", "WebApi", "manage all topic channel preferences for a contact", "requests/second", "requests served");
           
            return new OverviewViewModel
            {
                Servers = GetServerStatus(diskThresholdViolation.Any(), serversNotContactable.Any(), servicesNotRunning.Any()),
                SystemDiskSpaceViolations = diskThresholdViolation,
                ServersNotContactable = serversNotContactable,
                ServicesNotRunning = servicesNotRunning,
                Health = GetHealthStatus(memoryThresholdViolations.Any(), cpuThresholdViolations.Any()),
                MemoryThresholdViolations = memoryThresholdViolations,
                CpuThresholdViolations = cpuThresholdViolations,
                ServiceOverview = new List<ServiceOverviewViewModel>()
                {
                    listenerSummary,schedulerSummary,registrationSummary,preferenceSummary
                }
            };
        }

        private List<ServiceStatuViewModel> GetServicesNotRunning()
        {
            var systemDataProvider = new SystemDataProvider();
            return systemDataProvider.GetAllServiceStatus()
                .Where(s => s.Status != "Running")
                .Select(s => new
                    ServiceStatuViewModel
                {
                    Instance = s.Instance,
                    LastUpdatedDateTime = s.LastUpdatedDateTime.ToString(DataTimeFormatString),
                    MachineName = s.MachineName,
                    Service = s.Service,
                    Status = s.Status
                })
                .ToList();
        }

        private ServiceOverviewViewModel GetSummary(string displayName, string service, string instance, string throughputCounter, string totalCounter)
        {
            var last1Hour = DateTime.UtcNow.AddHours(-1);
            var performanceDataProvider = new PerformanceDataProvider();
            var requestsPerSecond = performanceDataProvider.GetCounterValues(instance, throughputCounter, service, last1Hour);
            var requestsPerSecondPerServer = requestsPerSecond.GroupBy(t => new { t.MachineName }, (key, group) =>
                         new
                         {
                             key.MachineName,
                             CounterValue = group.Max(g => g.CounterValue),
                             TimeStamp = group.Max(g => g.Timestamp)
                         }).ToList();
            var totalRequests = performanceDataProvider.GetCounterValues(instance, totalCounter, service, last1Hour);
            var totalRequestsPerServer = totalRequests.GroupBy(t => new {t.MachineName}, (key,group) =>
                         new
                         {
                            key.MachineName,
                            CounterValue = group.Max(g => g.CounterValue),
                            TimeStamp = group.Max(g => g.Timestamp)
                         }).ToList();
            var maxValue = requestsPerSecondPerServer.Any() ? requestsPerSecond.Max(l => l.CounterValue) : 0.0m;
            var avgValue = requestsPerSecondPerServer.Any() ? requestsPerSecond.Average(l => l.CounterValue) : 0.0m;
            var lastTimeItRan = requestsPerSecondPerServer.Any()
                ? requestsPerSecondPerServer.Max(t => t.TimeStamp) : new DateTime();
            var lastSuccesfulRequest = totalRequestsPerServer.Any() 
                ? totalRequestsPerServer.Sum(t => t.CounterValue) : 0.0m;
            return new ServiceOverviewViewModel
            {
                Category = displayName,
                LastTimeItRan = lastTimeItRan.ToString(DataTimeFormatString),
                ThroughputMax = maxValue.ToString("N2"),
                ThroughputAverage = avgValue.ToString("N2"),
                Total = lastSuccesfulRequest.ToString("N2")
            };
        }

        private StatusViewModel GetHealthStatus(bool isMemoryThresholdViolated, bool isCpuThresholdViolated)
        {
             var statusMessage = new List<string>
            {
                isMemoryThresholdViolated ? string.Format("Some of the server(s) have below threshold available memory ( < {0} )",MinAvailableMemoryThreshold): string.Empty,
                isCpuThresholdViolated ? string.Format("Some of the server(s) above threshold of CPU usage ( > {0} )",MaxCpuUsageThreshold): string.Empty
            };
            return new StatusViewModel
            {
                Status = !isMemoryThresholdViolated && !isCpuThresholdViolated,
                Description = string.Join(",", statusMessage.Where(s => s.Length > 0))
            };
        }

        private StatusViewModel GetServerStatus(bool isDiskBelowThreshold, bool isServersNotContactable, bool isServiceNotRunning)
        {
            var serversStatusMessage = new List<string>
            {
                isDiskBelowThreshold ? string.Format("Some of the server(s) have below threshold free disk space ( < {0} percentage free )",MinDiskFreeSpaceThresholdPercentage): string.Empty,
                isServersNotContactable ? string.Format("Some of the server(s) not contactable ( in the last {0} minutes )",ServerLastContactMinutes): string.Empty,
                isServiceNotRunning ? string.Format("Some of services not running"): string.Empty
            };
            var serverStatus = new StatusViewModel
            {
                Status = !isDiskBelowThreshold && !isServersNotContactable,
                Description = string.Join(",", serversStatusMessage.Where(s => s.Length > 0))
            };
            return serverStatus;
        }

        private List<SystemUpTimeViewModel> GetServerNotContactable(DateTime contactedAfterDateTime)
        {
            var systemDataProvider = new SystemDataProvider();
            var systemUpTimes = systemDataProvider.GetAllSystemUpTimes();
            var result = (from s in systemUpTimes
                          where s.LastUpdatedDateTime < contactedAfterDateTime
                          select s)
                          .ToList()
                          .Select(s => new SystemUpTimeViewModel
                          {
                              LastBootUpTime = s.LastBootUpTime.ToString(DataTimeFormatString),
                              MachineName = s.MachineName,
                              Service = s.Service,
                              LastUpdatedDateTime = s.LastUpdatedDateTime.ToString(DataTimeFormatString)
                          })
                          .ToList();
            return result;
        }


        private List<SystemDiskSpaceViewModel> GetDiskFreeSpaceBelowThreshold(double freeSpaceThreshold)
        {
            var systemDataProvider = new SystemDataProvider();
            var diskUsages = systemDataProvider.GetAllSystemDiskUsage();

            var result = (from d in diskUsages
                          where (((double)d.FreeSpace / (double)d.Size) * 100.0) < freeSpaceThreshold
                          select d)
                          .ToList()
                          .Select(s=>new SystemDiskSpaceViewModel
                          {
                              Service = s.Service,
                              MachineName = s.MachineName,
                              Drive = s.Drive,
                              FreeSpace = s.FreeSpace,
                              LastUpdatedDateTime = s.LastUpdatedDateTime.ToString(DataTimeFormatString),
                              Size = s.Size
                          })
                          .ToList();
            return result;
        }
        
        private List<LastPerformanceCounterData> GetLastCounterValueViolatesThreshold(string counter
                            , Func<LastPerformanceCounterData, bool> validator, DateTime dateTimeAfter)
        {
            var monitoringRepository = new PerformanceDataProvider();
            var result = monitoringRepository.GetMaxCounterValues(counter, dateTimeAfter)
                .Where(validator)
                .ToList();
            return result;
        }

        private const double MinDiskFreeSpaceThresholdPercentage = 20.00;
        private const double MinAvailableMemoryThreshold = 2000.00;
        private const double MaxCpuUsageThreshold = 90.00;
        private const int ServerLastContactMinutes = 60;
        private const string DataTimeFormatString = "yyyy-MM-dd HH:mm:ss";
    }
}