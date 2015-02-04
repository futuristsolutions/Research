using System.Data.Entity.ModelConfiguration;

namespace Contact.Monitoring.Models.Mapping
{
    public class SchedulerQueuePendingMap : EntityTypeConfiguration<SchedulerQueuePending>
    {
        public SchedulerQueuePendingMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("SchedulerQueuePending");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.LastUpdatedDateTime).HasColumnName("LastUpdatedDateTime");
            this.Property(t => t.PendingFrom).HasColumnName("PendingFrom");
            this.Property(t => t.QueueCount).HasColumnName("QueueCount");
        }
    }
}
