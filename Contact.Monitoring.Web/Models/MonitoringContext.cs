using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Contact.Monitoring.Web.Models.Mapping;

namespace Contact.Monitoring.Web.Models
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

        public DbSet<PerformanceCounterData> PerformanceCounterDatas { get; set; }
        public DbSet<SystemUpTime> SystemUpTimes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new PerformanceCounterDataMap());
            modelBuilder.Configurations.Add(new SystemUpTimeMap());
        }
    }
}
