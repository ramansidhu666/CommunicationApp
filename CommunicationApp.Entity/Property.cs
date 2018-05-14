using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationApp.Entity
{
    public class Property
    {
        public int PropertyId { get; set; }
        public int? CustomerId { get; set; }
        public int? PropertyStatusId { get; set; }
        public int? SaleOfBusinessId { get; set; }
        public int? PropertyForStatusId { get; set; }
        public string MLS { get; set; }
        public string Price { get; set; }
        public string MininumPrice { get; set; }
        public string MaximumPrice { get; set; }
        public string LocationPrefered { get; set; }
        public string Style { get; set; }
        public string Age { get; set; }
        public string Garage { get; set; }
        public string Bedrooms { get; set; }
        public string Bathrooms { get; set; }
        public string PropertyType { get; set; }
        public string Basement { get; set; }
        public string BasementValue { get; set; }
        public string Community { get; set; }
        public string Size { get; set; }
        public string Remark { get; set; }
        public string Kitchen { get; set; }
        public string Type { get; set; }
        public string Balcony { get; set; }
        public string Alivator { get; set; }
        public string ParkingSpace { get; set; }
        public string GarageType { get; set; }
        public string SideDoorEntrance { get; set; }
        public string Loundry { get; set; }
        public string Level { get; set; }
        public string ListPriceCode { get; set; }
        public string TypeTaxes { get; set; }
        public string TypeCommercial { get; set; }
        public string CategoryCommercial { get; set; }
        public string Use { get; set; }
        public string Zoning { get; set; }
        public Nullable<DateTime> CreatedOn { get; set; }
        public Nullable<DateTime> LastUpdatedon { get; set; }
        public bool IsActive { get; set; }//
        public bool? IsPropertyUpdated { get; set; }
        public virtual Customer Customers { get; set; }
        public virtual ICollection<PropertyImage> PropertyImages { get; set; }
        
    }
}
