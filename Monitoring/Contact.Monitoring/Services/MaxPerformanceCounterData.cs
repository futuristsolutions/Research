namespace Contact.Monitoring.Services
{
    public class MaxPerformanceCounterData
    {
        public string MachineName { get; set; }
        public string Service { get; set; }
        public string Counter { get; set; }
        public decimal CounterValue { get; set; }
    }
}