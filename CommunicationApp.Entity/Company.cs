using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace CommunicationApp.Entity
{
    public partial class Company
    {
        public int CompanyID { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public int CountryID { get; set; }
        public int StateID { get; set; }
        public int CityID { get; set; }
        public string EmailID { get; set; }
        public string WebSite { get; set; }
        public string PhoneNo { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<System.DateTime> LastUpdatedOn { get; set; }
        public bool IsActive { get; set; }
        public string LogoPath { get; set; }

        public virtual City Cities { get; set; }
        public virtual Country Countries { get; set; }
        public virtual State States { get; set; }
        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<Customer> Customers { get; set; }
        public virtual ICollection<OfficeLocation> OfficeLocations { get; set; }
        public virtual ICollection<Agent> Agents { get; set; }
       
    }
}
