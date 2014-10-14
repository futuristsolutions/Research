using System;
using System.Collections.Generic;

namespace Contact.Monitoring.Web.Models
{
    public partial class AddressBook
    {
        public int Id { get; set; }
        public System.DateTime LastUpdatedDateTime { get; set; }
        public int Count { get; set; }
    }
}
