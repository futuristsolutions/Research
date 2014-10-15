using System;
using System.Collections.Generic;
using System.Linq;
using Contact.Monitoring.Web.Models;
using Contact.Monitoring.Web.ViewModel;

namespace Contact.Monitoring.Web.Services
{
    public class OverviewService
    {
        public OverviewViewModel GetOverview()
        {
            var listenerSummary = GetSummary("Listener", "Listener", "_all", "requests/second", "requests served");
            var schedulerSummary = GetSummary("Push","Scheduler","_all", "requests/second", "requests served");
            var registrationSummary = GetSummary("Registration", "WebApi", "manage push addresses for a contact", "requests/second", "requests served");
            var preferenceSummary = GetSummary("Preference", "WebApi", "manage all topic channel preferences for a contact", "requests/second", "requests served");
           
            return new OverviewViewModel
            {
                Servers = GetServerStatus(),
                Health = GetHealthStatus(),
                ServiceOverview = new List<ServiceOverviewViewModel>()
                {
                    listenerSummary,schedulerSummary,registrationSummary,preferenceSummary
                }
            };
        }

        private ServiceOverviewViewModel GetSummary(string displayName, string service, string instance, string throughputCounter, string totalCounter)
        {
            var performanceDataProvider = new PerformanceDataProvider();
            var requestsPerSecond = performanceDataProvider.GetCounterValues(instance, throughputCounter, service, DateTime.UtcNow.AddHours(-1));
            var requestsPerSecondPerServer = requestsPerSecond.GroupBy(t => new { t.MachineName }, (key, group) =>
                         new
                         {
                             key.MachineName,
                             CounterValue = group.Max(g => g.CounterValue),
                             TimeStamp = group.Max(g => g.Timestamp)
                         }).ToList();
            var totalRequests = performanceDataProvider.GetCounterValues(instance, totalCounter, service, DateTime.UtcNow.AddHours(-1));
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
                LastTimeItRan = lastTimeItRan.ToString("yyyy-MM-dd HH:mm:ss"),
                ThroughputMax = maxValue.ToString("N2"),
                ThroughputAverage = avgValue.ToString("N2"),
                Total = lastSuccesfulRequest.ToString("N2")
            };
        }

        private StatusViewModel GetHealthStatus()
        {
            var memoryThresholdViolation = ValidateCounterValueThreshold("available mbytes",
                (p) => p.CounterValue < 5000);
            var cpuThresholdViolation = ValidateCounterValueThreshold("% processor time",
                (p) => p.CounterValue > 90);
             var statusMessage = new List<string>
            {
                memoryThresholdViolation ? "Some of the server(s) have below threshold available memory": string.Empty,
                cpuThresholdViolation ? "Some of the server(s) above threshold of CPU usage": string.Empty
            };
            return new StatusViewModel
            {
                Status = !memoryThresholdViolation && !cpuThresholdViolation,
                Description = string.Join(",", statusMessage.Where(s => s.Length > 0))
            };
        }

        private StatusViewModel GetServerStatus()
        {
            var isDiskBelowThreshold = IsDiskFreeSpaceBelowThreshold(MinDiskFreeSpaceThresholdPercentage);
            var isServersContactable = IsServerContactable(DateTime.UtcNow.AddMinutes(-ServerLastContactMinutes));
            var serversStatusMessage = new List<string>
            {
                isDiskBelowThreshold ? "Some of the server(s) have below threshold free disk space": string.Empty,
                !isServersContactable ? "Some of the server(s) not contactable": string.Empty
            };
            var serverStatus = new StatusViewModel
            {
                Status = !isDiskBelowThreshold && isServersContactable,
                Description = string.Join(",", serversStatusMessage.Where(s => s.Length > 0))
            };
            return serverStatus;
        }

        private bool IsServerContactable(DateTime contactedAfterDateTime)
        {
            var systemDataProvider = new SystemDataProvider();
            var systemUpTimes = systemDataProvider.GetAllSystemUpTimes();
            var result =  (from s in systemUpTimes
                where s.LastUpdatedDateTime > contactedAfterDateTime
                select s).ToList();
            return result.Any();
        }

        private bool IsDiskFreeSpaceBelowThreshold(double freeSpaceThreshold)
        {
            var systemDataProvider = new SystemDataProvider();
            var diskUsages = systemDataProvider.GetAllSystemDiskUsage();

            var result = (from d in diskUsages
                where (((double) d.FreeSpace/(double) d.Size)*100.0) < freeSpaceThreshold 
                select d).ToList();

            return result.Any();
        }
        
        private bool ValidateCounterValueThreshold(string counter, Func<LastPerformanceCounterData, bool> validator)
        {
            var monitoringRepository = new PerformanceDataProvider();
            var result = monitoringRepository.GetLastCounterValues(counter)
                .Where(validator)
                .ToList();
            return result.Any();
        }

        private const double MinDiskFreeSpaceThresholdPercentage = 20.00;
        private const int ServerLastContactMinutes = 30;
    }
}