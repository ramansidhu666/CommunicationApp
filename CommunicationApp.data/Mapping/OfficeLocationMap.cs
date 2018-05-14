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
    class OfficeLocationMap : EntityTypeConfiguration<OfficeLocation>
    {
        public OfficeLocationMap()
        {
            //table
            ToTable("OfficeLocation");

            HasKey(t => t.OfficeLocationId).Property(c => c.OfficeLocationId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            //properties
            Property(t => t.CompanyId);
            Property(t => t.City);
            Property(t => t.Country);
            Property(t => t.Fax);
            Property(t => t.TelephoneNo);
            Property(t => t.OfficeAddress);
            Property(t => t.Email);
            Property(t => t.Latitude);
            Property(t => t.Longitude);


            //CompanyId as foreign key
            HasRequired(p => p.Companies)
                .WithMany(c => c.OfficeLocations)
                .HasForeignKey(p => p.CompanyId);

           

        }
    }
}
