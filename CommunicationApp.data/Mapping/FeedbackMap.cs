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
    public class FeedBackMap : EntityTypeConfiguration<FeedBack>
    {
        public FeedBackMap()
        {
            //table
            ToTable("FeedBack");
            //key
            HasKey(t => t.FeedBackId).Property(c => c.FeedBackId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            //properties
            Property(t => t.CustomerId);
            Property(t => t.Message);
            Property(t => t.Subject);
            Property(t => t.IsRead);
            Property(t => t.CreatedOn);



            //CustomerId as foreign key
            HasRequired(p => p.Customers)
                .WithMany(c => c.FeedBacks)
                .HasForeignKey(p => p.CustomerId);

            
        }
    }
}
