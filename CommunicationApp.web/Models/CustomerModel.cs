using CommunicationApp.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;
namespace CommunicationApp.Models
{
    public partial class CustomerModel
    {
        [Display(Name = "Customer ID")]
        public int CustomerId { get; set; }
        public int? ParentId { get; set; }
        [Display(Name = "Company Name")]
        public int CompanyID { get; set; }

        [Display(Name = "Name")]
        public int UserId { get; set; }
        [Display(Name = "Treb Id")]
        public string TrebId { get; set; }

        [Display(Name = "Photo")]
        public string PhotoPath { get; set; }
        [Display(Name = "Logo")]
        public string Logo { get; set; }
        public string CompanyName { get; set; }
        [Required(ErrorMessage = "Required")]
        [Display(Name = "First Name")]
        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "Invalid First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Required")]
        [Display(Name = "Last Name")]
        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "Invalid Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Middle Name")]
        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "Invalid Middle Name")]
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
        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "special characters are not allowed.")]
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

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public Nullable<System.DateTime> CreatedOn { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public Nullable<System.DateTime> LastUpdatedOn { get; set; }

        [Display(Name = "Mobile Verify Code")]
        public string MobileVerifyCode { get; set; }
        [Display(Name = "Email Verify Code")]
        public string EmailVerifyCode { get; set; }

        [Display(Name = "Mobile Verified")]
        public bool IsMobileVerified { get; set; }
        
        [Display(Name = "Email Verified")]
        public bool IsEmailVerified { get; set; }
        public string Designation { get; set; } 
        public string DeviceSerialNo { get; set; }
        public string DeviceType { get; set; }//

        public string BrokerageName { get; set; }
        public string BrokerageEmail { get; set; }
        public string BrokeragePhoto { get; set; }//
        public string BrokeragePhoneNo { get; set; }
       [Display(Name = "Active Time")]
        public string ActiveTime { get; set; }
        [Display(Name = "Website Url")]        
        public string WebsiteUrl { get; set; }
        public string TipUrl { get; set; }
        public string OpenHouseAvalibility { get; set; }

        public string RecoNumber { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public Nullable<System.DateTime> RecoExpireDate { get; set; }
        //public Nullable<System.DateTime> RiskAssessmentExpireDate { get; set; }
        public string RiskAssessmentExpireDate { get; set; }
        public DateTime RiskExpiryDateNew { get; set; }
        [Display(Name = "Is Available")]      
        public bool? IsAvailable { get; set; }
        [Required(ErrorMessage = "Required")]
        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }
        public bool IsUpdated { get; set; }
        public bool? UpdateStatus { get; set; }
        public bool IsAppLike { get; set; }
        public bool? IsNotificationSoundOn { get; set; }
        public List<OfficeLocationModel> OfficeLocationModelList { get; set; }
        public List<EventModel> EventModelList { get; set; }
        public List<UserCustomerModel> StaffMemberList { get; set; }
        public List<MessageModel> MessageModelList { get; set; }
        public virtual UserModel User { get; set; }
        public virtual CompanyModel Company { get; set; }
        public virtual CountryModel Country { get; set; }
        public virtual StateModel State { get; set; }
        public virtual CityModel City { get; set; }
    }

    public class CustomerRiskExpiryModel
    {
        public int CustomerId { get; set; }
        public DateTime RiskExpiryDateNew { get; set; }
        public string RiskAssessmentExpireDate { get; set; }
    }

    public partial class CustomerExportModel
    {        
      
        [Display(Name = "Treb Id")]
        public string TrebId { get; set; }

        [Display(Name = "Photo")]
        public string PhotoPath { get; set; }
      
        [Display(Name = "First Name")]        
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]      
        public string LastName { get; set; }
        [Display(Name = "Middle Name")]        
        public string MiddleName { get; set; }
        [Display(Name = "Email Id")]       
        public string EmailID { get; set; }       
        [Display(Name = "Mobile No.")]    
        public string MobileNo { get; set; }      
        [Display(Name = "Website Url")]
        public string WebsiteUrl { get; set; }    
        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }
     
    }
    public class UserCustomerModel
    {
        public int? CustomerId { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string EmailID { get; set; }
        public string Flag { get; set; }
        public string TrebId { get; set; }
        public string Designation { get; set; }
        public string MobileNo { get; set; }
        public string ApplicationId { get; set; }
        public Decimal? Latitude { get; set; }
        public Decimal? Longitude { get; set; }
        public string DeviceType { get; set; }
        public string DeviceSerialNo { get; set; }

    }
    public class CustomerResponseModel
    {
        public int? CustomerId { get; set; }
        public string MobileVerifyCode { get; set; }
        public bool IsMobileVerified { get; set; }
        public string MobileNo { get; set; }
        public string File { get; set; }   
        public Decimal? Latitude { get; set; }
        public Decimal? Longitude { get; set; }
    }
    public class CustomerMobileVerifyModel
    {
        public int CustomerId { get; set; }
        public string MobileVerifyCode { get; set; }
    }
    public class ValidateEmailCodeModel
    {
        public int CustomerId { get; set; }
        public string EmailVerifyCode { get; set; }

    }

    public class TripHistoryModel
    {
        public int VehicleId { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string GpsPositionFrom { get; set; }
        public string GpsPositionTo { get; set; }
        public string LatitudeFrom { get; set; }
        public string LongitudeFrom { get; set; }
        public string LatitudeTo { get; set; }
        public string LongitudeTo { get; set; }
        public string AddressFrom { get; set; }
        public string AddressTo { get; set; }
        public string Kilometer { get; set; }
        public string TripDuration { get; set; }
        public string AverageSpeed { get; set; }
        public string TopSpeed { get; set; }
        public string FuelConsumed { get; set; }
    }
    public class EventHistoryModel
    {
        public string EventName { get; set; }
        public string MessageTime { get; set; }
        public string Location { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Address { get; set; }
        public string GpsSpeed { get; set; }
    }
    public class DeviceStatusModel
    {
        public string DateTime { get; set; }
        public string Odometer { get; set; }
        public string EngineOnOff { get; set; }
        public string VehicleBattery { get; set; }
        public string Alarm { get; set; }
        public string DoorsUnlocked { get; set; }
        public string Lights { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string EmergencyCenter { get; set; }
        public string AssistanceCenter { get; set; }
    }

    public class DeleteImageModel
    {
        public string ImagePropertyId { get; set; }
        public string DeletedImagePath { get; set; }
        public string NewImagePath { get; set; }       
       
    }

  
}
