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
    class SubCategoryMap : EntityTypeConfiguration<SubCategory>
    {
        public SubCategoryMap()
        {
            ToTable("SubCategory");
            HasKey(t => t.SubCategoryId).Property(c => c.SubCategoryId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity); 
            //properties
            Property(t => t.SubCategoryName).IsRequired();
            Property(t => t.CategoryId).IsRequired();            
            Property(t => t.IsActive);

            // //CategoryId as foreign key

            HasRequired(p => p.Categories)
                .WithMany(p => p.SubCategories)
                .HasForeignKey(p => p.CategoryId);
        }
    }
}
