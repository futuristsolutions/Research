using System;
using System.Collections.Generic;

namespace Contact.Monitoring.Web.Models
{
    public partial class PerformanceCounterData
    {
        public long Id { get; set; }
        public System.DateTime Timestamp { get; set; }
        public string MachineName { get; set; }
        public string Service { get; set; }
        public string Counter { get; set; }
        public decimal CounterValue { get; set; }
    }
}
