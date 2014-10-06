using System;
using System.Collections.Generic;

namespace Contact.Monitoring.Web.Models
{
    public partial class SystemUpTime
    {
        public int Id { get; set; }
        public string MachineName { get; set; }
        public System.DateTime LastBootUpTime { get; set; }
        public string Service { get; set; }
    }
}
