using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace CommunicationApp.Entity
{
    public partial class OpenHouse
    {
        public int OpenHouseId { get; set; }
        public int? CompanyId { get; set; }
        public int? CustomerId { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string ContactNumber { get; set; }
        public Nullable<DateTime> FromDateTime { get; set; }
        public Nullable<DateTime> ToDateTime { get; set; }
        public decimal? Price { get; set; }
        public string Comments { get; set; }
        public Nullable<DateTime> CreatedOn { get; set; }
        public Nullable<DateTime> LastUpdatedon { get; set; }
        public bool? IsActive { get; set; }

      
        public virtual Company Companies { get; set; }
        public virtual Customer Customers { get; set; }
       
      
    }
}
