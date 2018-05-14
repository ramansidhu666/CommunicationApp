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
    class RoleDetailMap : EntityTypeConfiguration<RoleDetail>
    {

        public RoleDetailMap()
        {
            //table
            ToTable("RoleDetails");
            HasKey(t => t.RoleDetailID).Property(c => c.RoleDetailID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity); 
            //properties
            Property(t => t.IsCreate);
            Property(t => t.IsEdit);
            Property(t => t.IsView);
            Property(t => t.IsDelete);
            Property(t => t.IsDownload);
            Property(t => t.CreateDate);
            Property(t => t.IsDetail);
            Property(t => t.FormId);
            Property(t => t.RoleId);

            //FormID as foreign key
            HasRequired(p => p.Forms)
                .WithMany(c => c.RoleDetails)
                .HasForeignKey(p => p.FormId);

            //RoleID as foreign key
            HasRequired(p => p.Roles)
                .WithMany(c => c.RoleDetails)
                .HasForeignKey(p => p.RoleId);
        }
    }
}
