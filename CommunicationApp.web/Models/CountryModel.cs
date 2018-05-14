using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CommunicationApp.Entity;
namespace CommunicationApp.Models
{
    public partial class CountryModel
    {
        public CountryModel()
        {
            //this.Companies = new HashSet<Company>();
            //this.States = new HashSet<StateModel>();
        }
        public int CountryID { get; set; }

        [Display(Name="Country")]
        [Required(ErrorMessage = "Required")]
        public string CountryName { get; set; }
        public string ISOAlpha2 { get; set; }
        public string ISOAlpha3 { get; set; }
        public int? ISONumeric { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrrencySymbol { get; set; }
        public string CountryFlag { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }

        //public virtual ICollection<Company> Companies { get; set; }
        //public virtual ICollection<StateModel> States { get; set; }
    }
}
