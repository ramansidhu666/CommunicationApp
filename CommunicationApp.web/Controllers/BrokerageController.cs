using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommunicationApp.Services;
using CommunicationApp.Web.Models;
using AutoMapper;
using CommunicationApp.Models;
using CommunicationApp.Infrastructure;
using CommunicationApp.Core.Infrastructure;
using System.Web.UI.WebControls;
using System.Net.Mail;
using System.Web.Configuration;
using System.Configuration;
using System.Data;
using CommunicationApp.Entity;

using CommunicationApp.Controllers;
using System.IO;
using System.Web.UI;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.html;
using System.Globalization;

namespace CommunicationApp.Web.Controllers
{
    public class BrokerageController : BaseController
    {
        public ICompanyService _CompanyService { get; set; }
        public IPropertyService _PropertyService { get; set; }
        public IPropertyImageService _PropertyImageService { get; set; }
        public ICustomerService _CustomerService { get; set; }
        public IAgentService _AgentService { get; set; }
        public IBrokerageServices _BrokerageService { get; set; }
        public IBrokerageServiceServices _BrokerageServiceServices { get; set; }
        public IBrokerageDetailServices _BrokerageDetailServices { get; set; }

        public BrokerageController(IAgentService AgentService, ICustomerService CustomerService, IUserService UserService, IRoleService RoleService, IFormService FormService, IRoleDetailService RoleDetailService, IRoleService _RoleService, IUserRoleService UserroleService, IPropertyImageService PropertyImageService, IPropertyService PropertyService, ICompanyService CompanyService, IBrokerageServices BrokerageService, IBrokerageServiceServices BrokerageServiceServices, IBrokerageDetailServices BrokerageDetailServices)
            : base(CustomerService, UserService, RoleService, FormService, RoleDetailService, UserroleService)
        {
            this._CompanyService = CompanyService;
            this._PropertyService = PropertyService;
            this._PropertyImageService = PropertyImageService;
            this._CustomerService = CustomerService;
            this._AgentService = AgentService;
            this._BrokerageService = BrokerageService;
            this._BrokerageServiceServices = BrokerageServiceServices;
            this._BrokerageDetailServices = BrokerageDetailServices;
        }
        // GET: /Brokerage/
        public ActionResult Index(string FirstName,string Email, string PhoneNo, string TrebId)
        {
            if(Session["CustomerID"] ==null)
            {
                return RedirectToAction("Index", "Account");
            }
            List<BrokerageListModel> model = new List<BrokerageListModel>();
            var brokerages = _BrokerageService.GetCompanies().Join(_CustomerService.GetCustomers(), b => b.CustomerId, c => c.CustomerId, (b, c) => new { cust = c, brok = b }).Where(c => c.cust.ParentId ==Convert.ToInt64( Session["UserId"])).ToList();
            Mapper.CreateMap<CommunicationApp.Entity.Brokerage, CommunicationApp.Web.Models.BrokerageListModel>();
            foreach (var brokerage in brokerages.Select(c=>c.brok))
            {
                var result = Mapper.Map<CommunicationApp.Entity.Brokerage, CommunicationApp.Web.Models.BrokerageListModel>(brokerage);
                string status = brokerage.Status == true ? "approved" : "disapproved";
                result.Status = status;
                //if(result.BrokerageDate!= ""&& result.BrokerageDate!=null)
                //{
                //    //string brokDate = DateTime.ParseExact(result.BrokerageDate, "dd/MM/yyyy", CultureInfo.InvariantCulture)
                //    //   .ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
                //    result.BrokDate = Convert.ToDateTime(result.BrokerageDate);
                //}
                //if (result.VerificationDate != "" && result.VerificationDate != null)
                //{
                //    result.VerifDate = Convert.ToDateTime(result.VerificationDate);
                //}
                var cust = _CustomerService.GetCustomer(Convert.ToInt32( result.CustomerId));
                result.TrebId = cust.TrebId;
                result.PhNo = cust.MobileNo;
                result.FirstName = cust.FirstName;
                result.EmailID = cust.EmailId; 
                result.PageUrl = CommonCls.GetURL() + "Brokerage/GetBrokerage?BrokerageId=" + result.BrokerageId;
                model.Add(result);
            }
            //search by  FirstName.
            if (!string.IsNullOrEmpty(FirstName))
            {
                model = model.Where(c => c.FirstName.ToLower().Contains(FirstName.ToLower().Trim())).ToList();//.Where(c => c.FirstName.ToLower().Contains(FirstName.ToLower()));
            }
            //search by  Email
            if (!string.IsNullOrEmpty(Email))
            {
                model = model.Where(c => c.EmailID.ToLower().Contains(Email.ToLower().Trim())).ToList();//.Where(c => c.FirstName.ToLower().Contains(FirstName.ToLower()));
            }
            //search by  PhoneNo
            if (!string.IsNullOrEmpty(PhoneNo))
            {
                model = model.Where(c => c.PhNo.ToLower().Contains(PhoneNo.ToLower().Trim())).ToList();//.Where(c => c.FirstName.ToLower().Contains(FirstName.ToLower()));
            }
            //For Search By TrebId
            if (!string.IsNullOrEmpty(TrebId) && !string.IsNullOrEmpty(TrebId) && TrebId.Trim() != "" && TrebId.Trim() != "")
            {
                model = model.Where(c => c.TrebId == TrebId).ToList();
            }
            SessionBatches();
            return View(model.OrderByDescending(c=>c.BrokerageId));
        }
        public void SessionBatches()
        {
            var CustomerId = Convert.ToInt32(Session["CustomerId"]);
            CommonClass CommonClass = new CommonClass();
            string QStr = "";
            DataTable dt = new DataTable();
            if (CustomerId != 0)
            {
                QStr = "SELECT (Select Count(*) From Agent inner join Customer on Customer.CustomerId=Agent.CustomerId  where Agent.IsActive=0 and Customer.ParentId=" + CustomerId + ") AS DeactiveAgentCount,";
                QStr += "(Select Count(*) From Agent inner join Customer on Customer.CustomerId=Agent.CustomerId  where Agent.IsActive=1 and Customer.ParentId=" + CustomerId + ") AS ActiveAgentCount,";
                QStr += "(Select Count(*) From Customer where ParentId=" + CustomerId + " AND IsActive=0) AS DeactiveCustomerCount,";
                QStr += "(Select Count(*) From Customer Where ParentId=" + CustomerId + " AND IsActive=1) AS ActiveCustomerCount,";
                QStr += "(Select Count(*) From Property inner join Customer on Customer.CustomerId=Property.CustomerId  where Property.IsActive=0 and Customer.ParentId=" + CustomerId + ") AS DeactivePropertyCount,";
                QStr += "(Select Count(*) From Property inner join Customer on Customer.CustomerId=Property.CustomerId  where Property.IsActive=1 and Customer.ParentId=" + CustomerId + ") AS ActivePropertyCount, ";
                QStr += "(Select Count(*) From Customer join Brokerage on Brokerage.CustomerId=Customer.CustomerId Where ParentId=" + CustomerId + " and ApprovalDate is NULL) AS NewRiskForm";
            }
            else
            {
                QStr = "SELECT (Select Count(*) From Agent inner join Customer on Customer.CustomerId=Agent.CustomerId  where Agent.IsActive=0 ) AS DeactiveAgentCount,";
                QStr += "(Select Count(*) From Agent inner join Customer on Customer.CustomerId=Agent.CustomerId  where Agent.IsActive=1) AS ActiveAgentCount,";
                QStr += "(Select Count(*) From Customer where  IsActive=0) AS DeactiveCustomerCount,";
                QStr += "(Select Count(*) From Customer Where  IsActive=1) AS ActiveCustomerCount,";
                QStr += "(Select Count(*) From Property inner join Customer on Customer.CustomerId=Property.CustomerId  where Property.IsActive=0 ) AS DeactivePropertyCount,";
                QStr += "(Select Count(*) From Property inner join Customer on Customer.CustomerId=Property.CustomerId  where Property.IsActive=1 ) AS ActivePropertyCount, ";
                QStr += "(Select Count(*) From Customer join Brokerage on Brokerage.CustomerId=Customer.CustomerId Where ApprovalDate is NULL) AS NewRiskForm";
            }

            dt = CommonClass.GetDataSet(QStr).Tables[0];

            //Pass data to seesion.
            Session["PendingPropertyCount"] = dt.Rows[0]["DeactivePropertyCount"].ToString();
            Session["ApprovePropertyCount"] = dt.Rows[0]["ActivePropertyCount"].ToString();
            Session["PendingAgentCount"] = dt.Rows[0]["DeactiveAgentCount"].ToString();
            Session["ApproveAgentCount"] = dt.Rows[0]["ActiveAgentCount"].ToString();
            Session["PendingUserCount"] = dt.Rows[0]["DeactiveCustomerCount"].ToString();
            Session["ApproveUserCount"] = dt.Rows[0]["ActiveCustomerCount"].ToString();
            Session["NewRiskForm"] = dt.Rows[0]["NewRiskForm"].ToString();
        }
        public ActionResult Brokerage(int CustomerId)
        {

            BrokeragerModel BrokeragerModel = new Web.Models.BrokeragerModel();
            List<BrokeragerDetailModel> BrokeragerDetailModelList = new List<BrokeragerDetailModel>();
            var BrokerageServices = _BrokerageServiceServices.BrokerageSevicess();
            Mapper.CreateMap<CommunicationApp.Entity.Brokerage, CommunicationApp.Web.Models.BrokeragerModel>();
            foreach (var BrokerageService in BrokerageServices)
            {
                if (BrokerageService.ParentId == null)
                {
                    BrokeragerDetailModelList.Add(ServiceModel(BrokerageService));

                    //filtter record according to parent
                    Mapper.CreateMap<CommunicationApp.Entity.Property, CommunicationApp.Web.Models.PropertyModel>();
                    var FiltterBrokerages = BrokerageServices.Where(c => c.ParentId == ServiceModel(BrokerageService).BrokerageServicesId);
                    foreach (var FiltterBrokerage in FiltterBrokerages)
                    {
                        BrokeragerDetailModelList.Add(ServiceModel(FiltterBrokerage));
                    }
                }

            }
            BrokeragerModel.BrokeragerDetailModelList = BrokeragerDetailModelList;
            BrokeragerModel.CustomerId = CustomerId;
            return View(BrokeragerModel);
        }


        public ActionResult GetBrokerage(int BrokerageId)
        {

            BrokeragerModel BrokeragerModel = new Web.Models.BrokeragerModel();
            List<PDFDetailModel> PDFDetailModelList = new List<PDFDetailModel>();
            //get data from brokerage table
            var BrokerageDetail = _BrokerageService.GetBrokerage(BrokerageId);
            if (BrokerageDetail != null)
            {
                
                Mapper.CreateMap<CommunicationApp.Entity.Brokerage, CommunicationApp.Web.Models.BrokeragerModel>();
                BrokeragerModel = Mapper.Map<CommunicationApp.Entity.Brokerage, CommunicationApp.Web.Models.BrokeragerModel>(BrokerageDetail);
                var cust = _CustomerService.GetCustomer(Convert.ToInt32(BrokerageDetail.CustomerId));
                BrokeragerModel.EmailId = cust.EmailId;
                BrokeragerModel.TrebId = cust.TrebId;
                BrokeragerModel.MobileNo = cust.MobileNo;
                //Get brokerage detail table
                var BrokerageDetailObj = _BrokerageDetailServices.BrokerageDetail().Where(c => c.BrokerageId == BrokeragerModel.BrokerageId).ToList();
                Mapper.CreateMap<CommunicationApp.Entity.BrokerageDetail, CommunicationApp.Web.Models.PDFDetailModel>();
                foreach (var Brokeragedtail in BrokerageDetailObj)
                {
                    var _model = Mapper.Map<CommunicationApp.Entity.BrokerageDetail, CommunicationApp.Web.Models.PDFDetailModel>(Brokeragedtail);
                    var Brokerageservice = _BrokerageServiceServices.GetBrokerageService(Convert.ToInt32(_model.BrokerageServicesId));
                    if (Brokerageservice != null)
                    {
                        _model.ParentId = Brokerageservice.ParentId;
                        _model.BrokerageValue = _model.BrokerageServicesValue.ToString();
                    }
                    PDFDetailModelList.Add(BrokerDetailFunction(_model));

                    ////filtter record according to parent                        
                    //var FiltterBrokerages = BrokerageDetailObj.Where(c => c.ParentId == ServiceModel(BrokerageService).BrokerageServicesId);
                    //foreach (var FiltterBrokerage in FiltterBrokerages)
                    //{
                    //    PDFDetailModelList.Add(ServiceModel(FiltterBrokerage));
                    //}

                }
            }

            BrokeragerModel.PDFDetailModelModelList = PDFDetailModelList;
      
            return View("~/Views/Brokerage/PDF.cshtml", BrokeragerModel);
        }
        [HttpPost]
        public ActionResult ApproveDisapprove(string Name,string Description,int BrokerageId,bool Status)
        {
            var brokerage = _BrokerageService.GetBrokerage(BrokerageId);
            if(brokerage!=null)
            {
                brokerage.Status = Status;
                brokerage.ApprovedBy = Name;
                brokerage.ApprovalDate = DateTime.Now;
                brokerage.ApprovalDescription = Description;
                _BrokerageService.UpdateBrokerage(brokerage);

                var cust = _CustomerService.GetCustomer(Convert.ToInt32( brokerage.CustomerId));
                string status = Status == true ? "approved" : "disapproved";
                SendAppDisappMailToUser(cust.FirstName+" "+cust.LastName, cust.EmailId,status, Description);
                TempData["ShowMessage"] = "success";
                TempData["MessageBody"] = "Risk assessment form is successfully "+status+" .";
                return Json("success");
               
            }
            else
            {
                TempData["ShowMessage"] = "error";
                TempData["MessageBody"] = "Some problem occured while proccessing operation.";
                return Json("Some problem occured while proccessing operation");
            }
        }

        [HttpPost]
        public ActionResult Brokerage(BrokeragerModel BrokeragerModel)
        {

            if (BrokeragerModel != null)
            {
                // BrokeragerModel.BrokerageDate = ConvertDate(BrokeragerModel.BrokerageDate);
                //BrokeragerModel.VerificationDate = ConvertDate(BrokeragerModel.BrokerageDate);

                //Save Brokerage
                var BrokerageId = InsertBrokerage(BrokeragerModel);
                //End: 

                if (BrokeragerModel.BrokeragerDetailModelList != null)
                {
                    List<PDFDetailModel> PDFDetailModelModelList = new List<PDFDetailModel>();
                    foreach (var item in BrokeragerModel.BrokeragerDetailModelList)
                    {
                        //save Brokerage Detail
                        item.BrokerageId = BrokerageId;
                        InsertBrokerageDeatil(item);
                        //End

                        //Get data for pdf detail.
                        //PDFDetailModelModelList.Add(BrokerDetailFunction(item));
                        //end
                    }
                    BrokeragerModel.PDFDetailModelModelList = PDFDetailModelModelList;
                    BrokeragerModel.PageUrl = "http://communicationapp.only4agents.com/Brokerage/GetBrokerage?BrokerageId=" + BrokerageId;
                }
            }

            // CreatePdf(BrokeragerModel);//create pdf with function.
            return View("~/Views/Brokerage/UrlPage.cshtml", BrokeragerModel);

        }

        public string ConvertDate(string dateSt)
        {
            string[] DateArr = dateSt.Split('/');
            int year =Convert.ToInt32( DateArr[2]);
            year += 1;
            return DateArr[0] + "/" + DateArr[1] + "/" + year.ToString();
        }

        #region Common functions for brokerage
        public List<SelectListItem> TypeList()
        {
            List<SelectListItem> BrokerageTypeList = new List<SelectListItem>();
            BrokerageTypeList.Add(new SelectListItem() { Text = "Frequently", Value = "1", Selected = false });
            BrokerageTypeList.Add(new SelectListItem() { Text = "Occasionally", Value = "2", Selected = true });
            BrokerageTypeList.Add(new SelectListItem() { Text = "Seldom", Value = "3", Selected = false });
            BrokerageTypeList.Add(new SelectListItem() { Text = "Never", Value = "4", Selected = false });
            BrokerageTypeList.Add(new SelectListItem() { Text = "N/A", Value = "5", Selected = false });
            BrokerageTypeList.Add(new SelectListItem() { Text = "Don't Know", Value = "6", Selected = false });
            return BrokerageTypeList;
        }
        public BrokeragerDetailModel ServiceModel(BrokerageSevice BrokeragerService)
        {
            BrokeragerDetailModel FiltterBrokeragerServiceModel = new Web.Models.BrokeragerDetailModel();

            FiltterBrokeragerServiceModel.BrokerageServicesId = BrokeragerService.BrokerageServicesId;
            FiltterBrokeragerServiceModel.BrokerageServices = BrokeragerService.BrokerageServices;
            FiltterBrokeragerServiceModel.ParentId = BrokeragerService.ParentId;
            return FiltterBrokeragerServiceModel;
        }

        public PDFDetailModel BrokerDetailFunction(PDFDetailModel item)
        {

            var BrokerageServices = _BrokerageServiceServices.BrokerageSevicess().Where(c => c.BrokerageServicesId == item.BrokerageServicesId).FirstOrDefault();
            PDFDetailModel PDFDetailModel = new PDFDetailModel();
            PDFDetailModel.BrokerageServices = BrokerageServices.BrokerageServices;
            PDFDetailModel.ParentId = BrokerageServices.ParentId;
            PDFDetailModel.BrokerageValue = item.BrokerageValue;
            if ((int)EnumValue.BrokerageType.Frequently == Convert.ToInt32(item.BrokerageValue))
            {
                if (BrokerageServices != null)
                {
                    PDFDetailModel.BrokerageValue = EnumValue.BrokerageType.Frequently.ToString();

                }
            }
            else if ((int)EnumValue.BrokerageType.Occasionally == Convert.ToInt32(item.BrokerageValue))
            {
                if (BrokerageServices != null)
                {
                    PDFDetailModel.BrokerageValue = EnumValue.BrokerageType.Occasionally.ToString();

                }
            }
            else if ((int)EnumValue.BrokerageType.Seldom == Convert.ToInt32(item.BrokerageValue))
            {
                if (BrokerageServices != null)
                {
                    PDFDetailModel.BrokerageValue = EnumValue.BrokerageType.Seldom.ToString();

                }
            }
            else if ((int)EnumValue.BrokerageType.DontKnow == Convert.ToInt32(item.BrokerageValue))
            {
                if (BrokerageServices != null)
                {
                    PDFDetailModel.BrokerageValue = EnumValue.BrokerageType.DontKnow.ToString();

                }
            }
            else if ((int)EnumValue.BrokerageType.NA == Convert.ToInt32(item.BrokerageValue))
            {
                if (BrokerageServices != null)
                {
                    PDFDetailModel.BrokerageValue = EnumValue.BrokerageType.NA.ToString();

                }
            }
            else if ((int)EnumValue.BrokerageType.Never == Convert.ToInt32(item.BrokerageValue))
            {
                if (BrokerageServices != null)
                {
                    PDFDetailModel.BrokerageValue = EnumValue.BrokerageType.Never.ToString();
                }
            }
            else
            {
                if (BrokerageServices != null)
                {
                    PDFDetailModel.BrokerageServices = BrokerageServices.BrokerageServices;

                }
            }
            return PDFDetailModel;
        }

        public int InsertBrokerage(BrokeragerModel BrokeragerModel)
        {
            Entity.Brokerage brokerage = new Brokerage();
            brokerage.BrokerageDate = BrokeragerModel.BrokerageDate;
            brokerage.BrokerVerification = BrokeragerModel.BrokerVerification;
            brokerage.Completedby = BrokeragerModel.Completedby;
            brokerage.Office = BrokeragerModel.Office;
            brokerage.VerificationDate = BrokeragerModel.VerificationDate;
            brokerage.Explanation = BrokeragerModel.Explanation;
            brokerage.CreatedOn = DateTime.Now;
            if (BrokeragerModel.CustomerId != null)
            {
                if (BrokeragerModel.CustomerId == 0)
                {
                    brokerage.CustomerId = null;
                }
                else
                {
                    brokerage.CustomerId = Convert.ToInt32(BrokeragerModel.CustomerId);
                }

            }

            brokerage.BrokerageOverallRiskLevel = BrokeragerModel.BrokerageOverallRiskLevel;
            brokerage.BrokerageDate = BrokeragerModel.BrokerageDate;
            _BrokerageService.InsertBrokerage(brokerage);

            //update reco expiredate in customer table
            var Customer = _CustomerService.GetCustomer(Convert.ToInt32(brokerage.CustomerId));
            if (Customer != null)
            {
                if (BrokeragerModel.BrokerageDate != null)
                {
                   // Customer.RiskAssessmentExpireDate = Convert.ToDateTime(BrokeragerModel.BrokerageDate).ToString();
                     //Customer.RiskAssessmentExpireDate = ConvertDate(BrokeragerModel.BrokerageDate);

                    Customer.RiskAssessmentExpireDate = DateTime.Now.AddYears(2).ToString();
                    _CustomerService.UpdateCustomer(Customer);
                }
                string UserName = Customer.FirstName + " " + Customer.LastName;
                string EmailAddress = "";
                string EmailVerifyCode = "";
                string subject = "Risk Assessment form filled by " + Customer.FirstName;
                string Body = "This user has filled the risk Assessment form. ";
                string TrebId = Customer.TrebId;
                string FilledFormUrl = "http://communicationapp.only4agents.com/Brokerage/GetBrokerage?BrokerageId=" + brokerage.BrokerageId;
                if (Customer.ParentId != null)
                {
                    var CustomerParent = _CustomerService.GetCustomer(Convert.ToInt32(Customer.ParentId));
                    if (CustomerParent != null)
                    {
                        EmailAddress = CustomerParent.EmailId;
                    }
                }



                SendMailToAdmin(UserName, EmailAddress, EmailVerifyCode, subject, Body, TrebId, FilledFormUrl);
            }
            //end

            return brokerage.BrokerageId;
        }

        public void InsertBrokerageDeatil(BrokeragerDetailModel BrokerageDetail)
        {
            BrokerageDetail BrokerageDetailEntity = new BrokerageDetail();
            BrokerageDetailEntity.BrokerageId = BrokerageDetail.BrokerageId;
            BrokerageDetailEntity.BrokerageServicesId = BrokerageDetail.BrokerageServicesId;
            BrokerageDetailEntity.BrokerageServicesValue = Convert.ToInt32(BrokerageDetail.BrokerageValue);
            _BrokerageDetailServices.InsertBrokerageDetail(BrokerageDetailEntity);

        }

        #endregion


        #region Send mail to Brokerage
        public void SendMailToAdmin(string UserName, string EmailAddress, string EmailVerifyCode, string subject, string Body, string TrebId, string FilledFormUrl)
        {
            try
            {

                string Logourl = CommonCls.GetURL() + "/images/EmailLogo.png";
                string Imageurl = CommonCls.GetURL() + "/images/EmailPic.png";

                // Send mail.
                MailMessage mail = new MailMessage();

                string FromEmailID = WebConfigurationManager.AppSettings["FromEmailID"];
                string FromEmailPassword = WebConfigurationManager.AppSettings["FromEmailPassword"];
                string ToEmailID = WebConfigurationManager.AppSettings["ToEmailID"];
                SmtpClient smtpClient = new SmtpClient(ConfigurationManager.AppSettings["SmtpServer"]);
                int _Port = Convert.ToInt32(WebConfigurationManager.AppSettings["Port"].ToString());
                Boolean _UseDefaultCredentials = Convert.ToBoolean(WebConfigurationManager.AppSettings["UseDefaultCredentials"].ToString());
                Boolean _EnableSsl = Convert.ToBoolean(WebConfigurationManager.AppSettings["EnableSsl"].ToString());
                mail.To.Add(new MailAddress(EmailAddress));
                mail.From = new MailAddress(FromEmailID);
                mail.Subject = subject;
                //string LogoPath = Common.GetURL() + "/images/logo.png";
                string msgbody = "";
                msgbody += "";
                msgbody += "<div>";
                msgbody += "<div style='float:left;width:100%;'>";
                msgbody += "<h2 style='float:left; width:100%; font-size:16px; font-family:arial; margin:0 0 10px;'>Dear Admin,</h2>";
                // msgbody += "<div style='float:left;width:100%; margin:4px 0;'><p style='float:left; font-size:14px; font-family:arial; line-height:25px; margin:0 11px 0 0;'>This message refers to your post :</p> <span style='float:left; font-size:16px; font-family:arial; margin:2px 0 0 0;'>" + PropertyType + "</span>";
                // msgbody += "</div>";  
                msgbody += "<p style='float:left;width:100%; font-size:14px; font-family:arial; line-height:25px; margin:0;'>" + Body + "";
                msgbody += "</p>";
                msgbody += "<p style='float:left;width:100%; font-size:14px; font-family:arial; line-height:25px; margin:0;'>User Name: <b>" + UserName + "</b>";
                msgbody += "<p style='float:left;width:100%; font-size:14px; font-family:arial; line-height:25px; margin:0;'>Treb Id: <b>" + TrebId + "</b>";
                msgbody += " <a style='color:red; width:100%;font-size:13px; float:left; margin:5px 0 0 0px; text-decoration:none; font-family:arial;' href='" + FilledFormUrl + "'>Click here to Check filled from: </a>";
                msgbody += "</p>";
                //msgbody += "</p>";
                //msgbody += "<p style='float:left;width:100%; font-size:14px; font-family:arial; line-height:25px; margin:0;'>Treb Id: <b>" + TrebId + "</b>";
                msgbody += "</p>";
                msgbody += "<span style='float:left; font-size:15px; font-family:arial; margin:12px 0 0 0; width:100%;'>Team</span>";
                msgbody += "<div style='width:auto; float:left;'>";
                msgbody += " <p style='font-family:arial; font-size:12px; font-weight:bold; margin:0px; padding:0px;'>Call: 416 844 5725 | 647 977 3268  </p>";
                msgbody += " <p style='font-family:arial; font-size:12px; font-weight:bold; margin:0px; padding:0px;'>Fax: 647 497 5646 </p>";
                msgbody += " <a style='color:red; width:100%; font-size:13px; float:left; margin:5px 0 0 0px; text-decoration:none; font-family:arial;' href='www.http://www.only4agents.com'>Web:www.only4agents.com</a>";
                msgbody += " <a style='color:red; width:100%;font-size:13px; float:left; margin:5px 0 0 0px; text-decoration:none; font-family:arial;' href='info@only4agents.com'>Email: info@only4agents.com</a>";
                msgbody += " <a style='color:red; width:100%;font-size:13px; float:left; margin:5px 0 0 0px; text-decoration:none; font-family:arial;' href='http://app.only4agents.com/'>Click here to login: www.app.only4agents.com/</a>";
                msgbody += " </div>";
                msgbody += "<div style='float:left; width:100%; margin:12px 0 6px 0;'>";
                //msgbody +="<img style='float:left; width:150px;' src='data:image/png;base64,"+ LogoBase64 +"' /> <img style='float:left; width:310px; margin:30px 0 0 30px;' src='data:image/png;base64,"+ ImageBase64 +"' />";
                msgbody += "<img style='float:left; width:500px;' src='" + Logourl + "' /> ";
                //<img style='float:left; width:310px; margin:30px 0 0 30px;' src='" + Imageurl + "' />
                msgbody += "</div>";

                msgbody += "</div>";
                msgbody += "</div>";
                mail.Body = msgbody;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Host = "smtp.gmail.com"; //_Host;
                smtp.Port = _Port;
                //smtp.UseDefaultCredentials = _UseDefaultCredentials;
                smtp.Credentials = new System.Net.NetworkCredential(FromEmailID, FromEmailPassword);// Enter senders User name and password
                smtp.EnableSsl = _EnableSsl;
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
        }
        #endregion
        public void SendAppDisappMailToUser(string UserName, string EmailAddress,string status, string Description)
        {
            try
            {

                string Logourl = CommonCls.GetURL() + "/images/EmailLogo.png";
                string Imageurl = CommonCls.GetURL() + "/images/EmailPic.png";

                // Send mail.
                MailMessage mail = new MailMessage();

                string FromEmailID = WebConfigurationManager.AppSettings["FromEmailID"];
                string FromEmailPassword = WebConfigurationManager.AppSettings["FromEmailPassword"];
                string ToEmailID = WebConfigurationManager.AppSettings["ToEmailID"];
                SmtpClient smtpClient = new SmtpClient(ConfigurationManager.AppSettings["SmtpServer"]);
                int _Port = Convert.ToInt32(WebConfigurationManager.AppSettings["Port"].ToString());
                Boolean _UseDefaultCredentials = Convert.ToBoolean(WebConfigurationManager.AppSettings["UseDefaultCredentials"].ToString());
                Boolean _EnableSsl = Convert.ToBoolean(WebConfigurationManager.AppSettings["EnableSsl"].ToString());
                mail.To.Add(new MailAddress(EmailAddress));
                mail.From = new MailAddress(FromEmailID);
                mail.Subject = "Risk assessment form";
                //string LogoPath = Common.GetURL() + "/images/logo.png";
                string msgbody = "";
                msgbody += "";
                msgbody += "<div>";
                msgbody += "<div style='float:left;width:100%;'>";
                msgbody += "<h2 style='float:left; width:100%; font-size:16px; font-family:arial; margin:0 0 10px;'>Dear "+UserName+ ",</h2>";
                // msgbody += "<div style='float:left;width:100%; margin:4px 0;'><p style='float:left; font-size:14px; font-family:arial; line-height:25px; margin:0 11px 0 0;'>This message refers to your post :</p> <span style='float:left; font-size:16px; font-family:arial; margin:2px 0 0 0;'>" + PropertyType + "</span>";
                // msgbody += "</div>";  
                msgbody += "<p style='float:left;width:100%; font-size:14px; font-family:arial; line-height:25px; margin:0;'>Your risk assessment form has been "+status;
                msgbody += "</p>";
                if(Description!="")
                {
                    msgbody += "<p style='float:left;width:100%; font-size:14px; font-family:arial; line-height:25px; margin:0;'>Comments: <b>" + Description + "</b>";

                    msgbody += "</p>";
                }
               
                msgbody += "<span style='float:left; font-size:15px; font-family:arial; margin:12px 0 0 0; width:100%;'>Team</span>";
                msgbody += "<div style='width:auto; float:left;'>";
                msgbody += " <p style='font-family:arial; font-size:12px; font-weight:bold; margin:0px; padding:0px;'>Call: 416 844 5725 | 647 977 3268  </p>";
                msgbody += " <p style='font-family:arial; font-size:12px; font-weight:bold; margin:0px; padding:0px;'>Fax: 647 497 5646 </p>";
                msgbody += " <a style='color:red; width:100%; font-size:13px; float:left; margin:5px 0 0 0px; text-decoration:none; font-family:arial;' href='www.http://www.only4agents.com'>Web:www.only4agents.com</a>";
                msgbody += " <a style='color:red; width:100%;font-size:13px; float:left; margin:5px 0 0 0px; text-decoration:none; font-family:arial;' href='info@only4agents.com'>Email: info@only4agents.com</a>";
                msgbody += " <a style='color:red; width:100%;font-size:13px; float:left; margin:5px 0 0 0px; text-decoration:none; font-family:arial;' href='http://CommunicationApp.only4agents.com/'>Click here to login: www.CommunicationApp.only4agents.com/</a>";
                msgbody += " </div>";
                msgbody += "<div style='float:left; width:100%; margin:12px 0 6px 0;'>";
                //msgbody +="<img style='float:left; width:150px;' src='data:image/png;base64,"+ LogoBase64 +"' /> <img style='float:left; width:310px; margin:30px 0 0 30px;' src='data:image/png;base64,"+ ImageBase64 +"' />";
                //msgbody += "<img style='float:left; width:500px;' src='" + Logourl + "' /> ";
                //<img style='float:left; width:310px; margin:30px 0 0 30px;' src='" + Imageurl + "' />
                msgbody += "</div>";

                msgbody += "</div>";
                msgbody += "</div>";
                mail.Body = msgbody;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Host = "smtp.gmail.com"; //_Host;
                smtp.Port = _Port;
                //smtp.UseDefaultCredentials = _UseDefaultCredentials;
                smtp.Credentials = new System.Net.NetworkCredential(FromEmailID, FromEmailPassword);// Enter senders User name and password
                smtp.EnableSsl = _EnableSsl;
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
        }
        

    }
}