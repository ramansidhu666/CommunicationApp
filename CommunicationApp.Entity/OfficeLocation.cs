using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace CommunicationApp.Entity
{
   public class OfficeLocation
    {
        public int OfficeLocationId { get; set; }
        public int? CompanyId { get; set; }
        public string OfficeAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string TelephoneNo { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public Decimal? Latitude { get; set; }
        public Decimal? Longitude { get; set; }
        public virtual Company Companies { get; set; }
        
    }
}
