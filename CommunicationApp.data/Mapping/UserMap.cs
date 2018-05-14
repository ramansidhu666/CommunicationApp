using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunicationApp.Core;
using CommunicationApp.Entity;
using System.ComponentModel.DataAnnotations.Schema;
namespace CommunicationApp.Data.Mapping
{
    public class UserMap : EntityTypeConfiguration<User>
    {
        public UserMap()
        {
            //table
            ToTable("Users");
            //key
            HasKey(t => t.UserId).Property(c => c.UserId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity); 
            //properties
            Property(t => t.FirstName);
            Property(t => t.LastName);
            Property(t => t.UserName).IsRequired();
            Property(t => t.Password).IsRequired();
            Property(t => t.TrebId).IsRequired();
            Property(t => t.UserEmailAddress).IsRequired();
            Property(t => t.CompanyID);
            Property(t => t.CreatedOn);
            Property(t => t.LastUpdatedOn);
            Property(t => t.IsNewsLetterSend);
            
            Property(t => t.IsActive);

            //CompanyID as foreign key
            HasRequired(p => p.Companies)
                .WithMany(c => c.Users)
                .HasForeignKey(p => p.CompanyID);
        }
    }
}
