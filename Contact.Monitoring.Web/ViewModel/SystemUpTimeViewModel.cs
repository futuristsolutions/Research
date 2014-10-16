using System.EnterpriseServices.Internal;

namespace Contact.Monitoring.Web.ViewModel
{
    public class SystemUpTimeViewModel
    {
        public string MachineName { get; set; }
        public string LastBootUpTime { get; set; }
        public string LastUpdatedDateTime { get; set; }
        public string Service { get; set; }
    }
}
