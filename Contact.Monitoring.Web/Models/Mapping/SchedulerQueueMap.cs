using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Contact.Monitoring.Web.Models.Mapping
{
    public class SchedulerQueueMap : EntityTypeConfiguration<SchedulerQueue>
    {
        public SchedulerQueueMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("SchedulerQueue");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.LastUpdatedDateTime).HasColumnName("LastUpdatedDateTime");
            this.Property(t => t.QueueCount).HasColumnName("QueueCount");
        }
    }
}
