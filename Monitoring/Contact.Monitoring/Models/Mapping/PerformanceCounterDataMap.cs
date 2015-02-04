using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Contact.Monitoring.Models.Mapping
{
    public class PerformanceCounterDataMap : EntityTypeConfiguration<PerformanceCounterData>
    {
        public PerformanceCounterDataMap()
        {
            // Primary Key
            this.HasKey(t => new { t.Id, t.Timestamp, t.MachineName, t.Service, t.InstanceName, t.Counter, t.CounterValue });

            // Properties
            this.Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.MachineName)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Service)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.InstanceName)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.Counter)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.CounterValue)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("PerformanceCounterData");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Timestamp).HasColumnName("Timestamp");
            this.Property(t => t.MachineName).HasColumnName("MachineName");
            this.Property(t => t.Service).HasColumnName("Service");
            this.Property(t => t.InstanceName).HasColumnName("InstanceName");
            this.Property(t => t.Counter).HasColumnName("Counter");
            this.Property(t => t.CounterValue).HasColumnName("CounterValue");
        }
    }
}
