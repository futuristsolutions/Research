namespace Contact.Monitoring.Models
{
    public partial class SystemUpTime
    {
        public int Id { get; set; }
        public System.DateTime LastUpdatedDateTime { get; set; }
        public string MachineName { get; set; }
        public System.DateTime LastBootUpTime { get; set; }
        public string Service { get; set; }
    }
}
