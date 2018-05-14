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
    class MessageMap : EntityTypeConfiguration<Message>
    {
        public MessageMap()
        {
            //table
            ToTable("Message");

            HasKey(t => t.MessageId).Property(c => c.MessageId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            //properties
            Property(t => t.AdminId);
            Property(t => t.CustomerIds);
            Property(t => t.Messages);
            Property(t => t.Heading);
            Property(t => t.ImageUrl);
            Property(t => t.CreatedOn);
            Property(t => t.LastUpdatedon);            
            Property(t => t.IsActive);


            //CustomerId as foreign key
            HasRequired(p => p.Customers)
                .WithMany(c => c.Messagess)
                .HasForeignKey(p => p.AdminId);
        }
    }
}
