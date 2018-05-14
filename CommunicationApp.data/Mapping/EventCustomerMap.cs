using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunicationApp.Entity;
using System.ComponentModel.DataAnnotations.Schema;
namespace CommunicationApp.Data.Mapping
{
    public class EventCustomerMap : EntityTypeConfiguration<EventCustomer>
    {
        public EventCustomerMap()
        {
            //table
            ToTable("EventCustomer");
            //key
            HasKey(t => t.EventCustomerId).Property(c => c.EventCustomerId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            //properties
            Property(t => t.CustomerId).IsRequired();
            Property(t => t.EventId).IsRequired();



            //CustomerId as foreign key
            HasRequired(p => p.Customers)
                .WithMany(c => c.EventCustomers)
                .HasForeignKey(p => p.CustomerId);

            //CustomerId as foreign key
            HasRequired(p => p.Events)
                .WithMany(c => c.EventCustomers)
                .HasForeignKey(p => p.EventId);

            
        }
    }
}
