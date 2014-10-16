using System.Collections.Generic;
using Contact.Monitoring.Web.Controllers;
using Contact.Monitoring.Web.Repository;
using Contact.Monitoring.Web.Services;

namespace Contact.Monitoring.Web.ViewModel
{
    public class OverviewViewModel
    {
        public StatusViewModel Servers { get; set; }

        public StatusViewModel Health { get; set; }

        public List<ServiceOverviewViewModel> ServiceOverview { get; set; }
        public List<SystemDiskSpaceViewModel> SystemDiskSpaceViolations { get; set; }
        public List<SystemUpTimeViewModel> ServersNotContactable { get; set; }
        public List<LastPerformanceCounterData> MemoryThresholdViolations { get; set; }
        public List<LastPerformanceCounterData> CpuThresholdViolations { get; set; }
        public List<ServiceStatuViewModel> ServicesNotRunning { get; set; }
    }

    public class StatusViewModel
    {
        public bool Status { get; set; }

        public string Description { get; set; }
    }

    public class ServiceOverviewViewModel
    {
        public string Category { get; set; }

        public string ThroughputAverage { get; set; }

        public string ThroughputMax { get; set; }

        public string LastTimeItRan { get; set; }

        public string Total { get; set; }
    }
}