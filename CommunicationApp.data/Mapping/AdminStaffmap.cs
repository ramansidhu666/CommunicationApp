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
    class AdminStaffMap : EntityTypeConfiguration<AdminStaff>
    {
        public AdminStaffMap()
        {
            //table
            ToTable("AdminStaff");

            HasKey(t => t.AdminStaffId).Property(c => c.AdminStaffId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity); 
            //properties
            Property(t => t.CustomerId);           
            Property(t => t.PhotoPath);
            Property(t => t.FirstName);
            Property(t => t.LastName);            
            Property(t => t.EmailId);
            Property(t => t.MobileNo);
            Property(t => t.Address);
            Property(t => t.DOB);           
            Property(t => t.CityName);
            Property(t => t.ZipCode);            
            Property(t => t.Latitude).HasPrecision(18, 8);
            Property(t => t.Longitude).HasPrecision(18, 8);            
            Property(t => t.WebsiteUrl);
            Property(t => t.Designation);            
            Property(t => t.IsActive);
            

            //CompanyID as foreign key
            HasRequired(p => p.Customers)
                .WithMany(c => c.AdminStaffs)
                .HasForeignKey(p => p.CustomerId);

           
        }
    }
}
