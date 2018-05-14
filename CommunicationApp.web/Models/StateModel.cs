using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace CommunicationApp.Models
{
    public partial class StateModel
    {
        public StateModel()
        {
            //this.Cities = new HashSet<CityModel>();
            //this.Companies = new HashSet<CompanyModel>();
        }

        public int StateID { get; set; }

        [Display(Name = "State")]
        [Required(ErrorMessage="Required")]
        public string StateName { get; set; }

        public int CountryID { get; set; }

        public Nullable<System.DateTime> CreatedOn { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }

        //public virtual ICollection<CityModel> Cities { get; set; }
        //public virtual ICollection<CompanyModel> Companies { get; set; }
        //public virtual CountryModel Country { get; set; }
    }
}
