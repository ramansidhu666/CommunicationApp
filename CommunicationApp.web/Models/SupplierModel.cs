using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CommunicationApp.Models
{
    public class SupplierModel
    {
        public int SupplierId { get; set; }
        public int? AdminId { get; set; }
        [Display(Name = "Category")]
        [Required(ErrorMessage = "Required")]
        public int? CategoryId { get; set; }
        [Required(ErrorMessage = "Required")]
        [Display(Name = "Sub Category")]
        public int? SubCategoryId { get; set; }
        [MaxLength(20, ErrorMessage = "Length cannot exceed 20 character")]
        [Required(ErrorMessage = "Required")]
        [Display(Name = "First Name")]
        [RegularExpression("^([a-zA-Z.&'-]+)$", ErrorMessage = "Invalid First Name")]
        public string FirstName { get; set; }
        [MaxLength(20, ErrorMessage = "Length cannot exceed 20 character")]
        [Required(ErrorMessage = "Required")]
        [Display(Name = "Last Name")]
        [RegularExpression("^([a-zA-Z.&'-]+)$", ErrorMessage = "Invalid Last Name")]
        public string LastName { get; set; }
        [MaxLength(500, ErrorMessage = "Length cannot exceed 500 character")]
        [Required(ErrorMessage = "Required")]
        [Display(Name = "Address")]        
        public string Address { get; set; }
        [Display(Name = "Description")]
        public string Description { get; set; }
        public string Skill { get; set; }
        [Required(ErrorMessage = "Required")]
        [Display(Name = "Email Id")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}"
            + @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\"
            + @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Invalid E-mail Id")]
        public string EmailID { get; set; }
        [Required(ErrorMessage = "Required")]
        [RegularExpression("^([0-9]+)$", ErrorMessage = "Invalid Mobile No")]
        [Display(Name = "Mobile No.")]
        [MinLength(7)]
        [MaxLength(14)]
        public string MobileNo { get; set; }

        [MaxLength(20, ErrorMessage = "Length cannot exceed 20 character")]
        [Required(ErrorMessage = "Required")]
        [Display(Name = "Region")]
       // [RegularExpression("^([a-zA-Z.&'-]+)$", ErrorMessage = "Invalid Region")]
        public string Region { get; set; }

        [MaxLength(20, ErrorMessage = "Length cannot exceed 20 character")]
        [Required(ErrorMessage = "Required")]
        [Display(Name = "Sub Region")]        
        public string SubRegion { get; set; }
        [Display(Name = "Sub Region Name")]
        public string SubRegionName { get; set; }
        public string Photopath { get; set; }
        public bool IsActive { get; set; }
        public bool IsView { get; set; }
        
    }
}