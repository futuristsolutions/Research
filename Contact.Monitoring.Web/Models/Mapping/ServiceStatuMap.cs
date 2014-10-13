using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Contact.Monitoring.Web.Models.Mapping
{
    public class ServiceStatuMap : EntityTypeConfiguration<ServiceStatu>
    {
        public ServiceStatuMap()
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

            this.Property(t => t.Instance)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Status)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("ServiceStatus");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.LastUpdatedDateTime).HasColumnName("LastUpdatedDateTime");
            this.Property(t => t.MachineName).HasColumnName("MachineName");
            this.Property(t => t.Service).HasColumnName("Service");
            this.Property(t => t.Instance).HasColumnName("Instance");
            this.Property(t => t.Status).HasColumnName("Status");
        }
    }
}