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
    class SupplierMap : EntityTypeConfiguration<Supplier>
    {
        public SupplierMap()
        {
            //table
            ToTable("Supplier");

            HasKey(t => t.SupplierId).Property(c => c.SupplierId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity); 
            //properties
            Property(t => t.AdminId).IsRequired();
            Property(t => t.CategoryId).IsRequired();
            Property(t => t.SubCategoryId).IsRequired();
            Property(t => t.FirstName);
            Property(t => t.Lastname);           
            Property(t => t.Address);
            Property(t => t.Description);
            Property(t => t.Skill);
            Property(t => t.Photopath);
            Property(t => t.EmailID);
            Property(t => t.Region);
            Property(t => t.SubRegion); 
            Property(t => t.IsActive).IsRequired();

            //AdminId as foreign key
            HasRequired(p => p.Customers)
                .WithMany(c => c.Suppliers)
                .HasForeignKey(p => p.AdminId);

            //CategoryId as foreign key
            HasRequired(p => p.Categories)
                .WithMany(c => c.Suppliers)
                .HasForeignKey(p => p.CategoryId);
            //SubCategoryId as foreign key
            HasRequired(p => p.SubCategories)
                .WithMany(c => c.Suppliers)
                .HasForeignKey(p => p.SubCategoryId);
                        
        }
    }
}
