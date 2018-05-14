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
    class BannerMap : EntityTypeConfiguration<Banner>
    {
        public BannerMap()
        {
            //table
            ToTable("Banner");

            HasKey(t => t.BannerId).Property(c => c.BannerId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            //properties
            Property(t => t.CustomerId);
            Property(t => t.Url);               
            Property(t => t.CreatedOn);
            Property(t => t.LastUpdatedon);         
            Property(t => t.IsActive);



            //CustomerId as foreign key
            HasRequired(p => p.Customers)
                .WithMany(c => c.Banners)
                .HasForeignKey(p => p.CustomerId);
        }
    }
}
