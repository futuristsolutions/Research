using System;
using System.Collections.Generic;

namespace Contact.Monitoring.Web.Models
{
    public partial class SystemDiskSpace
    {
        public int Id { get; set; }
        public System.DateTime LastUpdatedDateTime { get; set; }
        public string MachineName { get; set; }
        public string Service { get; set; }
        public string Drive { get; set; }
        public decimal Size { get; set; }
        public decimal FreeSpace { get; set; }
    }
}
