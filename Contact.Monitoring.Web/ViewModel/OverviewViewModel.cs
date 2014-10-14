using System.Collections.Generic;

namespace Contact.Monitoring.Web.ViewModel
{
    public class OverviewViewModel
    {
        public StatusViewModel Servers { get; set; }

        public StatusViewModel Health { get; set; }

        public List<ServiceOverviewViewModel> ServiceOverview { get; set; }

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