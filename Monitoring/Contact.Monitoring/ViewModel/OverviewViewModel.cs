using System.Collections.Generic;

namespace Contact.Monitoring.ViewModel
{
    public class OverviewViewModel
    {
        public StatusViewModel Servers { get; set; }

        public StatusViewModel Health { get; set; }

        public AddressBookViewModel LastAddressBookUpdate { get; set; }

        public List<ServiceOverviewViewModel> ServiceOverview { get; set; }
        public List<SystemDiskSpaceViewModel> SystemDiskSpaceViolations { get; set; }
        public List<SystemUpTimeViewModel> ServersNotContactable { get; set; }
        public List<MaxPerformanceCounterDataViewModel> MemoryThresholdViolations { get; set; }
        public List<MaxPerformanceCounterDataViewModel> CpuThresholdViolations { get; set; }
        public List<ServiceStatuViewModel> ServicesNotRunning { get; set; }
        public List<SchedulerQueuePendingViewModel> PendingScheduleMessages { get; set; }
    }
}