using System;
using System.Collections.Generic;

namespace Contact.Monitoring.Web.Models
{
    public partial class SchedulerQueue
    {
        public int Id { get; set; }
        public System.DateTime LastUpdatedDateTime { get; set; }
        public int QueueCount { get; set; }
    }
}