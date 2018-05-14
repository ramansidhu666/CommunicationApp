using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunicationApp.Entity;
using System.ComponentModel.DataAnnotations.Schema;
namespace CommunicationApp.Data.Mapping
{
    public class CityMap : EntityTypeConfiguration<City>
    {
        public CityMap()
        {
            //table
            ToTable("City");
            //key
            HasKey(t => t.CityID).Property(c => c.CityID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            //properties
            Property(t => t.CityName).IsRequired();
            Property(t => t.StateID).IsRequired();
            Property(t => t.CreatedOn);
            Property(t => t.IsActive).IsRequired();
            

            //StateID as foreign key
            HasRequired(p => p.States)
                .WithMany(c => c.Cities)
                .HasForeignKey(p => p.StateID);

            
        }
    }
}
