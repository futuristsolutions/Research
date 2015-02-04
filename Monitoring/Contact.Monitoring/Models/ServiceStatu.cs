namespace Contact.Monitoring.Models
{
    public partial class ServiceStatu
    {
        public int Id { get; set; }
        public System.DateTime LastUpdatedDateTime { get; set; }
        public string MachineName { get; set; }
        public string Service { get; set; }
        public string Instance { get; set; }
        public string Status { get; set; }
    }
}
