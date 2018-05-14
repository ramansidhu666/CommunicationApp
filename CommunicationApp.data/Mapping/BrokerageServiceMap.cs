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
    public class BrokerageSeviceMap : EntityTypeConfiguration<BrokerageSevice>
    {
        public BrokerageSeviceMap()
        {
            //table
            ToTable("BrokerageServices");

            HasKey(t => t.BrokerageServicesId).Property(c => c.BrokerageServicesId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            //properties
            Property(t => t.BrokerageServices);
            Property(t => t.ParentId);
            Property(t => t.Status);
           



            ////CustomerId as foreign key
            //HasRequired(p => p.Customers)
            //    .WithMany(c => c.Agents)
            //    .HasForeignKey(p => p.CustomerId);

        }
    }
}
