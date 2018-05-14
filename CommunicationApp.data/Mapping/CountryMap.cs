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
    class CountryMap : EntityTypeConfiguration<Country>
    {
        public CountryMap()
        {
            ToTable("Country");
            HasKey(t => t.CountryID).Property(c => c.CountryID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity); 
            //properties
            Property(t => t.CountryName).IsRequired();
            Property(t => t.ISOAlpha2);
            Property(t => t.ISOAlpha3);
            Property(t => t.ISONumeric);
            Property(t => t.CurrencyCode);
            Property(t => t.CurrrencySymbol);
            Property(t => t.CountryFlag);
            Property(t => t.CreatedOn);
            Property(t => t.IsActive).IsRequired();
        }
    }
}
