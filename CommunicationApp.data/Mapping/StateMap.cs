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
    class StateMap : EntityTypeConfiguration<State>
    {

        public StateMap()
        {
            //table
            ToTable("State");
            HasKey(t => t.StateID).Property(c => c.StateID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity); 
            //properties
            Property(t => t.CountryID).IsRequired();
            Property(t => t.CreatedOn);
            Property(t => t.IsActive).IsRequired();
            Property(t => t.StateName).IsRequired();

            //CountryID as foreign key
            HasRequired(p => p.Countries)
                .WithMany(c => c.States)
                .HasForeignKey(p => p.CountryID);
            
        }
    }
}
