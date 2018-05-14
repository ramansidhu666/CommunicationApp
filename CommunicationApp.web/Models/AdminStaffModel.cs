using CommunicationApp.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;
namespace CommunicationApp.Models
{
    public partial class AdminStaffModel
    {
        [Display(Name = "AdminStaff ID")]
        public int AdminStaffId { get; set; }      
        public int CustomerId { get; set; } 
        [Display(Name = "Photo")]
        public string PhotoPath { get; set; }
        [MaxLength(20, ErrorMessage = "Length cannot exceed 20 character")]
        [Required(ErrorMessage = "Required")]
        [Display(Name = "First Name")]
        [RegularExpression("^([a-zA-Z .&'-]+)$", ErrorMessage = "Invalid First Name")]
        public string FirstName { get; set; }

        [MaxLength(20, ErrorMessage = "Length cannot exceed 20 character")]
        [Required(ErrorMessage = "Required")]
        [Display(Name = "Last Name")]
        [RegularExpression("^([a-zA-Z.&'-]+)$", ErrorMessage = "Invalid Last Name")]
       public string LastName { get; set; }
        [MaxLength(30, ErrorMessage = "Length cannot exceed 20 character")]
        [Required(ErrorMessage = "Required")]
        [Display(Name = "Email Id")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}"
            + @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\"
            + @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Invalid Office E-mail Id")]
        public string EmailId { get; set; }
        public string Address { get; set; }    
        public string DOB { get; set; } 
        [Required(ErrorMessage = "Required")]
        [RegularExpression("^([0-9]+)$", ErrorMessage = "Invalid Mobile No")]
        [Display(Name = "Mobile No.")]
        [MinLength(7)]
        [MaxLength(14)]
        public string MobileNo { get; set; }
        [Display(Name = "CityName")]
        public string CityName { get; set; }
        [Display(Name = "Zip Code")]
        [RegularExpression(@"^\d{5}(-\d{4})?$", ErrorMessage = "Invalid Zip Code")]
        public string ZipCode { get; set; }
        [Display(Name = "Latitude")]
        public Decimal Latitude { get; set; }
        [Display(Name = "Longitude")]
        public Decimal Longitude { get; set; }        
        [Display(Name = "Website Url")]        
        public string WebsiteUrl { get; set; }
        [Display(Name = "Designation")] 
        public string Designation { get; set; }
        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }
        
        
    }


  
}

 
        
       
       
        
           
              
        
               
        
        
        
        
        
   
        