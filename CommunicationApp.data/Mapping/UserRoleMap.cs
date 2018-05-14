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
    class UserRoleMap : EntityTypeConfiguration<UserRole>
    {

        public UserRoleMap()
        {
            //table
            ToTable("UserRoles");
            HasKey(t => t.UserRoleId).Property(c => c.UserRoleId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity); 
            //properties
            Property(t => t.RoleId).IsRequired();
            Property(t => t.UserId).IsRequired();

            //RoleID as foreign key
            HasRequired(p => p.Roles)
                .WithMany(c => c.UserRoles)
                .HasForeignKey(p => p.RoleId);

            //UserId as foreign key
            HasRequired(p => p.Users)
                .WithMany(c => c.UserRoles)
                .HasForeignKey(p => p.UserId);
        }
    }
}
