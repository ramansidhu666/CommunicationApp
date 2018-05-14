using CommunicationApp.Models;
using CommunicationApp.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CommunicationApp.Models
{
    public class AdminModel
    {
       
            [Display(Name = "Customer ID")]
            public int CustomerId { get; set; }
            public int? ParentId { get; set; }
            [Display(Name = "Company Name")]
            public int CompanyID { get; set; }
            
            [MaxLength(50, ErrorMessage = "Length cannot exceed 50 character")]            
            [Display(Name = "Company Name")]
            //[RegularExpression("^([a-zA-Z .&'-]+)$", ErrorMessage = "Invalid Last Name")]
            public string CompanyName { get; set; }

            [Display(Name = "Name")]
            public int UserId { get; set; }
            [Display(Name = "Treb Id")]
            public string TrebId { get; set; }

            [Display(Name = "Photo")]
            public string PhotoPath { get; set; }
            [MaxLength(20, ErrorMessage = "Length cannot exceed 20 character")]
            [Required(ErrorMessage = "Required")]
            [Display(Name = "First Name")]
            [RegularExpression("^([a-zA-Z .&'-]+)$", ErrorMessage = "Invalid First Name")]
            public string FirstName { get; set; }
            [MaxLength(20 ,ErrorMessage = "Length cannot exceed 20 character")]
            [Required(ErrorMessage = "Required")]
            [Display(Name = "Last Name")]
            [RegularExpression("^([a-zA-Z .&'-]+)$", ErrorMessage = "Invalid Last Name")]
            public string LastName { get; set; }

            [MaxLength(20, ErrorMessage = "Length cannot exceed 20 character")]
            [Display(Name = "Middle Name")]
            [RegularExpression("^([a-zA-Z .&'-]+)$", ErrorMessage = "Invalid Middle Name")]
            public string MiddleName { get; set; }

            [Required(ErrorMessage = "Required")]
            [Display(Name = "Email Id")]
            [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}"
                + @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\"
                + @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Invalid Office E-mail Id")]
            public string EmailID { get; set; }

            [Required(ErrorMessage = "Required")]
            [RegularExpression("^([0-9]+)$", ErrorMessage = "Invalid Mobile No")]
            [Display(Name = "Mobile No.")]
            [MinLength(7)]
            public string MobileNo { get; set; }
            public string DOB { get; set; }
            [Required(ErrorMessage = "Required")]            
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Address")]
            public string Address { get; set; }

            [Display(Name = "Country")]
            public int CountryID { get; set; }

            [Display(Name = "State")]
            public int StateID { get; set; }

            [Display(Name = "City")]
            public int CityID { get; set; }

            [Display(Name = "CityName")]
            public string CityName { get; set; }
            [Display(Name = "Zip Code")]
            [RegularExpression(@"^\d{5}(-\d{4})?$", ErrorMessage = "Invalid Zip Code")]
            public string ZipCode { get; set; }

            [Display(Name = "Application ID")]
            [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "Invalid Application ID")]
            public string ApplicationId { get; set; }

            [Display(Name = "Latitude")]
            public Decimal? Latitude { get; set; }

            [Display(Name = "Longitude")]
            public Decimal? Longitude { get; set; }

            public Nullable<System.DateTime> CreatedOn { get; set; }
            public Nullable<System.DateTime> LastUpdatedOn { get; set; }

            [Display(Name = "Mobile Verify Code")]
            public string MobileVerifyCode { get; set; }
            [Display(Name = "Email Verify Code")]
            public string EmailVerifyCode { get; set; }

            [Display(Name = "Mobile Verified")]
            public bool IsMobileVerified { get; set; }
            [Display(Name = "Admin Company Logo")]
            public string AdminCompanyLogo { get; set; }           
            [Display(Name = "Admin Company Address")]
            public string AdminCompanyAddress { get; set; }
            [Display(Name = "Email Verified")]
            public bool IsEmailVerified { get; set; }
            public string Designation { get; set; }
            public string DeviceSerialNo { get; set; }
            public string DeviceType { get; set; }//

            [Display(Name = "Website Url")]
            [Required(ErrorMessage = "Required")]
            public string WebsiteUrl { get; set; }
            public string TipUrl { get; set; }
            public string OpenHouseAvalibility { get; set; }
           


            [Display(Name = "Is Available")]
            public bool? IsAvailable { get; set; }
            [Required(ErrorMessage = "Required")]
            [Display(Name = "Is Active")]
            public bool IsActive { get; set; }
            public bool IsUpdated { get; set; }
            public bool? UpdateStatus { get; set; }
            public bool? IsNotificationSoundOn { get; set; }
            public List<OfficeLocationModel> OfficeLocationModelList { get; set; }
            public List<EventModel> EventModelList { get; set; }
            public List<UserCustomerModel> StaffMemberList { get; set; }
            public virtual UserModel User { get; set; }
            public virtual CompanyModel Company { get; set; }
            public virtual CountryModel Country { get; set; }
            public virtual StateModel State { get; set; }
            public virtual CityModel City { get; set; }
        
    }
}