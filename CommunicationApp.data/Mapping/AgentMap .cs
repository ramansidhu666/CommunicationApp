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
    class AgentMap : EntityTypeConfiguration<Agent>
    {
        public AgentMap()
        {
            //table
            ToTable("Agent");

            HasKey(t => t.AgentId).Property(c => c.AgentId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity); 
            //properties
            Property(t => t.CompanyId).IsRequired();
            Property(t => t.CustomerId).IsRequired();
            Property(t => t.ParentId);
            Property(t => t.MLS);
            Property(t => t.City);           
            Property(t => t.Date);
            Property(t => t.FromTime);
            Property(t => t.ToTime);
            Property(t => t.Date2);
            Property(t => t.FromTime2);
            Property(t => t.ToTime2);
            Property(t => t.Price);
            Property(t => t.AgentStatusId);
            Property(t => t.Comments);          
            Property(t => t.CreatedOn);
            Property(t => t.LastUpdatedon);          
            Property(t => t.IsActive).IsRequired();

            //CompanyId as foreign key
            HasRequired(p => p.Companies)
                .WithMany(c => c.Agents)
                .HasForeignKey(p => p.CompanyId);

            //CustomerId as foreign key
            HasRequired(p => p.Customers)
                .WithMany(c => c.Agents)
                .HasForeignKey(p => p.CustomerId);
                        
        }
    }
}
