namespace Contact.Monitoring.ViewModel
{
    public class MaxPerformanceCounterDataViewModel
    {
        public string MachineName { get; set; }
        public string Service { get; set; }
        public string Counter { get; set; }
        public decimal CounterValue { get; set; }
    }
}