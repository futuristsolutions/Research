using System;
using System.Collections.Generic;

namespace Contact.Monitoring.Web.Models
{
    public partial class SchedulerQueuePendingViewModel
    {
        public string LastUpdatedDateTime { get; set; }
        public string PendingFrom { get; set; }
        public int QueueCount { get; set; }
    }
}
