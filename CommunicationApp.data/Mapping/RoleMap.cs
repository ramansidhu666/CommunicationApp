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
    class RoleMap:EntityTypeConfiguration<Role>
    {

        public RoleMap()
        {
            //table
            ToTable("Roles");
            HasKey(t => t.RoleId).Property(c => c.RoleId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity); 
            //properties
            Property(t => t.RoleName).IsRequired();
            Property(t => t.RoleDescription).IsRequired();
            Property(t => t.RoleType);
            Property(t => t.IsActive);
            Property(t => t.CreatedOn);
        }
    }
}
