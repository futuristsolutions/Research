using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Antlr.Runtime;
using Contact.Monitoring.Web.Models;
using Contact.Monitoring.Web.Repository;
using Contact.Monitoring.Web.ViewModel;
using Microsoft.Ajax.Utilities;

namespace Contact.Monitoring.Web.Services
{
    public class OverviewService
    {
        public OverviewViewModel GetOverview()
        {
            var listenerSummary = GetSummary("Listener", "requests/second", "successful requests");
            var schedulerSummary = GetSummary("Scheduler", "requests/second", "successful requests");
            return new OverviewViewModel
            {
                Servers = GetServerStatus(),
                Health = GetHealthStatus(),
                ServiceOverview = new List<ServiceOverviewViewModel>()
                {
                    listenerSummary,schedulerSummary
                }
            };
        }

        private ServiceOverviewViewModel GetSummary(string category, string throughputCounter, string totalCounter)
        {
            var monitoringRepository = new MonitoringRepository();
            var listenerRequestPerSecond = monitoringRepository.GetCounterValues(throughputCounter, category, DateTime.UtcNow.AddHours(-1));
            var listenerSuccesfulRequests = monitoringRepository.GetCounterValues(totalCounter, category, DateTime.UtcNow.AddHours(-1));
            var maxValue = listenerRequestPerSecond.Max(l => l.CounterValue);
            var avgValue = listenerRequestPerSecond.Average(l => l.CounterValue);
            var lastEntry = listenerRequestPerSecond.OrderBy(l => l.Id).Take(1).FirstOrDefault();
            var lastSuccesfulRequest = listenerSuccesfulRequests.OrderBy(l => l.Id).Take(1).FirstOrDefault();
            return new ServiceOverviewViewModel
            {
                Category = category,
                LastTimeItRan = lastEntry.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"),
                ThroughputMax = maxValue.ToString(),
                ThroughputAverage = avgValue.ToString(),
                Total = lastSuccesfulRequest.CounterValue.ToString()
            };
        }

        private ServiceOverviewViewModel GetListenerSummary()
        {
            var monitoringRepository = new MonitoringRepository();
            var listenerRequestPerSecond = monitoringRepository.GetCounterValues("requests/second", "Listener", DateTime.UtcNow.AddHours(-1));
            var listenerSuccesfulRequests = monitoringRepository.GetCounterValues("successful requests", "Listener", DateTime.UtcNow.AddHours(-1));
            var maxValue = listenerRequestPerSecond.Max(l => l.CounterValue);
            var avgValue = listenerRequestPerSecond.Average(l => l.CounterValue);
            var lastEntry = listenerRequestPerSecond.OrderBy(l => l.Id).Take(1).FirstOrDefault();
            var lastSuccesfulRequest = listenerSuccesfulRequests.OrderBy(l => l.Id).Take(1).FirstOrDefault();
            return new ServiceOverviewViewModel
            {
                Category = "Listener",
                LastTimeItRan = lastEntry.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"),
                ThroughputMax = maxValue.ToString(),
                ThroughputAverage = avgValue.ToString(),
                Total = lastSuccesfulRequest.CounterValue.ToString()
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
            var monitoringRepository = new MonitoringRepository();
            var systemUpTimes = monitoringRepository.GetAllSystemUpTimes();
            var result =  (from s in systemUpTimes
                where s.LastUpdatedDateTime > contactedAfterDateTime
                select s).ToList();
            return result.Any();
        }

        private bool IsDiskFreeSpaceBelowThreshold(double freeSpaceThreshold)
        {
            var monitoringRepository = new MonitoringRepository();
            var diskUsages = monitoringRepository.GetAllSystemDiskUsage();

            var result = (from d in diskUsages
                where (((double) d.FreeSpace/(double) d.Size)*100.0) < freeSpaceThreshold 
                select d).ToList();

            return result.Any();
        }
        
        private bool ValidateCounterValueThreshold(string counter, Func<PerformanceCounterData, bool> validator)
        {
            var monitoringRepository = new MonitoringRepository();
            var result = monitoringRepository.GetLastCounterValues(counter)
                .Where(validator);
            return result.Any();
        }

        private const double MinDiskFreeSpaceThresholdPercentage = 20.00;
        private const int ServerLastContactMinutes = 30;
    }
}