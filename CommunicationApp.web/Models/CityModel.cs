using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace CommunicationApp.Models
{
    public partial class CityModel
    {
        public CityModel()
        {
            //this.Companies = new HashSet<CompanyModel>();
        }
        public int CityID { get; set; }

        [Display(Name="City")]
        [Required(ErrorMessage = "Required")]
        public string CityName { get; set; }

        public int StateID { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }

        public virtual StateModel State { get; set; }
        //public virtual ICollection<CompanyModel> Companies { get; set; }
        //public virtual ICollection<DriverModel> Drivers { get; set; }
        //public virtual ICollection<CustomerModel> Customers { get; set; }
    }
}
