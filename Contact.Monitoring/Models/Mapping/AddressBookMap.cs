using System.Data.Entity.ModelConfiguration;

namespace Contact.Monitoring.Models.Mapping
{
    public class AddressBookMap : EntityTypeConfiguration<AddressBook>
    {
        public AddressBookMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("AddressBook");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.LastUpdatedDateTime).HasColumnName("LastUpdatedDateTime");
            this.Property(t => t.Count).HasColumnName("Count");
        }
    }
}
