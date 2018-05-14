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
    class CustomerMap : EntityTypeConfiguration<Customer>
    {
        public CustomerMap()
        {
            //table
            ToTable("Customer");

            HasKey(t => t.CustomerId).Property(c => c.CustomerId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity); 
            //properties
            Property(t => t.CompanyID).IsRequired();
            Property(t => t.UserId).IsRequired();
            Property(t => t.ParentId);
            Property(t => t.PhotoPath);
            Property(t => t.FirstName).IsRequired();
            Property(t => t.LastName);
            Property(t => t.MiddleName);
            Property(t => t.EmailId).IsRequired();
            Property(t => t.MobileNo).IsRequired();
            Property(t => t.Address);
            Property(t => t.DOB);
            Property(t => t.OpenHouseAvalibility);
            Property(t => t.CountryID);
            Property(t => t.StateID);
            Property(t => t.CityID);
            Property(t => t.CityName);
            Property(t => t.ZipCode);
            Property(t => t.ApplicationId);
            Property(t => t.Latitude).HasPrecision(18, 8);
            Property(t => t.Longitude).HasPrecision(18, 8);
            Property(t => t.CreatedOn);
            Property(t => t.LastUpdatedOn);
            Property(t => t.MobileVerifyCode);
            Property(t => t.EmailVerifyCode);
            Property(t => t.IsMobileVerified);
            Property(t => t.IsEmailVerified);
            Property(t => t.DeviceSerialNo);
            Property(t => t.DeviceType);
            Property(t => t.TrebId);
            Property(t => t.WebsiteUrl);
            Property(t => t.Designation);
            Property(t => t.IsAvailable);
            Property(t => t.IsActive).IsRequired();
            Property(t => t.IsUpdated);
            Property(t => t.UpdateStatus);
            Property(t => t.IsAppLike);
            Property(t => t.IsNotificationSoundOn);
            Property(t => t.RecoNumber);
            Property(t => t.RecoExpireDate);
            Property(t => t.RiskAssessmentExpireDate);

            //CompanyID as foreign key
            HasRequired(p => p.Companies)
                .WithMany(c => c.Customers)
                .HasForeignKey(p => p.CompanyID);

            //UserId as foreign key
            HasRequired(p => p.Users)
                .WithMany(c => c.Customers)
                .HasForeignKey(p => p.UserId);

            //CountyID as foreign key
            HasRequired(p => p.Countries)
                .WithMany(c => c.Customers)
                .HasForeignKey(p => p.CountryID);

            //StateID as foreign key
            HasRequired(p => p.States)
                .WithMany(c => c.Customers)
                .HasForeignKey(p => p.StateID);

            //CityID as foreign key
            HasRequired(p => p.Cities)
                .WithMany(c => c.Customers)
                .HasForeignKey(p => p.CityID);
        }
    }
}
