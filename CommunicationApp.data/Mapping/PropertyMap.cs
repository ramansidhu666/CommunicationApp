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
    class PropertyMap : EntityTypeConfiguration<Property>
    {
        public PropertyMap()
        {
            //table
            ToTable("Property");

            HasKey(t => t.PropertyId).Property(c => c.PropertyId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            //properties
            Property(t => t.CustomerId);
            Property(t => t.PropertyStatusId);
            Property(t => t.SaleOfBusinessId);
            Property(t => t.Age);
            Property(t => t.Basement);
            Property(t => t.Bathrooms);
            Property(t => t.BasementValue);
            Property(t => t.Bedrooms);
            Property(t => t.Community);
            Property(t => t.Garage);
            Property(t => t.LocationPrefered);
            Property(t => t.MLS);
            Property(t => t.Price);
            Property(t => t.MaximumPrice);
            Property(t => t.MininumPrice);     
            Property(t => t.PropertyType);
            Property(t => t.Remark);
            Property(t => t.Size);
            Property(t => t.Style);
            Property(t => t.SideDoorEntrance);
            Property(t => t.Type);
            Property(t => t.GarageType);
            Property(t => t.Balcony);
            Property(t => t.Level);
            Property(t => t.Loundry);
            Property(t => t.ParkingSpace);
            Property(t => t.Alivator);
            Property(t => t.Use);
            Property(t => t.ListPriceCode);
            Property(t => t.TypeCommercial);
            Property(t => t.CategoryCommercial);
            Property(t => t.Zoning);
            Property(t => t.TypeTaxes);
            Property(t => t.Kitchen);
            Property(t => t.CreatedOn);
            Property(t => t.LastUpdatedon);
            Property(t => t.IsActive);
            Property(t => t.IsPropertyUpdated);



            //CustomerId as foreign key
            HasRequired(p => p.Customers)
                .WithMany(c => c.Properties)
                .HasForeignKey(p => p.CustomerId);

           

        }
    }
}
