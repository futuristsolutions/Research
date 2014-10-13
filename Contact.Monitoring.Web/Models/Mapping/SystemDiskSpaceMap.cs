using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Contact.Monitoring.Web.Models.Mapping
{
    public class SystemDiskSpaceMap : EntityTypeConfiguration<SystemDiskSpace>
    {
        public SystemDiskSpaceMap()
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

            this.Property(t => t.Drive)
                .IsRequired()
                .HasMaxLength(5);

            // Table & Column Mappings
            this.ToTable("SystemDiskSpace");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.LastUpdatedDateTime).HasColumnName("LastUpdatedDateTime");
            this.Property(t => t.MachineName).HasColumnName("MachineName");
            this.Property(t => t.Service).HasColumnName("Service");
            this.Property(t => t.Drive).HasColumnName("Drive");
            this.Property(t => t.Size).HasColumnName("Size");
            this.Property(t => t.FreeSpace).HasColumnName("FreeSpace");
        }
    }
}
