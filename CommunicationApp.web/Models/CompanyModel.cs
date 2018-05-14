using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace CommunicationApp.Models
{
    public partial class CompanyModel
    {
        public CompanyModel()
        {
           // this.Users = new HashSet<UserModel>();
        }

        public int CompanyID { get; set; }
        [Required(ErrorMessage = "Required")]
        [Display(Name = "Name")]
        public string CompanyName { get; set; }
        [Required(ErrorMessage = "Required")]
        [Display(Name = "Address")] 
        public string CompanyAddress { get; set; }
        [Display(Name = "Country")]
        public int CountryID { get; set; }
        [Display(Name = "State")]
        public int StateID { get; set; }
        [Display(Name = "City")]
        public int CityID { get; set; }
        [Required(ErrorMessage = "Required")]
        [Display(Name = "Email")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}"
            + @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\"
            + @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Email is not valid")]
        public string EmailID { get; set; }
        [Required(ErrorMessage = "Required")]
        [Display(Name = "Web Site")]
        public string WebSite { get; set; }
        [Required(ErrorMessage = "Required")]
        [Display(Name = "Phone No")]
        public string PhoneNo { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<System.DateTime> LastUpdatedOn { get; set; }
        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }

        [Display(Name = "Logo")]
        public string LogoPath { get; set; }

        //public virtual CityModel City { get; set; }
        //public virtual CountryModel Country { get; set; }
        //public virtual StateModel State { get; set; }
        //public virtual ICollection<UserModel> Users { get; set; }
    }
}
