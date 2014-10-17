namespace Contact.Monitoring.ViewModel
{
    public class PerformanceCounterDataViewModel
    {
        public long Id { get; set; }
        public string Timestamp { get; set; }
        public string MachineName { get; set; }
        public string Service { get; set; }
        public string Counter { get; set; }
        public double CounterValue { get; set; }
    }
}
