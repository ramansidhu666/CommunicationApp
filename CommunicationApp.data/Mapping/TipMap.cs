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
    class TipMap : EntityTypeConfiguration<Tip>
    {
        public TipMap()
        {
            //table
            ToTable("Tip");

            HasKey(t => t.TipId).Property(c => c.TipId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            //properties
            Property(t => t.CustomerId);
            Property(t => t.TipUrl);
            Property(t => t.Title);    
            Property(t => t.CreatedOn);
            Property(t => t.LastUpdatedon);
            Property(t => t.ShowDate);
            Property(t => t.IsActive);



            //CustomerId as foreign key
            HasRequired(p => p.Customers)
                .WithMany(c => c.Tips)
                .HasForeignKey(p => p.CustomerId);

           

        }
    }
}
