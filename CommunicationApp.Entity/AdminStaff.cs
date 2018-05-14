using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace CommunicationApp.Entity
{
    public partial class AdminStaff
    {
        public int AdminStaffId { get; set; }
        public int CustomerId { get; set; }        
        public string PhotoPath { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailId { get; set; }
        public string MobileNo { get; set; }
        public string Address { get; set; }       
        public string DOB { get; set; }       
        public string CityName { get; set; }
        public string ZipCode { get; set; }       
        public Decimal Latitude { get; set; }
        public Decimal Longitude { get; set; }
        public string WebsiteUrl { get; set; }
        public string Designation { get; set; }
        public bool IsActive { get; set; }
        public virtual Customer Customers { get; set; }
        
    }
}
