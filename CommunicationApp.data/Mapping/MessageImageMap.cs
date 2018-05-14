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
    class MessageImageMap : EntityTypeConfiguration<MessageImage>
    {
        public MessageImageMap()
        {
            //table
            ToTable("MessageImage");

            HasKey(t => t.MessageImageId).Property(c => c.MessageImageId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            //properties
          
            Property(t => t.MessageId);          
            Property(t => t.ImageUrl);
           


            ////CustomerId as foreign key
            //HasRequired(p => p.Customers)
            //    .WithMany(c => c.Messagess)
            //    .HasForeignKey(p => p.AdminId);
        }
    }
}
