namespace Contact.Monitoring.ViewModel
{
    public class SystemDiskSpaceViewModel
    {
        public string LastUpdatedDateTime { get; set; }
        public string MachineName { get; set; }
        public string Service { get; set; }
        public string Drive { get; set; }
        public decimal Size { get; set; }
        public decimal FreeSpace { get; set; }
    }
}