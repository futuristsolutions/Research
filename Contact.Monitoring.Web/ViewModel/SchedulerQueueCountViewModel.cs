using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contact.Monitoring.Web.ViewModel
{
    public class SchedulerQueueCountViewModel
    {
        public int Id { get; set; }
        public string LastUpdatedDateTime { get; set; }
        public int QueueCount { get; set; }
    }
}