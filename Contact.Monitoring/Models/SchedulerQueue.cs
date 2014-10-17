namespace Contact.Monitoring.Models
{
    public partial class SchedulerQueue
    {
        public int Id { get; set; }
        public System.DateTime LastUpdatedDateTime { get; set; }
        public int QueueCount { get; set; }
    }
}
