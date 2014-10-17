namespace Contact.Monitoring.Models
{
    public partial class SchedulerQueuePending
    {
        public int Id { get; set; }
        public System.DateTime LastUpdatedDateTime { get; set; }
        public System.DateTime PendingFrom { get; set; }
        public int QueueCount { get; set; }
    }
}
