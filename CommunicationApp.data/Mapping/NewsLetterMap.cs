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
    class NewsLetterMap : EntityTypeConfiguration<NewsLetter_Entity>
    {
        public NewsLetterMap()
        {
            //table
            ToTable("NewsLetter");

            HasKey(t => t.NewsLetterId).Property(c => c.NewsLetterId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            //properties
            Property(t => t.AdminId);
            Property(t => t.NewsLetterName);
            Property(t => t.OrderNo);
            Property(t => t.Image);
            Property(t => t.fwd_date);
            Property(t => t.SelectedUsers);
            Property(t => t.IsActive);
          

            //CustomerId as foreign key
            HasRequired(p => p.Customers)
                .WithMany(c => c.NewsLetters)
                .HasForeignKey(p => p.AdminId);

           

        }
    }
}
