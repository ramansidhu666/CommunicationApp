using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunicationApp.Entity;
using System.ComponentModel.DataAnnotations.Schema;
namespace PropertyAppAmeba.Data.Mapping
{
    public class PropertyImageMap : EntityTypeConfiguration<PropertyImage>
    {
        public PropertyImageMap()
        {
            //table
            ToTable("PropertyImage");
            //key
            HasKey(t => t.PropertyImageId).Property(c => c.PropertyImageId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            //properties
            Property(t => t.PropertyId);
            Property(t => t.ImagePath);

            //PropertyID as foreign key
            HasRequired(p => p.Properties)
                .WithMany(c => c.PropertyImages)
                .HasForeignKey(p => p.PropertyId);
            
        }
    }
}
