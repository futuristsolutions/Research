namespace Contact.Monitoring.Web.Repository
{
    public class ServiceStatuViewModel
    {
        public int Id { get; set; }
        public string LastUpdatedDateTime { get; set; }
        public string MachineName { get; set; }
        public string Service { get; set; }
        public string Instance { get; set; }
        public string Status { get; set; }
    }
}