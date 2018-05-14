
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace CommunicationApp.Entity
{
    public partial class Customer
    {
        public int CustomerId { get; set; }
        public int CompanyID { get; set; }
        public int UserId { get; set; }
        public int? ParentId { get; set; }
        public string PhotoPath { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string EmailId { get; set; }
        public string MobileNo { get; set; }
        public string Address { get; set; }
        public string OpenHouseAvalibility { get; set; }
        public string DOB { get; set; }
        public int CountryID { get; set; }
        public int StateID { get; set; }
        public int CityID { get; set; }
        public string CityName { get; set; }
        public string ZipCode { get; set; }
        public string ApplicationId { get; set; }
        public Decimal? Latitude { get; set; }
        public Decimal? Longitude { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<System.DateTime> LastUpdatedOn { get; set; }
        public string MobileVerifyCode { get; set; }
        public string EmailVerifyCode { get; set; }
        public bool IsMobileVerified { get; set; }
        public bool IsEmailVerified { get; set; }
        public string DeviceSerialNo { get; set; }
        public string DeviceType { get; set; }
        public string WebsiteUrl { get; set; }
        public string Designation { get; set; }
        public string TrebId { get; set; }
        public bool? IsAvailable { get; set; }
        public bool IsActive { get; set; }
        public bool? IsUpdated { get; set; }//
        public bool? UpdateStatus { get; set; }
        public bool? IsAppLike { get; set; }
        public bool? IsNotificationSoundOn { get; set; }
        public string RecoNumber { get; set; }
        public Nullable<System.DateTime> RecoExpireDate { get; set; }
        //public Nullable<System.DateTime> RiskAssessmentExpireDate { get; set; }
        public string RiskAssessmentExpireDate { get; set; }
        public virtual User Users { get; set; }
        public virtual Company Companies { get; set; }
        public virtual Country Countries { get; set; }
        public virtual State States { get; set; }
        public virtual City Cities { get; set; }
        public virtual ICollection<Property> Properties { get; set; }
        public virtual ICollection<Agent> Agents { get; set; }
        public virtual ICollection<FeedBack> FeedBacks { get; set; }//
        public virtual ICollection<Event> Events { get; set; }
        public virtual ICollection<Tip> Tips { get; set; }
        public virtual ICollection<EventCustomer> EventCustomers { get; set; }
        public virtual ICollection<AdminStaff> AdminStaffs { get; set; }
        public virtual ICollection<Views> Viewss { get; set; }
        public virtual ICollection<Supplier> Suppliers { get; set; }
        public virtual ICollection<Message> Messagess { get; set; }
        public virtual ICollection<PdfForm> PdfForms { get; set; }//
        public virtual ICollection<Brokerage> Brokerages { get; set; }//
        public virtual ICollection<Banner> Banners { get; set; }//
        public virtual ICollection<NewsLetter_Entity> NewsLetters { get; set; }
    }
}
