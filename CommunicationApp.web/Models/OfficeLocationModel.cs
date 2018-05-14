using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CommunicationApp.Models
{
    public class OfficeLocationModel
    {
        public int OfficeLocationId { get; set; }
        public int? CompanyId { get; set; }
        [Required(ErrorMessage = "Required")]
        [Display(Name = "Office Address")]
        public string OfficeAddress { get; set; }

        [MaxLength (20,ErrorMessage="you can enter only maximum 20 character")]
        [Display(Name = "City")]
        [Required(ErrorMessage = "must enter city.")]
        public string  City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        [Required(ErrorMessage = "Required")]
        [Display(Name = "Telephone No")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Entered phone format is not valid.")]
        public string TelephoneNo { get; set; }
        
        [Required(ErrorMessage = "Required")]
        [Display(Name = "Fax")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Entered fax format is not valid.")]
        public string Fax { get; set; }
        [Required(ErrorMessage = "Required")]
        [Display(Name = "Email")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}"
            + @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\"
            + @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Invalid Office E-mail Id")]
        public string Email { get; set; }
        public Decimal? Latitude { get; set; }
        public Decimal? Longitude { get; set; }
    }
}

 