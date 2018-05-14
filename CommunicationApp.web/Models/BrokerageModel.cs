using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommunicationApp.Web.Models
{
    public class BrokeragerModel
    {
        public BrokeragerModel()
        {
            this.BrokeragerDetailModelList = new List<BrokeragerDetailModel>();

        }
        public int BrokerageId { get; set; }
        public int? CustomerId { get; set; }       
        public string Brokerage { get; set; }
        public string BrokerageDate { get; set; }
        public string Office { get; set; }
        public string Completedby { get; set; }
        public string BrokerVerification { get; set; }
        public string VerificationDate { get; set; }
        public string BrokerageOverallRiskLevel { get; set; }
        public string Explanation { get; set; }
        public string PageUrl { get; set; }
        public string ApprovedBy { get; set; }
        public string ApprovalDescription { get; set; }
        public bool Status { get; set; }
        public Nullable<DateTime> ApprovalDate { get; set; }
        public Nullable< DateTime> CreatedOn { get; set; }
        public List<BrokeragerDetailModel> BrokeragerDetailModelList { get; set; }
        public List<PDFDetailModel> PDFDetailModelModelList { get; set; }

        public string EmailId { get; set; }
        public string MobileNo { get; set; }
               public string TrebId { get; set; }

    }


    public class BrokerageListModel
    {
        public BrokerageListModel()
        {
            this.BrokeragerDetailModelList = new List<BrokeragerDetailModel>();

        }
        public Nullable<DateTime> CreatedOn { get; set; }
        public string ApprovedBy { get; set; }
        public Nullable<DateTime> ApprovalDate { get; set; }
        public string ApprovalDescription { get; set; }
        public string Status { get; set; }
        public int BrokerageId { get; set; }
        public string TrebId { get; set; }
        public string PhNo { get; set; }
        public string FirstName { get; set; }
        public string EmailID { get; set; }
        public string MobileNo { get; set; }
        public int? CustomerId { get; set; }
        public string Brokerage { get; set; }
        public string BrokerageDate { get; set; }
        public DateTime BrokDate { get; set; }
        public string Office { get; set; }
        public string Completedby { get; set; }
        public string BrokerVerification { get; set; }
        public DateTime VerifDate { get; set; }
        public string VerificationDate { get; set; }
        public string BrokerageOverallRiskLevel { get; set; }
        public string Explanation { get; set; }
        public string PageUrl { get; set; }
        public List<BrokeragerDetailModel> BrokeragerDetailModelList { get; set; }
        public List<PDFDetailModel> PDFDetailModelModelList { get; set; }


    }


    public class BrokerageCustomer
    {
        public int BrokerageId { get; set; }
        public int? CustomerId { get; set; }       
        public string Brokerage { get; set; }
        public string BrokerageDate { get; set; }
        public string Office { get; set; }
        public string Completedby { get; set; }
        public string BrokerVerification { get; set; }
        public string VerificationDate { get; set; }
        public string BrokerageOverallRiskLevel { get; set; }
        public string Explanation { get; set; }
        public string PageUrl { get; set; }
        public List<BrokeragerDetailModel> BrokeragerDetailModelList { get; set; }
        public List<PDFDetailModel> PDFDetailModelModelList { get; set; }

  
        public int? ParentId { get; set; }
    
        public int CompanyID { get; set; }

      
     
        public string TrebId { get; set; }

      
        public string PhotoPath { get; set; }
      
        public string Logo { get; set; }
        public string CompanyName { get; set; }
       
        public string FirstName { get; set; }

       
        public string LastName { get; set; }

        public string MiddleName { get; set; }

       
        public string EmailID { get; set; }

        public string MobileNo { get; set; }
        public string DOB { get; set; }
       
        public string Password { get; set; }

      
        public string Address { get; set; }

        
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
        public string Designation { get; set; }
        public string DeviceSerialNo { get; set; }
        public string DeviceType { get; set; }//

        public string BrokerageName { get; set; }
        public string BrokerageEmail { get; set; }
        public string BrokeragePhoto { get; set; }//
        public string BrokeragePhoneNo { get; set; }
       
        public string ActiveTime { get; set; }
       
        public string WebsiteUrl { get; set; }
        public string TipUrl { get; set; }
        public string OpenHouseAvalibility { get; set; }

        public string RecoNumber { get; set; }
        
        public Nullable<System.DateTime> RecoExpireDate { get; set; }
        //public Nullable<System.DateTime> RiskAssessmentExpireDate { get; set; }
        public string RiskAssessmentExpireDate { get; set; }
    }
    
}