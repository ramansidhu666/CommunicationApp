using AutoMapper;
using CommunicationApp.Controllers;
using CommunicationApp.Core.Infrastructure;
using CommunicationApp.Entity;
using CommunicationApp.Infrastructure;
using CommunicationApp.Models;
using CommunicationApp.Services;
using CommunicationApp.Web.Infrastructure.PushNotificationFile;
using CommunicationApp.Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
//using ExportToExcel;
using System.Text.RegularExpressions;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using System.Web.UI.WebControls;




namespace CommunicationApp.Web.Controllers
{
    public class CustomerController : BaseController
    {
        public IUserService _UserService { get; set; }
        public IUserRoleService _UserRoleService { get; set; }       
        public IOfficeLocationService _OfficeLocationService { get; set; }
        public ICityService _CityService { get; set; }
        public IStateService _StateService { get; set; }
        public ICountryService _CountryService { get; set; }
        public ICompanyService _CompanyService { get; set; }
        public IAgentService _AgentService { get; set; }
        public IEventCustomerService _EventCustomerService { get; set; }
        public IEventService _EventService { get; set; }
        public ITipService _TipService { get; set; }
        public IPropertyImageService _PropertyImageService { get; set; }
        public IPropertyService _PropertyService { get; set; }
        public IFeedBackService _FeedBackService { get; set; }
        public INotification _Notification { get; set; }
        public CustomerController(INotification Notification, IAgentService AgentService, IEventCustomerService EventCustomerService, IEventService EventService, ITipService TipService, IPropertyService PropertyService, IPropertyImageService PropertyImageService, IFeedBackService FeedBackService, ICompanyService CompanyService, ICountryService CountryService, IStateService StateService, ICityService CityService, IOfficeLocationService OfficeLocationService, ICustomerService CustomerService, IUserService UserService, IFormService FormService, IRoleDetailService RoleDetailService, IRoleService RoleService, IUserRoleService UserRoleService)
            : base(CustomerService, UserService, RoleService, FormService, RoleDetailService, UserRoleService)
        {
            this._Notification = Notification;
            this._CustomerService = CustomerService;
            this._UserService = UserService;
            this._UserRoleService = UserRoleService;
            this._OfficeLocationService = OfficeLocationService;
            this._CompanyService = CompanyService;
            this._CountryService = CountryService;
            this._StateService = StateService;
            this._CityService = CityService;

            this._AgentService = AgentService;
            this._EventCustomerService = EventCustomerService;
            this._EventService = EventService;
            this._CountryService = CountryService;
            this._TipService = TipService;
            this._PropertyService = PropertyService;
            this._PropertyImageService = PropertyImageService;

        }
        //
        // GET: /Customer/
        public ActionResult Index()
        {
            return View();
        }
        private void CheckPermission()
        {
            RoleDetailModel roleDetail = UserPermission("customer");
            TempData["Create"] = roleDetail.IsCreate;
            TempData["Delete"] = roleDetail.IsDelete;
            TempData["Detail"] = roleDetail.IsDetail;
            TempData["Edit"] = roleDetail.IsEdit;
            TempData["View"] = roleDetail.IsView;
        }


        // GET: /Customer/Edit/5
        public ActionResult Edit(int id)
        {

            var Customers = _CustomerService.GetCustomers().Where(c => c.CustomerId == id).FirstOrDefault();
            var User = _UserService.GetUser(Customers.UserId);
            var models = new List<CustomerModel>();
            CommonClass CommonClass = new CommonClass();
            string QStr = "";
            DataTable dt = new DataTable();
            QStr = "Select * From Customer  where CustomerId =" + id;
            dt = CommonClass.GetDataSet(QStr).Tables[0];
            List<CustomerModel> CustomerList = new List<CustomerModel>();
            CustomerModel CustomerEntity = new CustomerModel();
            CustomerEntity.CustomerId = Convert.ToInt32(dt.Rows[0]["CustomerId"]);
            CustomerEntity.UserId = Convert.ToInt32(dt.Rows[0]["UserId"] as int?); ;
            CustomerEntity.TrebId = (dt.Rows[0]["TrebId"]).ToString();
            CustomerEntity.PhotoPath = (dt.Rows[0]["PhotoPath"]).ToString();
            CustomerEntity.FirstName = (dt.Rows[0]["FirstName"]).ToString();
            CustomerEntity.LastName = (dt.Rows[0]["LastName"]).ToString();
            CustomerEntity.MiddleName = (dt.Rows[0]["MiddleName"]).ToString();
            CustomerEntity.MobileNo = (dt.Rows[0]["MobileNo"]).ToString();
            CustomerEntity.EmailID = (dt.Rows[0]["EmailId"]).ToString();
            CustomerEntity.WebsiteUrl = (dt.Rows[0]["WebsiteUrl"]).ToString();

            CustomerEntity.DOB = (dt.Rows[0]["DOB"]).ToString();
            CustomerEntity.Designation = (dt.Rows[0]["Designation"]).ToString();
            CustomerEntity.RecoNumber = (dt.Rows[0]["RecoNumber"]).ToString();
            if (dt.Rows[0]["RecoExpireDate"] != DBNull.Value)
            {
                CustomerEntity.RecoExpireDate = Convert.ToDateTime((dt.Rows[0]["RecoExpireDate"]));
            }


            CustomerEntity.CreatedOn = Convert.ToDateTime(((dt.Rows[0]["CreatedOn"]).ToString() == "" ? "01/01/1900" : dt.Rows[0]["CreatedOn"])); ;
            if (dt.Rows[0]["LastUpdatedOn"] != DBNull.Value)
            {
                CustomerEntity.LastUpdatedOn = Convert.ToDateTime((dt.Rows[0]["LastUpdatedOn"]));
            }
            
            CustomerEntity.IsActive = Convert.ToBoolean((dt.Rows[0]["IsActive"]));
            if (dt.Rows[0]["UpdateStatus"] != DBNull.Value)
            {
                CustomerEntity.UpdateStatus = Convert.ToBoolean((dt.Rows[0]["UpdateStatus"]));
            }
            if (CustomerEntity.PhotoPath != null && CustomerEntity.PhotoPath != "")
            {
                CustomerEntity.PhotoPath = CustomerEntity.PhotoPath;
            }
            else
            {
                CustomerEntity.PhotoPath = CommonCls.GetURL() + "/images/noImage.jpg";
            }
            if (User!=null)
            {
                CustomerEntity.Password = User.Password;
            }
            ViewBag.CityID = (Customers.CityID <= 0 ? "" : Customers.CityID.ToString());
            ViewBag.StateID = (Customers.StateID <= 0 ? "" : Customers.StateID.ToString());
            ViewBag.Countrylist = new SelectList(_CountryService.GetCountries(), "CountryID", "CountryName", Customers.CountryID);
            ViewBag.Citylist = new SelectList(_CityService.GetCities(), "CityID", "CityName", Customers.CityID);
            ViewBag.UserId = new SelectList(_UserService.GetUsers(), "UserId", "FirstName", Customers.UserId);
            return View(CustomerEntity);
        }


        public ActionResult UserProfile(int id)
        {

            var Customers = _CustomerService.GetCustomers().Where(c => c.CustomerId == id).FirstOrDefault();
            var User = _UserService.GetUser(Customers.UserId);
            var models = new List<CustomerModel>();
            CommonClass CommonClass = new CommonClass();
            string QStr = "";
            DataTable dt = new DataTable();
            QStr = "Select * From Customer  where CustomerId =" + id;
            dt = CommonClass.GetDataSet(QStr).Tables[0];
            List<CustomerModel> CustomerList = new List<CustomerModel>();
            CustomerModel CustomerEntity = new CustomerModel();
            CustomerEntity.CustomerId = Convert.ToInt32(dt.Rows[0]["CustomerId"]);
            CustomerEntity.UserId = Convert.ToInt32(dt.Rows[0]["UserId"] as int?); ;
            CustomerEntity.TrebId = (dt.Rows[0]["TrebId"]).ToString();
            CustomerEntity.PhotoPath = (dt.Rows[0]["PhotoPath"]).ToString();
            CustomerEntity.FirstName = (dt.Rows[0]["FirstName"]).ToString();
            CustomerEntity.LastName = (dt.Rows[0]["LastName"]).ToString();
            CustomerEntity.MiddleName = (dt.Rows[0]["MiddleName"]).ToString();
            CustomerEntity.MobileNo = (dt.Rows[0]["MobileNo"]).ToString();
            CustomerEntity.EmailID = (dt.Rows[0]["EmailId"]).ToString();
            CustomerEntity.WebsiteUrl = (dt.Rows[0]["WebsiteUrl"]).ToString();

            CustomerEntity.DOB = (dt.Rows[0]["DOB"]).ToString();
            CustomerEntity.Designation = (dt.Rows[0]["Designation"]).ToString();
            CustomerEntity.RecoNumber = (dt.Rows[0]["RecoNumber"]).ToString();
            if (dt.Rows[0]["RecoExpireDate"] != DBNull.Value)
            {
                CustomerEntity.RecoExpireDate = Convert.ToDateTime((dt.Rows[0]["RecoExpireDate"]));
            }


            CustomerEntity.CreatedOn = Convert.ToDateTime(((dt.Rows[0]["CreatedOn"]).ToString() == "" ? "01/01/1900" : dt.Rows[0]["CreatedOn"])); ;
            if (dt.Rows[0]["LastUpdatedOn"] != DBNull.Value)
            {
                CustomerEntity.LastUpdatedOn = Convert.ToDateTime((dt.Rows[0]["LastUpdatedOn"]));
            }

            CustomerEntity.IsActive = Convert.ToBoolean((dt.Rows[0]["IsActive"]));
            if (dt.Rows[0]["UpdateStatus"] != DBNull.Value)
            {
                CustomerEntity.UpdateStatus = Convert.ToBoolean((dt.Rows[0]["UpdateStatus"]));
            }
            if (CustomerEntity.PhotoPath != null && CustomerEntity.PhotoPath != "")
            {
                CustomerEntity.PhotoPath = CustomerEntity.PhotoPath;
            }
            else
            {
                CustomerEntity.PhotoPath = CommonCls.GetURL() + "/images/noImage.jpg";
            }
            if (User != null)
            {
                CustomerEntity.Password = User.Password;
            }
            ViewBag.CityID = (Customers.CityID <= 0 ? "" : Customers.CityID.ToString());
            ViewBag.StateID = (Customers.StateID <= 0 ? "" : Customers.StateID.ToString());
            ViewBag.Countrylist = new SelectList(_CountryService.GetCountries(), "CountryID", "CountryName", Customers.CountryID);
            ViewBag.Citylist = new SelectList(_CityService.GetCities(), "CityID", "CityName", Customers.CityID);
            ViewBag.UserId = new SelectList(_UserService.GetUsers(), "UserId", "FirstName", Customers.UserId);
            return View(CustomerEntity);
        }

        public ActionResult ActiveProfile(int id)
        {

            var Customers = _CustomerService.GetCustomers().Where(c => c.CustomerId == id).FirstOrDefault();
            var User = _UserService.GetUser(Customers.UserId);
            var models = new List<CustomerModel>();
            CommonClass CommonClass = new CommonClass();
            string QStr = "";
            DataTable dt = new DataTable();
            QStr = "Select * From Customer  where CustomerId =" + id;
            dt = CommonClass.GetDataSet(QStr).Tables[0];
            List<CustomerModel> CustomerList = new List<CustomerModel>();
            CustomerModel CustomerEntity = new CustomerModel();
            CustomerEntity.CustomerId = Convert.ToInt32(dt.Rows[0]["CustomerId"]);
            CustomerEntity.UserId = Convert.ToInt32(dt.Rows[0]["UserId"] as int?); ;
            CustomerEntity.TrebId = (dt.Rows[0]["TrebId"]).ToString();
            CustomerEntity.PhotoPath = (dt.Rows[0]["PhotoPath"]).ToString();
            CustomerEntity.FirstName = (dt.Rows[0]["FirstName"]).ToString();
            CustomerEntity.LastName = (dt.Rows[0]["LastName"]).ToString();
            CustomerEntity.MiddleName = (dt.Rows[0]["MiddleName"]).ToString();
            CustomerEntity.MobileNo = (dt.Rows[0]["MobileNo"]).ToString();
            CustomerEntity.EmailID = (dt.Rows[0]["EmailId"]).ToString();
            CustomerEntity.WebsiteUrl = (dt.Rows[0]["WebsiteUrl"]).ToString();

            CustomerEntity.DOB = (dt.Rows[0]["DOB"]).ToString();
            CustomerEntity.Designation = (dt.Rows[0]["Designation"]).ToString();
            CustomerEntity.RecoNumber = (dt.Rows[0]["RecoNumber"]).ToString();
            if (dt.Rows[0]["RecoExpireDate"] != DBNull.Value)
            {
                CustomerEntity.RecoExpireDate = Convert.ToDateTime((dt.Rows[0]["RecoExpireDate"]));
            }


            CustomerEntity.CreatedOn = Convert.ToDateTime(((dt.Rows[0]["CreatedOn"]).ToString() == "" ? "01/01/1900" : dt.Rows[0]["CreatedOn"])); 
            if (dt.Rows[0]["LastUpdatedOn"] != DBNull.Value)
            {
                CustomerEntity.LastUpdatedOn = Convert.ToDateTime((dt.Rows[0]["LastUpdatedOn"]));
            }

            CustomerEntity.IsActive = Convert.ToBoolean((dt.Rows[0]["IsActive"]));
            if (dt.Rows[0]["UpdateStatus"] != DBNull.Value)
            {
                CustomerEntity.UpdateStatus = Convert.ToBoolean((dt.Rows[0]["UpdateStatus"]));
            }
            if (CustomerEntity.PhotoPath != null && CustomerEntity.PhotoPath != "")
            {
                CustomerEntity.PhotoPath = CustomerEntity.PhotoPath;
            }
            else
            {
                CustomerEntity.PhotoPath = CommonCls.GetURL() + "/images/noImage.jpg";
            }
            if (User != null)
            {
                CustomerEntity.Password = User.Password;
            }
            ViewBag.CityID = (Customers.CityID <= 0 ? "" : Customers.CityID.ToString());
            ViewBag.StateID = (Customers.StateID <= 0 ? "" : Customers.StateID.ToString());
            ViewBag.Countrylist = new SelectList(_CountryService.GetCountries(), "CountryID", "CountryName", Customers.CountryID);
            ViewBag.Citylist = new SelectList(_CityService.GetCities(), "CityID", "CityName", Customers.CityID);
            ViewBag.UserId = new SelectList(_UserService.GetUsers(), "UserId", "FirstName", Customers.UserId);
            return View(CustomerEntity);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit([Bind(Include = "CustomerId,TrebId,WebsiteUrl,ApplicationID,Password,CompanyID,UserId,PhotoPath,FirstName,LastName,MiddleName,EmailID,DOB,MobileNo,CountryID,StateID,CityID,ZipCode,Latitude,Longitude,CreatedOn,LastUpdatedOn,MobileVerifyCode,EmailVerifyCode,IsMobileVerified,IsEmailVerified,IsActive,RecoNumber,RecoExpireDate")] CustomerModel Customermodel, HttpPostedFileBase file)
        {
            UserPermissionAction("property", RoleAction.view.ToString());
            CheckPermission();
            TempData["ShowMessage"] = "";
            TempData["MessageBody"] = "";
            try
            {
                TempData["ShowMessage"] = "error";
                TempData["MessageBody"] = "Please fill the required field with valid data";
                if (ModelState.IsValid)
                {
                  
                    var CustomerFound = _CustomerService.GetCustomers().Where(c => ((c.EmailId.Trim() == Customermodel.EmailID.Trim() || c.MobileNo.Trim() == Customermodel.MobileNo.Trim()) && c.CustomerId != Customermodel.CustomerId)).FirstOrDefault();
                    if (CustomerFound == null)
                    {
                        var PhotoPath = "";

                        var CustomerUpdate = _CustomerService.GetCustomer(Customermodel.CustomerId);//.Where(c => c.CustomerId == Customermodel.CustomerId).FirstOrDefault();
                        if (CustomerUpdate != null)
                        {
                             PhotoPath = CustomerUpdate.PhotoPath;
                            if (file != null)
                            {

                                if (CustomerUpdate.PhotoPath != "")
                                {   //Delete Old Image
                                    string pathDel = Server.MapPath("~/CustomerPhoto");

                                    FileInfo objfile = new FileInfo(pathDel);
                                    if (objfile.Exists) //check file exsit or not
                                    {
                                        objfile.Delete();
                                    }
                                    //End :Delete Old Image
                                }

                                //Save the photo in Folder
                                var fileExt = Path.GetExtension(file.FileName);
                                string fileName = Guid.NewGuid() + fileExt;
                                var subPath = Server.MapPath("~/CustomerPhoto");

                                //Check SubPath Exist or Not
                                if (!Directory.Exists(subPath))
                                {
                                    Directory.CreateDirectory(subPath);
                                }
                                //End : Check SubPath Exist or Not

                                var path = Path.Combine(subPath, fileName);
                                file.SaveAs(path);

                                PhotoPath = CommonCls.GetURL() + "/CustomerPhoto/" + fileName;
                            }


                            CommonClass CommonClass = new CommonClass();
                            string QStr = "";
                            DataTable dt = new DataTable();
                            QStr = "update Customer set PhotoPath='" + PhotoPath + "' ,MobileNo='" + Customermodel.MobileNo + "' ,Address='" + Customermodel.Address + "' ,FirstName='" + Customermodel.FirstName + "' ";
                            QStr += " ,LastName='" + Customermodel.LastName + "',MiddleName='" + Customermodel.MiddleName + "' ,EmailId='" + Customermodel.EmailID + "' ,CityID='" + Convert.ToInt32(Customermodel.CityID) + "' ,StateID='" + Convert.ToInt32(Customermodel.StateID) + "' , ";
                            QStr += "IsActive='" + Convert.ToBoolean(Customermodel.IsActive) + "' ,WebsiteUrl='" + Customermodel.WebsiteUrl + "' ,UpdateStatus='" + true + "' Where CustomerId='" + Convert.ToInt32(Customermodel.CustomerId) + "'  ";
                            CommonClass.ExecuteNonQuery(QStr);
                            // dt = CommonClass.GetDataSet(QStr).Tables[0];
                            CustomerUpdate.LastUpdatedOn = DateTime.Now;
                            _CustomerService.UpdateCustomer(CustomerUpdate);
                            if (Customermodel.IsActive == false)
                            {
                                string Flag = "14";
                                var NotificationStatus = false;
                                string Message = "Your account is deactivated";
                                string JsonMessage = "{\"Flag\":\"" + Flag + "\",\"Message\":\"" + Message + "\"}";
                                //Save Notification
                                Notification Notification = new Notification();
                                Notification.NotificationSendBy = 1;
                                Notification.NotificationSendTo = Convert.ToInt32(CustomerUpdate.CustomerId);
                                Notification.IsRead = false;
                                Notification.Flag =Convert.ToInt32( Flag);
                                Notification.RequestMessage = Message;
                                _Notification.InsertNotification(Notification);
                                if (CustomerUpdate.DeviceType == EnumValue.GetEnumDescription(EnumValue.DeviceType.Android))
                                {
                                    CommonCls.SendGCM_Notifications(CustomerUpdate.ApplicationId, JsonMessage, true);
                                }
                                else
                                {
                                    //Dictionary<string, object> Dictionary = new Dictionary<string, object>();
                                    //Dictionary.Add("Flag", Flag);
                                    //Dictionary.Add("Message", Message);
                                    //NotificationStatus = PushNotificatinAlert.SendPushNotification(CustomerUpdate.ApplicationId, Message, Flag.ToString(), JsonMessage, Dictionary, 1);
                                    int count = _Notification.GetNotifications().Where(c => c.NotificationSendTo == Convert.ToInt32(CustomerUpdate.CustomerId) && c.IsRead == false).ToList().Count();
                                           
                                    CommonCls.TestSendFCM_Notifications(CustomerUpdate.ApplicationId, JsonMessage, Message,count, true);
                                }
                            }
                            string FirstName = CustomerUpdate.FirstName + " " + CustomerUpdate.MiddleName + " " + CustomerUpdate.LastName;
                            if (CustomerUpdate.IsUpdated == true)
                            {
                                SendMailToUpdatedUser(FirstName, CustomerUpdate.EmailId, CustomerUpdate.TrebId);
                            }
                            else
                            {
                                SendMailToUser(FirstName, CustomerUpdate.EmailId, CustomerUpdate.TrebId);
                            }



                            TempData["ShowMessage"] = "success";
                            TempData["MessageBody"] = CustomerUpdate.FirstName + " is update successfully.";
                        }
                        else
                        {
                            TempData["ShowMessage"] = "error";
                            TempData["MessageBody"] = "Customer not found.";
                        }
                        return RedirectToAction("customerlist", "Property");
                    }
                    else
                    {
                        TempData["ShowMessage"] = "error";

                        if (CustomerFound.EmailId.Trim() == Customermodel.EmailID.Trim())
                        {
                            TempData["MessageBody"] = Customermodel.EmailID + " is already exists.";
                        }
                        if (CustomerFound.TrebId.Trim() == Customermodel.TrebId.Trim())
                        {
                            TempData["MessageBody"] = Customermodel.TrebId + " is already exists.";
                        }
                        if (CustomerFound.MobileNo.Trim() == Customermodel.MobileNo.Trim())
                        {
                            TempData["MessageBody"] = "This" + " " + Customermodel.MobileNo + " is already exists.";
                        }
                        else
                        {
                            TempData["MessageBody"] = "Please fill the required field with valid data";
                        }
                    }

                }


            }


            catch (RetryLimitExceededException)
            {
                TempData["ShowMessage"] = "error";
                TempData["MessageBody"] = "Some unknown problem occured while proccessing save operation on " + Customermodel.FirstName + " client";

            }
            var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Key, x.Value.Errors }).ToArray();
            var modelStateErrors = this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors);
            //ViewBag.CityID = new SelectList(_CityService.GetCities(), "CityID", "CityName", Carriermodel.CityID);
            // ViewBag.CompanyID = new SelectList(_CompanyService.GetCompanies(), "CompanyID", "CompanyName", Customermodel.CompanyID);
            //ViewBag.CountryID = new SelectList(_CountryService.GetCountries(), "CountryID", "CountryName", Carriermodel.CountryID);
            //ViewBag.StateID = new SelectList(_StateService.GetStates(), "StateID", "StateName", Carriermodel.StateID);
            ViewBag.UserId = new SelectList(_UserService.GetUsers(), "UserId", "FirstName", Customermodel.UserId);
            ViewBag.CityID = (Customermodel.CityID <= 0 ? "" : Customermodel.CityID.ToString());
            ViewBag.StateID = (Customermodel.StateID <= 0 ? "" : Customermodel.StateID.ToString());
            ViewBag.Countrylist = new SelectList(_CountryService.GetCountries(), "CountryID", "CountryName", Customermodel.CountryID);
            ViewBag.Statelist = new SelectList(_StateService.GetStates(), "StateID", "StateName", Customermodel.StateID);
            ViewBag.Citylist = new SelectList(_CityService.GetCities(), "CityID", "CityName", Customermodel.CityID);


            return View(Customermodel);
        }

        public ActionResult ExportUsers()
        {
            var Customers = _CustomerService.GetCustomers();
            var models = new List<CustomerExportModel>();
            Mapper.CreateMap<CommunicationApp.Entity.Customer, CommunicationApp.Models.CustomerExportModel>();

            foreach (var Customer in Customers)
            {
                CommunicationApp.Models.CustomerExportModel CustomerModel = Mapper.Map<CommunicationApp.Entity.Customer, CommunicationApp.Models.CustomerExportModel>(Customer);


                models.Add(CustomerModel);
            }

            GridView gv = new GridView();
            gv.DataSource = models;
            gv.DataBind();


            if (models != null)
            {
                return new DownloadFileActionResult(models, "UserList.xls");
            }
            else
            {
                return new JavaScriptResult();
            }
        }
        public string SaveImage(string Base64String)
        {
            string fileName = Guid.NewGuid() + ".Jpeg";
            System.Drawing.Image image = CommonCls.Base64ToImage(Base64String);
            var subPath = System.Web.HttpContext.Current.Server.MapPath("~/CustomerPhoto");
            var path = System.IO.Path.Combine(subPath, fileName);
            image.Save(path, System.Drawing.Imaging.ImageFormat.Jpeg);

            string URL = CommonCls.GetURL() + "/CustomerPhoto/" + fileName;
            return URL;
        }

        public void SendMailToUser(string UserName, string EmailAddress, string TrebId)
        {
            try
            {
                //Send mail
                MailMessage mail = new MailMessage();
                string Logourl = CommonCls.GetURL() + "/images/EmailLogo.png";
                string Imageurl = CommonCls.GetURL() + "/images/EmailPic.png";
                string FromEmailID = WebConfigurationManager.AppSettings["FromEmailID"];
                string FromEmailPassword = WebConfigurationManager.AppSettings["FromEmailPassword"];
                string ToEmailID = WebConfigurationManager.AppSettings["ToEmailID"];
                SmtpClient smtpClient = new SmtpClient(ConfigurationManager.AppSettings["SmtpServer"]);
                int _Port = Convert.ToInt32(WebConfigurationManager.AppSettings["Port"].ToString());
                Boolean _UseDefaultCredentials = Convert.ToBoolean(WebConfigurationManager.AppSettings["UseDefaultCredentials"].ToString());
                Boolean _EnableSsl = Convert.ToBoolean(WebConfigurationManager.AppSettings["EnableSsl"].ToString());
                mail.To.Add(new MailAddress(EmailAddress));
                mail.From = new MailAddress(FromEmailID);
                mail.Subject = "Your account is activated";
                //string LogoPath = Common.GetURL() + "/images/logo.png";
                string msgbody = "";
                msgbody += "";
                msgbody += " <div>";
                msgbody += " <div style='float:left;width:100%;'>";
                msgbody += "<h2 style='float:left; width:100%; font-size:16px; font-family:arial; margin:0 0 10px;'>Dear " + UserName + ",</h2>";
                msgbody += "<div style='float:left;width:100%; margin:4px 0;'><p style='float:left; font-size:14px; font-family:arial; line-height:25px; margin:5px 11px 0 0;'>Treb Id :</p> <span style='float:left; font-size:14px; font-family:arial; margin:10px 0 0 0;'>" + TrebId + "</span>";
                msgbody += " </div>";
                msgbody += "<p style='float:left;width:100%; font-size:14px; font-family:arial; line-height:25px; margin:0;'>Your account is approved.</p>";
                msgbody += "<p style='float:left;width:100%; font-size:14px; font-family:arial; line-height:25px; margin:0;'>Login using your TREB ID and start posting/ reviewing entries.";
                msgbody += " </p>";
                msgbody += "<div style='float:left; width:100%; margin:5px 0;'>";
                msgbody += "<span style='float:left; font-size:15px; font-family:arial; margin:12px 0 0 0; width:100%;'>Thanks</span><span style='float:left; font-size:15px; font-family:arial; margin:8px 0 0 0; width:100%;'></span>";

                msgbody += " </div>";
                msgbody += "<div style='float:left; width:100%; margin:5px 0;'>";
                msgbody += "<span style='float:left; font-size:15px; font-family:arial; margin:12px 0 0 0; width:100%;'>Team</span><span style='float:left; font-size:15px; font-family:arial; margin:8px 0 0 0; width:100%;'></span>";

                msgbody += " </div>";
                msgbody += "<div style='width:auto; float:left;'>";
                msgbody += " <p style='font-family:arial; font-size:12px; font-weight:bold; margin:0px; padding:0px;'>Call: 416 844 5725 | 647 977 3268  </p>";
                msgbody += " <p style='font-family:arial; font-size:12px; font-weight:bold; margin:0px; padding:0px;'> Fax: 647 497 5646 </p>";
                msgbody += " <a style='color:red; width:100%; font-size:13px; float:left; margin:5px 0 0 0px; text-decoration:none; font-family:arial;' href='http://www.only4agents.com'>Web: www.only4agents.com</a>";
                msgbody += " <a style='color:red; width:100%; font-size:13px; float:left; margin:5px 0 0 0px; text-decoration:none; font-family:arial;' href='#'>Email: info@only4agents.com</a>";
                msgbody += " </div>";
                msgbody += "<div style='float:left; width:100%; margin:5px 0;'>";
                //msgbody += "<img style='float:left; width:150px;' src='data:image/png;base64," + LogoBase64 + "' /> <img style='float:left; width:310px; margin:30px 0 0 30px;' src='data:image/png;base64," + ImageBase64 + "' />";
                msgbody += "<img style='float:left; width:500px;' src='" + Logourl + "' /> ";
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
        public void SendMailToUpdatedUser(string UserName, string EmailAddress, string TrebId)
        {
            try
            {
                //Send mail
                MailMessage mail = new MailMessage();
                string Logourl = CommonCls.GetURL() + "/images/EmailLogo.png";
                string Imageurl = CommonCls.GetURL() + "/images/EmailPic.png";
                string FromEmailID = WebConfigurationManager.AppSettings["FromEmailID"];
                string FromEmailPassword = WebConfigurationManager.AppSettings["FromEmailPassword"];
                string ToEmailID = WebConfigurationManager.AppSettings["ToEmailID"];
                SmtpClient smtpClient = new SmtpClient(ConfigurationManager.AppSettings["SmtpServer"]);
                int _Port = Convert.ToInt32(WebConfigurationManager.AppSettings["Port"].ToString());
                Boolean _UseDefaultCredentials = Convert.ToBoolean(WebConfigurationManager.AppSettings["UseDefaultCredentials"].ToString());
                Boolean _EnableSsl = Convert.ToBoolean(WebConfigurationManager.AppSettings["EnableSsl"].ToString());
                mail.To.Add(new MailAddress(EmailAddress));
                mail.From = new MailAddress(FromEmailID);
                mail.Subject = "Profile update request - Approved";
                //string LogoPath = Common.GetURL() + "/images/logo.png";
                string msgbody = "";
                msgbody += "";
                msgbody += " <div>";
                msgbody += " <div style='float:left;width:100%;'>";
                msgbody += "<h2 style='float:left; width:100%; font-size:16px; font-family:arial; margin:0 0 10px;'>Dear " + UserName + ",</h2>";
                msgbody += "<div style='float:left;width:100%; margin:4px 0;'><p style='float:left; font-size:14px; font-family:arial; line-height:25px; margin:5px 11px 0 0;'>Treb Id</p> <span style='float:left; font-size:14px; font-family:arial; margin:10px 0 0 0;'>" + TrebId + "</span>";
                msgbody += " </div>";
                msgbody += "<p style='float:left;width:100%; font-size:14px; font-family:arial; line-height:25px; margin:0;'>Your profile was successfully updated.</p>";

                msgbody += "<div style='float:left; width:100%; margin:5px 0;'>";
                msgbody += "<span style='float:left; font-size:15px; font-family:arial; margin:12px 0 0 0; width:100%;'>Thanks</span><span style='float:left; font-size:15px; font-family:arial; margin:8px 0 0 0; width:100%;'></span>";
                msgbody += " </div>";
                msgbody += "<div style='float:left; width:100%; margin:5px 0;'>";
                msgbody += "<span style='float:left; font-size:15px; font-family:arial; margin:12px 0 0 0; width:100%;'>Team</span><span style='float:left; font-size:15px; font-family:arial; margin:8px 0 0 0; width:100%;'></span>";
                msgbody += " </div>";
                msgbody += "<div style='width:auto; float:left;'>";
                msgbody += " <p style='font-family:arial; font-size:12px; font-weight:bold; margin:0px; padding:0px;'>Call: 416 844 5725 | 647 977 3268  </p>";
                msgbody += " <p style='font-family:arial; font-size:12px; font-weight:bold; margin:0px; padding:0px;'> Fax: 647 497 5646 </p>";
                msgbody += " <a style='color:red; width:100%; font-size:13px; float:left; margin:5px 0 0 0px; text-decoration:none; font-family:arial;' href='http://www.only4agents.com'>Web: www.only4agents.com</a>";
                msgbody += " <a style='color:red; width:100%; font-size:13px; float:left; margin:5px 0 0 0px; text-decoration:none; font-family:arial;' href='#'>Email: info@only4agents.com</a>";
                msgbody += " </div>";
                msgbody += "<div style='float:left; width:100%; margin:5px 0;'>";
                //msgbody += "<img style='float:left; width:150px;' src='data:image/png;base64," + LogoBase64 + "' /> <img style='float:left; width:310px; margin:30px 0 0 30px;' src='data:image/png;base64," + ImageBase64 + "' />";
                msgbody += "<img style='float:left; width:500px;' src='" + Logourl + "' /> ";
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
        public void DeleteImage(string filePath)
        {
            try
            {
                var uri = new Uri(filePath);
                var fileName = Path.GetFileName(uri.AbsolutePath);
                var subPath = System.Web.HttpContext.Current.Server.MapPath("~/CustomerPhoto");
                var path = Path.Combine(subPath, fileName);

                FileInfo file = new FileInfo(path);
                if (file.Exists)
                {
                    file.Delete();
                }
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.Message.ToString();
                ErrorLogging.LogError(ex);
            }
        }
    }
}