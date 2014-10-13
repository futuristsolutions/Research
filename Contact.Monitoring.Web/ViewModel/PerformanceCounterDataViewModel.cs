using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Contact.Monitoring.Web.Controllers
{
    public class PerformanceCounterDataViewModel
    {
        public long Id { get; set; }
        public string Timestamp { get; set; }
        public string MachineName { get; set; }
        public string Service { get; set; }
        public string Counter { get; set; }
        public double CounterValue { get; set; }
    }
}
