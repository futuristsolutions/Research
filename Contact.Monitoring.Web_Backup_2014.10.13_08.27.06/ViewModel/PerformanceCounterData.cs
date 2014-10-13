using System;
using System.Collections.Generic;

namespace Contact.Monitoring.Web.ViewModel
{
    public partial class PerformanceCounterDataViewModel
    {
        public long Id { get; set; }
        public System.DateTime Timestamp { get; set; }
        public string MachineName { get; set; }
        public string Service { get; set; }
        public string Counter { get; set; }
        public decimal Value { get; set; }
    }
}
