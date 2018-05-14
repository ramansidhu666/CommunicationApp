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
    public class BrokerageMap : EntityTypeConfiguration<Brokerage>
    {
        public BrokerageMap()
        {
            //table
            ToTable("Brokerage");

            HasKey(t => t.BrokerageId).Property(c => c.BrokerageId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            //properties
            Property(t => t.CustomerId);
            Property(t => t.BrokerageDate);
            Property(t => t.Office);
            Property(t => t.Completedby);
            Property(t => t.BrokerVerification);
            Property(t => t.VerificationDate);
            Property(t => t.BrokerageOverallRiskLevel);
            Property(t => t.Explanation);
            Property(t => t.CreatedOn);
            Property(t => t.ApprovalDate);
            Property(t => t.ApprovalDescription);
            Property(t => t.ApprovedBy);
            Property(t => t.Status);

            //CustomerId as foreign key
            HasRequired(p => p.Customers)
                .WithMany(c => c.Brokerages)
                .HasForeignKey(p => p.CustomerId);

        }
    }
}
