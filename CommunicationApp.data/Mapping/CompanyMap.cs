using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using CommunicationApp.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace CommunicationApp.Data.Mapping
{
    public class CompanyMap : EntityTypeConfiguration<Company>
    {
        public CompanyMap()
        {
            ToTable("Company");
            HasKey(c => c.CompanyID).Property(c => c.CompanyID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(c => c.CompanyName).IsRequired();
            Property(c => c.CompanyAddress).HasMaxLength(1024).IsRequired();
            Property(c => c.CountryID).IsRequired();
            Property(c => c.StateID).IsRequired();
            Property(c => c.CityID).IsRequired();
            Property(c => c.EmailID).IsRequired();
            Property(c => c.WebSite).IsRequired();
            Property(c => c.PhoneNo).IsRequired();
            Property(c => c.CreatedOn);
            Property(c => c.LastUpdatedOn);
            Property(c => c.IsActive).IsRequired();
            Property(c => c.LogoPath);

            //CountyID as foreign key
            HasRequired(p => p.Countries)
                .WithMany(c => c.Companies)
                .HasForeignKey(p => p.CountryID);

            //StateID as foreign key
            HasRequired(p => p.States)
                .WithMany(c => c.Companies)
                .HasForeignKey(p => p.StateID);

            //CityID as foreign key
            HasRequired(p => p.Cities)
                .WithMany(c => c.Companies)
                .HasForeignKey(p => p.CityID);
           
        }
    }
}
