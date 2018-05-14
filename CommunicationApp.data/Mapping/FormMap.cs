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
    class FormMap : EntityTypeConfiguration<Form>
    {
        public FormMap()
        {
            //table
            ToTable("Forms");
            HasKey(t => t.FormId).Property(c => c.FormId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity); 
            //properties
            Property(t => t.FormName).IsRequired();
            Property(t => t.ControllerName).IsRequired();
            
        }
    }
}