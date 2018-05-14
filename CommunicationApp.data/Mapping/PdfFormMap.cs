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
    class PdfFormMap : EntityTypeConfiguration<PdfForm>
    {
        public PdfFormMap()
        {
            //table
            ToTable("PdfForm");

            HasKey(t => t.PdfFormId).Property(c => c.PdfFormId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            //properties
            Property(t => t.CustomerId);
            Property(t => t.Url);               
            Property(t => t.CreatedOn);
            Property(t => t.LastUpdatedon);         
            Property(t => t.IsActive);



            //CustomerId as foreign key
            HasRequired(p => p.Customers)
                .WithMany(c => c.PdfForms)
                .HasForeignKey(p => p.CustomerId);
        }
    }
}
