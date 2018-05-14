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
    class OpenHouseMap : EntityTypeConfiguration<OpenHouse>
    {
        public OpenHouseMap()
        {
            //table
            ToTable("OpenHouse");

            HasKey(t => t.OpenHouseId).Property(c => c.OpenHouseId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity); 
            //properties
            Property(t => t.CompanyId).IsRequired();
            Property(t => t.CustomerId).IsRequired();
            Property(t => t.Address);
            Property(t => t.City);
            Property(t => t.ContactNumber);
            Property(t => t.FromDateTime);
            Property(t => t.ToDateTime);
            Property(t => t.Price);
            Property(t => t.Comments);          
            Property(t => t.CreatedOn);
            Property(t => t.LastUpdatedon);          
            Property(t => t.IsActive).IsRequired();

            //CompanyId as foreign key
            HasRequired(p => p.Companies)
                .WithMany(c => c.OpenHouses)
                .HasForeignKey(p => p.CompanyId);

            //CustomerId as foreign key
            HasRequired(p => p.Customers)
                .WithMany(c => c.OpenHouses)
                .HasForeignKey(p => p.CustomerId);
                        
        }
    }
}
