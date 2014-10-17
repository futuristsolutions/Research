using System.Data.Entity;
using Contact.Monitoring.Models.Mapping;

namespace Contact.Monitoring.Models
{
    public partial class MonitoringContext : DbContext
    {
        static MonitoringContext()
        {
            Database.SetInitializer<MonitoringContext>(null);
        }

        public MonitoringContext()
            : base("Name=MonitoringContext")
        {
        }

        public DbSet<AddressBook> AddressBooks { get; set; }
        public DbSet<PerformanceCounterData> PerformanceCounterDatas { get; set; }
        public DbSet<SchedulerQueue> SchedulerQueues { get; set; }
        public DbSet<SchedulerQueuePending> SchedulerQueuePendings { get; set; }
        public DbSet<ServiceStatu> ServiceStatus { get; set; }
        public DbSet<SystemDiskSpace> SystemDiskSpaces { get; set; }
        public DbSet<SystemUpTime> SystemUpTimes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new AddressBookMap());
            modelBuilder.Configurations.Add(new PerformanceCounterDataMap());
            modelBuilder.Configurations.Add(new SchedulerQueueMap());
            modelBuilder.Configurations.Add(new SchedulerQueuePendingMap());
            modelBuilder.Configurations.Add(new ServiceStatuMap());
            modelBuilder.Configurations.Add(new SystemDiskSpaceMap());
            modelBuilder.Configurations.Add(new SystemUpTimeMap());
        }
    }
}
