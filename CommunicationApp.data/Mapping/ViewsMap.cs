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
    class ViewsMap : EntityTypeConfiguration<Views>
    {
        public ViewsMap()
        {
            //table
            ToTable("Views");

            HasKey(t => t.ViewsId).Property(c => c.ViewsId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            //properties
            Property(t => t.CustomerId);
            Property(t => t.PropertyId);  
            Property(t => t.Status);



            //CustomerId as foreign key
            HasRequired(p => p.Customers)
                .WithMany(c => c.Viewss)
                .HasForeignKey(p => p.CustomerId);

           

        }
    }
}
