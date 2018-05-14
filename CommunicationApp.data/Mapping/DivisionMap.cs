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
    class DivisionMap : EntityTypeConfiguration<Division>
    {
        public DivisionMap()
        {
            //table
            ToTable("Division");

            HasKey(t => t.DivisionId).Property(c => c.DivisionId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            //properties
            Property(t => t.ParentId);
            Property(t => t.DivisionName);
            Property(t => t.CreationDate);  
            
          

           

        }
    }
}
