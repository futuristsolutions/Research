namespace Contact.Monitoring.Web.ViewModel
{
    public class SystemUpTimeViewModel
    {
        public string MachineName { get; set; }
        public System.DateTime LastBootUpTime { get; set; }
        public string Service { get; set; }
    }
}
