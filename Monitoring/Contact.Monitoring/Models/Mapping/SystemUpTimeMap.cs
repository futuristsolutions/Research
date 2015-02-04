using System.Data.Entity.ModelConfiguration;

namespace Contact.Monitoring.Models.Mapping
{
    public class SystemUpTimeMap : EntityTypeConfiguration<SystemUpTime>
    {
        public SystemUpTimeMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.MachineName)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Service)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("SystemUpTime");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.LastUpdatedDateTime).HasColumnName("LastUpdatedDateTime");
            this.Property(t => t.MachineName).HasColumnName("MachineName");
            this.Property(t => t.LastBootUpTime).HasColumnName("LastBootUpTime");
            this.Property(t => t.Service).HasColumnName("Service");
        }
    }
}
