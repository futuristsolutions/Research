using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Contact.Monitoring.ViewModel;

namespace Contact.Monitoring.Services
{
    public class OverviewService
    {
        public OverviewViewModel GetOverview()
        {
            var diskThresholdViolation = GetDiskFreeSpaceBelowThreshold(MinDiskFreeSpaceThresholdPercentage);
            var servicesNotRunning = GetServicesNotRunning();
            var lastContactedAfter = ServerLastContactedAfterTime();
            var serversNotContactable = GetServerNotContactable(lastContactedAfter);
            var pendingScheduleMessages = GetPendingScheduleMessages();
            var lastAddressBookUpdate = GetLastAddressBookUpdate();
            var lookbackTime = LookBackTime();
            var memoryThresholdViolations = GetLastCounterValueViolatesThreshold("available mbytes",
                         (p) => (double)p.CounterValue < MinAvailableMemoryThreshold, lookbackTime);
            var cpuThresholdViolations = GetLastCounterValueViolatesThreshold("% processor time",
                        (p) => (double)p.CounterValue > MaxCpuUsageThreshold, lookbackTime);
           
            var listenerSummary = GetSummary("Listener", "Listener", "_all", "requests/second", "requests served");
            var schedulerSummary = GetSummary("Push","Scheduler","_all", "requests/second", "requests served");
            var registrationSummary = GetSummary("Registration", "WebApi", "manage push addresses for a contact", "requests/second", "requests served");
            var preferenceSummary = GetSummary("Preference", "WebApi", "manage all topic channel preferences for a contact", "requests/second", "requests served");
           
            return new OverviewViewModel
            {
                LastAddressBookUpdate = lastAddressBookUpdate,
                Servers = GetServerStatus(diskThresholdViolation.Any(), serversNotContactable.Any(), servicesNotRunning.Any()),
                SystemDiskSpaceViolations = diskThresholdViolation,
                ServersNotContactable = serversNotContactable,
                ServicesNotRunning = servicesNotRunning,
                PendingScheduleMessages = pendingScheduleMessages,
                Health = GetHealthStatus(memoryThresholdViolations.Any(), cpuThresholdViolations.Any(), pendingScheduleMessages.Any()),
                MemoryThresholdViolations = memoryThresholdViolations,
                CpuThresholdViolations = cpuThresholdViolations,
                ServiceOverview = new List<ServiceOverviewViewModel>()
                {
                    registrationSummary,preferenceSummary,listenerSummary,schedulerSummary
                }
            };
        }

        private AddressBookViewModel GetLastAddressBookUpdate()
        {
            var systemDataProvider = new SystemDataProvider();
            return systemDataProvider.GetLastAddressBookUpdate()
                    .Select(s => new AddressBookViewModel
                    {
                        Count = s.Count,
                        LastUpdatedDateTime = s.LastUpdatedDateTime.ToString(DataTimeFormatString)
                    })
                    .FirstOrDefault();
        }

        private static DateTime LookBackTime()
        {
            var lookbackTime = DateTime.UtcNow.AddHours(-LookBackTimeHours);
            return lookbackTime;
        }

        private static DateTime ServerLastContactedAfterTime()
        {
            var lookbackTime = DateTime.UtcNow.AddHours(-ServerLastContactMinutes);
            return lookbackTime;
        }

        private List<SchedulerQueuePendingViewModel> GetPendingScheduleMessages()
        {
            var systemDataProvider = new SystemDataProvider();
            return systemDataProvider.GetSchedulerQueuePending()
                .Select(s => new SchedulerQueuePendingViewModel
                {
                    LastUpdatedDateTime = s.LastUpdatedDateTime.ToString(DataTimeFormatString),
                    PendingFrom = s.PendingFrom.ToString(DataTimeFormatString),
                    QueueCount = s.QueueCount
                }).ToList();
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
            var lookBackTime = LookBackTime();
            var performanceDataProvider = new PerformanceDataProvider();
            var thoughputValues = performanceDataProvider.GetCounterValues(instance, throughputCounter, service, lookBackTime);
            var thoughputValuesPerServer = thoughputValues.GroupBy(t => new { t.MachineName }, (key, group) =>
                         new
                         {
                             key.MachineName,
                             CounterValue = group.Max(g => g.CounterValue),
                             TimeStamp = group.Max(g => g.Timestamp)
                         }).ToList();
            var totalRequests = performanceDataProvider.GetCounterValues(instance, totalCounter, service, lookBackTime);
            var totalRequestsPerServer = totalRequests.GroupBy(t => new {t.MachineName}, (key,group) =>
                         new
                         {
                            key.MachineName,
                            CounterValue = group.Max(g => g.CounterValue),
                            TimeStamp = group.Max(g => g.Timestamp)
                         }).ToList();
            var maxValue = thoughputValuesPerServer.Any() ? thoughputValues.Max(l => l.CounterValue).ToString("N2") : "NO DATA";
            var avgValue = thoughputValuesPerServer.Any() ? thoughputValues.Average(l => l.CounterValue).ToString("N2") : "NO DATA";

            var sumTotalRequestsPerServer = totalRequestsPerServer.Any()
                ? totalRequestsPerServer.Sum(t => t.CounterValue).ToString("N2") : "NO DATA";

            var lastNonZeroCounterValues = performanceDataProvider.GetCounterValues(instance, throughputCounter, service, 0.0m);
            var lastTimeItRan = lastNonZeroCounterValues.Any()
                ? lastNonZeroCounterValues.First().Timestamp.ToString(DataTimeFormatString)
                : "NO DATA";

            return new ServiceOverviewViewModel
            {
                Category = displayName,
                LastTimeItRan = lastTimeItRan,
                ThroughputMax = maxValue,
                ThroughputAverage = avgValue,
                Total = sumTotalRequestsPerServer
            };
        }

        private StatusViewModel GetHealthStatus(bool isMemoryThresholdViolated, bool isCpuThresholdViolated, bool isAnyPendingScheduleMessages)
        {
             var healthStatusDescription = (new List<string>
            {
                isMemoryThresholdViolated ? string.Format("Some of the server(s) have below threshold available memory ( < {0} Mbytes )",MinAvailableMemoryThreshold): string.Empty,
                isCpuThresholdViolated ? string.Format("Some of the server(s) above threshold of CPU usage ( > {0} % )",MaxCpuUsageThreshold): string.Empty,
                isAnyPendingScheduleMessages ? string.Format("Undelivered messages in scheduler queue"): string.Empty
            }).Where(s => s.Length > 0).ToList();
            return new StatusViewModel
            {
                Status = !healthStatusDescription.Any(),
                Description = healthStatusDescription
            };
        }

        private StatusViewModel GetServerStatus(bool isDiskBelowThreshold, bool isServersNotContactable, bool isServiceNotRunning)
        {
            var serversStatusDescription = (new List<string>
            {
                isDiskBelowThreshold ? string.Format("Some of the server(s) have below threshold free disk space ( < {0} % free )",MinDiskFreeSpaceThresholdPercentage): string.Empty,
                isServersNotContactable ? string.Format("Some of the server(s) not contactable ( in the last {0} minutes )",ServerLastContactMinutes): string.Empty,
                isServiceNotRunning ? string.Format("Some of services not running"): string.Empty
            }).Where(s => s.Length > 0).ToList();
            
            var serverStatus = new StatusViewModel
            {
                Status = !serversStatusDescription.Any(),
                Description = serversStatusDescription
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
                          .Select(Mapper.Map<SystemUpTimeViewModel>)
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
                          .Select(Mapper.Map<SystemDiskSpaceViewModel>)
                          .ToList();
            return result;
        }
        
        private List<MaxPerformanceCounterDataViewModel> GetLastCounterValueViolatesThreshold(string counter
                            , Func<MaxPerformanceCounterData, bool> validator, DateTime dateTimeAfter)
        {
            var monitoringRepository = new PerformanceDataProvider();
            var result = monitoringRepository.GetMaxCounterValues(counter, dateTimeAfter)
                .Where(validator)
                .ToList()
                .Select(Mapper.Map<MaxPerformanceCounterDataViewModel>)
                .ToList();
            return result;
        }

        private const double MinDiskFreeSpaceThresholdPercentage = 20.00;
        private const double MinAvailableMemoryThreshold = 2000.00;
        private const double MaxCpuUsageThreshold = 90.00;
        private const int ServerLastContactMinutes = 60;
        private const string DataTimeFormatString = "yyyy-MM-dd HH:mm:ss";
        private const int LookBackTimeHours = 1;
    }

}