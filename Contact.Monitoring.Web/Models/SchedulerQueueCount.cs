using System;
using System.Collections.Generic;

namespace Contact.Monitoring.Web.Models
{
    public partial class SchedulerQueueCount
    {
        public int Id { get; set; }
        public System.DateTime LastUpdatedDateTime { get; set; }
        public int QueueCount { get; set; }
    }
}
