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
    public class BrokerageDetailMap : EntityTypeConfiguration<BrokerageDetail>
    {
        public BrokerageDetailMap()
        {
            //table
            ToTable("BrokerageDetail");

            HasKey(t => t.BrokerageDetailId).Property(c => c.BrokerageDetailId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            //properties
            Property(t => t.BrokerageId);
            Property(t => t.BrokerageServicesId);
            Property(t => t.BrokerageServicesValue);



            //CustomerId as foreign key
            HasRequired(p => p.Brokerages)
                .WithMany(c => c.BrokerageDetails)
                .HasForeignKey(p => p.BrokerageId);

            //CustomerId as foreign key
            HasRequired(p => p.BrokerageSevices)
                .WithMany(c => c.BrokerageDetails)
                .HasForeignKey(p => p.BrokerageServicesId);

        }
    }
}
