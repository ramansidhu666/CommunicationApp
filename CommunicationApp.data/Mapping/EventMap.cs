using CommunicationApp.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationApp.Data.Mapping
{
    class EventMap : EntityTypeConfiguration<Event>
    {
        public EventMap()
        {
            //table
            ToTable("Event");

            HasKey(t => t.EventId).Property(c => c.EventId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            //properties
            Property(t => t.CustomerId);
            Property(t => t.EventName);
            Property(t => t.EventImage);
            Property(t => t.StartTime);
            Property(t => t.EndTime);
            Property(t => t.EventDescription);            
            Property(t => t.CreatedOn);
            Property(t => t.LastUpdatedon);
            Property(t => t.IsActive);



            //CustomerId as foreign key
            HasRequired(p => p.Customers)
                .WithMany(c => c.Events)
                .HasForeignKey(p => p.CustomerId);

           

        }
    }
}
