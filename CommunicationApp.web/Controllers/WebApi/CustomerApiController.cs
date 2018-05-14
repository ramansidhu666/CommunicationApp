using System;
using System.Web;

using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using CommunicationApp.Entity;
using CommunicationApp.Services;
using Newtonsoft.Json;
using AutoMapper;
using CommunicationApp.Models;
using CommunicationApp.Infrastructure;
using CommunicationApp.Core.Infrastructure;
using System.IO;
using System.Drawing;
using System.Configuration;
//using System.Data.Spatial;
using System.Spatial;
using CommunicationApp.Core.UtilityManager;
using System.Net.Mail;
using System.Web.Configuration;
using CommunicationApp.Web.Models;
using System.Globalization;
using CommunicationApp.Web.Infrastructure.PushNotificationFile;
using CommunicationApp.Web.Infrastructure.AsyncTask;


namespace CommunicationApp.Controllers.WebApi
{
    [RoutePrefix("Customer")]
    public class CustomerApiController : ApiController
    {
        public IUserService _UserService { get; set; }
        public IUserRoleService _UserRoleService { get; set; }
        public ICustomerService _CustomerService { get; set; }
        public IOfficeLocationService _OfficeLocationService { get; set; }
        public IEventService _EventService { get; set; }
        public ITipService _TipService { get; set; }
        public IAdminStaffService _AdminStaffService { get; set; }
        public ISupplierService _SupplierService { get; set; }
        public ISubCategoryService _SubCategoryService { get; set; }
        public ICompanyService _CompanyService { get; set; }
        public IMessageService _MessageService { get; set; }
        public IMessageImageService _MessageImageService { get; set; }
        public IPdfFormService _PdfFormService { get; set; }
        public IDivisionService _DivisionService { get; set; }
        public IBannerService _BannerService { get; set; }
        public INotification _Notification { get; set; }
        public CustomerApiController(INotification Notification,ITipService TipService, IEventService EventService, IOfficeLocationService OfficeLocationService, ICustomerService CustomerService, IUserService UserService, IUserRoleService UserRoleService, IAdminStaffService AdminStaffService, ISupplierService SupplierService, ISubCategoryService SubCategoryService, ICompanyService CompanyService, IMessageService MessageService, IPdfFormService PdfFormService, IDivisionService DivisionService, IBannerService BannerService, IMessageImageService MessageImageService)
        {
            this._Notification = Notification;
            this._CustomerService = CustomerService;
            this._UserService = UserService;
            this._UserRoleService = UserRoleService;
            this._OfficeLocationService = OfficeLocationService;
            this._EventService = EventService;
            this._TipService = TipService;
            this._AdminStaffService = AdminStaffService;
            this._SupplierService = SupplierService;
            this._SubCategoryService = SubCategoryService;
            this._CompanyService = CompanyService;
            this._MessageService = MessageService;
            this._PdfFormService = PdfFormService;
            this._DivisionService = DivisionService;
            this._BannerService = BannerService;//
            this._MessageImageService = MessageImageService;//
        }


        public string ConvertDate(string dateSt)
        {
            string[] DateArr = dateSt.Split('/');
            //From : 0-MM,1-DD,2-YYYY HH:MM:ss
            //From : 0-DD,1-MM,2-YYYY HH:MM:ss
            return DateArr[1] + "/" + DateArr[0] + "/" + DateArr[2];
        }


        [Route("GetAdmin")]
        [HttpGet]
        public HttpResponseMessage GetAdmin(int? Id)
        {
            try
            {
                int AdminId = Convert.ToInt32(Id);
                var Customer = _CustomerService.GetCustomers().Where(c => c.CustomerId == AdminId).FirstOrDefault();
                Mapper.CreateMap<CommunicationApp.Entity.Customer, CommunicationApp.Models.CustomerModel>();
                CommunicationApp.Models.CustomerModel CustomerModel = Mapper.Map<CommunicationApp.Entity.Customer, CommunicationApp.Models.CustomerModel>(Customer);
                if (CustomerModel != null)
                {
                    var OfficeLocations = _OfficeLocationService.GetOfficeLocations().Where(c => c.CompanyId == CustomerModel.CompanyID);
                    Mapper.CreateMap<CommunicationApp.Entity.OfficeLocation, CommunicationApp.Models.OfficeLocationModel>();
                    List<OfficeLocationModel> OfficeLocationModelList = new List<OfficeLocationModel>();
                    foreach (var OfficeLocation in OfficeLocations)
                    {
                        CommunicationApp.Models.OfficeLocationModel OfficeLocationModel = Mapper.Map<CommunicationApp.Entity.OfficeLocation, CommunicationApp.Models.OfficeLocationModel>(OfficeLocation);
                        OfficeLocationModelList.Add(OfficeLocationModel);
                    }
                    CustomerModel.OfficeLocationModelList = OfficeLocationModelList;
                    var Company = _CompanyService.GetCompany(CustomerModel.CompanyID);
                    if (Company != null)
                    {
                        CustomerModel.CompanyName = Company.CompanyName;
                        CustomerModel.Logo = Company.LogoPath;
                    }
                }

                JobScheduler.UpdateActiveUser();

                List<UserCustomerModel> StaffMemberList = new List<UserCustomerModel>();
                var StaffMembers = _AdminStaffService.GetAdminStaffs().Where(c => c.CustomerId == AdminId);
                foreach (var staffMember in StaffMembers)
                {
                    UserCustomerModel staffmember = new Models.UserCustomerModel();
                    staffmember.Name = staffMember.FirstName;
                    staffmember.EmailID = staffMember.EmailId;
                    staffmember.Designation = staffMember.Designation;
                    staffmember.MobileNo = staffMember.MobileNo;
                    StaffMemberList.Add(staffmember);
                }
                if (StaffMemberList.Count != 0)
                {
                    CustomerModel.StaffMemberList = StaffMemberList;
                }
                CustomerModel.FirstName = CustomerModel.FirstName + " " + CustomerModel.LastName;

                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("success", CustomerModel), Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.Message.ToString();
                ErrorLogging.LogError(ex);
                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Customer not found."), Configuration.Formatters.JsonFormatter);
            }
        }
        [Route("GetMessages")]
        [HttpGet]
        public HttpResponseMessage GetMessages(int? Adminid, int AgentId)
        {
            try
            {
                int AdminId = Convert.ToInt32(Adminid);
                List<MessageModel> MessageModelList = new List<MessageModel>();
                var Messages = _MessageService.GetMessages().Where(c => c.AdminId == AdminId).OrderByDescending(c => c.MessageId);
                foreach (var Message in Messages)
                {
                    var Customerids = Message.CustomerIds.Split(',');
                    if (Customerids != null)
                    {
                        if (Customerids.Contains(AgentId.ToString()))
                        {
                            MessageModel MessageModel = new MessageModel();
                            MessageModel.MessageId = Message.MessageId;
                            MessageModel.Heading = Message.Heading;
                            MessageModel.Messages = Message.Messages;
                            MessageModel.CustomerIds = Message.CustomerIds;
                            MessageModel.CreatedOn = Message.CreatedOn;
                            MessageModel.IsWithImage = true;
                            //  MessageModel.ImageUrl = Message.ImageUrl;
                            var MessageImages = _MessageImageService.GetMessageImages().Where(c => c.MessageId == MessageModel.MessageId);
                            List<MessageImageModel> MessageImageModelList = new List<MessageImageModel>();
                            foreach (var MessageImage in MessageImages)
                            {
                                MessageImageModel MessageImageModel = new MessageImageModel();
                                MessageImageModel.ImageUrl = MessageImage.ImageUrl;
                                MessageImageModelList.Add(MessageImageModel);

                            }
                            MessageModel.ImageUrlList = MessageImageModelList;
                            MessageModelList.Add(MessageModel);
                        }

                    }

                }
                //int numberOfObjectsPerPage = 10;
                //MessageModelList = MessageModelList.Skip(numberOfObjectsPerPage * pageNumber).Take(numberOfObjectsPerPage).ToList();

                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("success", MessageModelList), Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.Message.ToString();
                ErrorLogging.LogError(ex);
                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Customer not found."), Configuration.Formatters.JsonFormatter);
            }
        }
        [Route("DeleteMessage")]
        [HttpGet]
        public HttpResponseMessage DeleteMessage(int? CustomerId, int? MesageId)
        {
            try
            {
                List<MessageModel> MessageModelList = new List<MessageModel>();
                var Message = _MessageService.GetMessages().Where(c => c.MessageId == MesageId && c.CustomerIds.Contains(CustomerId.ToString())).FirstOrDefault();
                if (Message != null)
                {
                    List<String> Customerids = Message.CustomerIds.Split(',').Select(i => i.Trim()).Where(i => i != string.Empty).ToList();
                    if (Customerids != null)
                    {
                        Customerids.Remove(CustomerId.ToString());
                        Message.CustomerIds = string.Join<string>(",", Customerids);
                        _MessageService.UpdateMessage(Message);
                    }
                    else
                    {
                        _MessageService.DeleteMessage(Message);
                    }
                    return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("success", "message successfully deleted."), Configuration.Formatters.JsonFormatter);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("success", "No any message found."), Configuration.Formatters.JsonFormatter);
                }

                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("success", ""), Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.Message.ToString();
                ErrorLogging.LogError(ex);
                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "message not found."), Configuration.Formatters.JsonFormatter);
            }
        }
        [Route("GetAllAdmin")]
        [HttpGet]
        public HttpResponseMessage GetAllAdmin()
        {
            try
            {
                var Admins = _CustomerService.GetCustomers().Where(x => x.Designation == "Admin" && x.IsActive == true);
                Mapper.CreateMap<CommunicationApp.Entity.Customer, CommunicationApp.Models.CustomerModel>();
                CustomerModel CustomerModel = new CustomerModel();
                List<CustomerModel> ModelList = new List<CustomerModel>();
                foreach (var Admin in Admins)
                {
                    CustomerModel = Mapper.Map<CommunicationApp.Entity.Customer, CommunicationApp.Models.CustomerModel>(Admin);
                    var Company = _CompanyService.GetCompany(Admin.CompanyID);
                    if (Company != null)
                    {
                        CustomerModel.CompanyName = Company.CompanyName;
                        ModelList.Add(CustomerModel);
                    }


                }

                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("success", ModelList), Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.Message.ToString();
                ErrorLogging.LogError(ex);
                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Admin not found."), Configuration.Formatters.JsonFormatter);
            }
        }

        [Route("GetSuppliersByName")]
        [HttpGet]
        public HttpResponseMessage GetSuppliersByName([FromUri]string SuplierName)
        {
            try
            {
                List<SupplierModel> model = new List<SupplierModel>();

                var Suppliers = _SupplierService.GetSuppliers().Where(c => SuplierName.ToLower().Trim().Contains(c.FirstName.ToLower().Trim())).ToList();
                if (Suppliers.Count == 0)
                {
                    Suppliers = _SupplierService.GetSuppliers().Where(c => SuplierName.Contains(c.Lastname)).ToList();
                }

                Mapper.CreateMap<CommunicationApp.Entity.Supplier, CommunicationApp.Models.SupplierModel>();


                foreach (var supplier in Suppliers)
                {
                    var Skill = _SubCategoryService.GetSubCategory(Convert.ToInt32(supplier.SubCategoryId));
                    var models = Mapper.Map<CommunicationApp.Entity.Supplier, CommunicationApp.Models.SupplierModel>(supplier);
                    var SupDivision = _DivisionService.GetDivision(Convert.ToInt32(supplier.SubRegion));
                    var Division = _DivisionService.GetDivision(Convert.ToInt32(supplier.Region));
                    if (Division != null)
                    {
                        if (SupDivision != null)
                        {
                            models.SubRegionName = SupDivision.DivisionName;
                            models.Region = Division.DivisionName;
                        }
                    }
                    if (Skill != null)
                    {
                        models.Skill = Skill.SubCategoryName;
                    }
                    model.Add(models);
                }
                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("success", model.OrderBy(c => c.FirstName)), Configuration.Formatters.JsonFormatter);
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Supplier not found."), Configuration.Formatters.JsonFormatter);
            }

        }


        [Route("SearchSuppliers")]
        [HttpPost]
        public HttpResponseMessage SearchSuppliers([FromBody]SupplierModel SupplierModel)
        {
            try
            {
                List<SupplierModel> model = new List<SupplierModel>();
                var Suppliers = _SupplierService.GetSuppliers().Where(c => c.CategoryId == SupplierModel.CategoryId && c.SubCategoryId == SupplierModel.SubCategoryId);
                //For Search By Name
                if (!string.IsNullOrEmpty(SupplierModel.Region))
                {
                    Suppliers = Suppliers.Where(c => c.Region == SupplierModel.Region);
                }
                if (!string.IsNullOrEmpty(SupplierModel.SubRegion))
                {
                    Suppliers = Suppliers.Where(c => c.SubRegion == SupplierModel.SubRegion);
                }
                Mapper.CreateMap<CommunicationApp.Entity.Supplier, CommunicationApp.Models.SupplierModel>();
                foreach (var supplier in Suppliers)
                {

                    var Skill = _SubCategoryService.GetSubCategory(Convert.ToInt32(supplier.SubCategoryId));
                    var models = Mapper.Map<CommunicationApp.Entity.Supplier, CommunicationApp.Models.SupplierModel>(supplier);
                    var SupDivision = _DivisionService.GetDivision(Convert.ToInt32(supplier.SubRegion));
                    var Division = _DivisionService.GetDivision(Convert.ToInt32(supplier.Region));
                    if (Division != null)
                    {
                        if (SupDivision != null)
                        {
                            models.SubRegionName = SupDivision.DivisionName;
                            models.Region = Division.DivisionName;
                        }
                    }
                    if (Skill != null)
                    {
                        models.Skill = Skill.SubCategoryName;
                    }
                    model.Add(models);
                }
                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("success", model.OrderBy(c => c.FirstName)), Configuration.Formatters.JsonFormatter);
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Supplier not found."), Configuration.Formatters.JsonFormatter);
            }

        }


        [Route("ReviewSuppliers")]
        [HttpPost]
        public HttpResponseMessage ReviewSuppliers([FromBody]SupplierModel SupplierModel)
        {
            try
            {
                string EmailSendTo = "";
                string UserName = "";
                string EmailAddress = "";
                string TrebId = "";
                string Subject = "";
                string body = "";
                string EmailRecieverName = "";
                var Suplier = _SupplierService.GetSupplier(SupplierModel.SupplierId);
                if (Suplier != null)
                {
                    EmailSendTo = Suplier.EmailID;
                    EmailRecieverName = Suplier.FirstName;
                }

                var Customer = _CustomerService.GetCustomer(Convert.ToInt32(SupplierModel.AdminId));
                if (Customer != null)
                {
                    UserName = Customer.FirstName;
                    EmailAddress = Customer.Address;
                    TrebId = Customer.TrebId;
                    var FirstPart = TrebId.Substring(0, 3);
                    var LastPart = TrebId.Substring(TrebId.Length - 1);
                    if (SupplierModel.IsView)
                    {
                        Subject = "Profile view by " + FirstPart + "***" + LastPart;
                        body = "your profile view by <b>" + FirstPart + "***" + LastPart + "</b> .for any information contact .";
                    }
                    else
                    {
                        Subject = "Shared  by " + FirstPart + "***" + LastPart;
                        body = "your profile shared by <b>" + FirstPart + "***" + LastPart + "</b> .for any information contact .";
                    }
                }
                SendMailToUser(UserName, EmailSendTo, TrebId, Subject, body, EmailRecieverName);

                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("success", "Mail send successfully."), Configuration.Formatters.JsonFormatter);
            }

            catch (Exception ex)
            {
                string ErrorMsg = ex.Message.ToString();
                ErrorLogging.LogError(ex);
                return Request.CreateResponse(HttpStatusCode.NotImplemented, CommonCls.CreateMessage("error", "Plese try later."), Configuration.Formatters.JsonFormatter);
            }
        }


        [Route("GetTipOfTheDay")]
        [HttpGet]
        public HttpResponseMessage GetTipOfTheDay()
        {
            try
            {


                CustomerModel CustomerModel = new Models.CustomerModel();
                //var Tip = _TipService.GetTips().Where(c => c.ShowDate == null && c.CustomerId == ParentId).FirstOrDefault();
                var Tip = _TipService.GetTips().Where(c => c.ShowDate == null).FirstOrDefault();
                if (Tip != null)
                {
                    CustomerModel.TipUrl = Tip.TipUrl;
                    Tip.ShowDate = DateTime.Now;
                    _TipService.UpdateTip(Tip);

                }
                else
                {
                    //var TipFound = _TipService.GetTips().Where(c => c.CustomerId == ParentId).OrderBy(c => c.ShowDate).FirstOrDefault();
                    var TipFound = _TipService.GetTips().OrderBy(c => c.ShowDate).FirstOrDefault();
                    if (TipFound != null)
                    {
                        CustomerModel.TipUrl = TipFound.TipUrl;
                        TipFound.ShowDate = DateTime.Now;
                        _TipService.UpdateTip(TipFound);
                    }

                }

                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("success", CustomerModel), Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.Message.ToString();
                ErrorLogging.LogError(ex);
                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Please try not found."), Configuration.Formatters.JsonFormatter);
            }
        }

        [Route("GetEvent")]
        [HttpGet]
        public HttpResponseMessage GetEvent(int ParentId)
        {
            try
            {
                CultureInfo culture = new CultureInfo("en-US");
                DateTime TodayDate = Convert.ToDateTime(DateTime.Now.ToString("MM/dd/yyyy"), culture);
                var Events = _EventService.GetEvents().Where(c => DateTime.ParseExact(c.EventDate.ToString("MM/dd/yyyy"), "MM/dd/yyyy", culture) >= TodayDate && c.IsActive == true && c.CustomerId == ParentId).OrderBy(c => c.EventDate).ToList();
               // var Events = _EventService.GetEvents().Where(c => Convert.ToDateTime(c.EventDate.ToString("dd/MM/yyyy"), culture) >= TodayDate && c.IsActive == true && c.CustomerId == ParentId).OrderBy(c => c.EventDate).ToList();
                Mapper.CreateMap<CommunicationApp.Entity.Event, CommunicationApp.Models.EventModel>();
                List<EventModel> EventModelList = new List<EventModel>();
                foreach (var Event in Events)
                {
                    CommunicationApp.Models.EventModel EventModel = Mapper.Map<CommunicationApp.Entity.Event, CommunicationApp.Models.EventModel>(Event);
                    EventModelList.Add(EventModel);
                }

                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("success", EventModelList), Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.Message.ToString();
                ErrorLogging.LogError(ex);
                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Please try not found."), Configuration.Formatters.JsonFormatter);
            }
        }

        [Route("GetBanner")]
        [HttpGet]
        public HttpResponseMessage GetBanner(int ParentId)
        {
            try
            {

                var Banners = _BannerService.GetBanners().Where(c => c.CustomerId == ParentId);
                Mapper.CreateMap<CommunicationApp.Entity.Banner, CommunicationApp.Web.Models.BannerModel>();
                List<BannerModel> BannerModelList = new List<BannerModel>();
                foreach (var Banner in Banners)
                {
                    CommunicationApp.Web.Models.BannerModel BannerModel = Mapper.Map<CommunicationApp.Entity.Banner, CommunicationApp.Web.Models.BannerModel>(Banner);
                    BannerModelList.Add(BannerModel);
                }

                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("success", BannerModelList), Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.Message.ToString();
                ErrorLogging.LogError(ex);
                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Please try not found."), Configuration.Formatters.JsonFormatter);
            }
        }

        [Route("GetOfficeLocation")]
        [HttpGet]
        public HttpResponseMessage GetOfficeLocation()
        {
            try
            {
                var models = new List<OfficeLocationModel>();
                var OfficeLocations = _OfficeLocationService.GetOfficeLocations();
                Mapper.CreateMap<CommunicationApp.Entity.OfficeLocation, CommunicationApp.Models.OfficeLocationModel>();
                foreach (var OfficeLocation in OfficeLocations)
                {
                    CommunicationApp.Models.OfficeLocationModel OfficeLocationModel = Mapper.Map<CommunicationApp.Entity.OfficeLocation, CommunicationApp.Models.OfficeLocationModel>(OfficeLocation);
                    models.Add(OfficeLocationModel);
                }

                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("success", models), Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.Message.ToString();
                ErrorLogging.LogError(ex);
                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Location not found."), Configuration.Formatters.JsonFormatter);
            }
        }
        [Route("GetAllCustomers")]
        [HttpGet]
        public IHttpActionResult GetAllCustomers()
        {
            var Customers = _CustomerService.GetCustomers();
            var models = new List<CustomerModel>();
            Mapper.CreateMap<CommunicationApp.Entity.Customer, CommunicationApp.Models.CustomerModel>();
            foreach (var Customer in Customers)
            {
                models.Add(Mapper.Map<CommunicationApp.Entity.Customer, CommunicationApp.Models.CustomerModel>(Customer));
            }

            return Json(models);
        }
        [Route("GetCustomerByID")]
        [HttpGet]
        public HttpResponseMessage GetCustomerByID([FromUri] int CustomerId)
        {
            try
            {
                var Customer = _CustomerService.GetCustomer(CustomerId);
                Mapper.CreateMap<CommunicationApp.Entity.Customer, CommunicationApp.Models.CustomerModel>();
                CommunicationApp.Models.CustomerModel CustomerModel = Mapper.Map<CommunicationApp.Entity.Customer, CommunicationApp.Models.CustomerModel>(Customer);
                if (CustomerModel.RecoExpireDate != null)
                {
                   CustomerModel.RiskAssessmentExpireDate = Convert.ToDateTime(CustomerModel.RiskAssessmentExpireDate).ToString("s");
                }
                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("success", CustomerModel), Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.Message.ToString();
                ErrorLogging.LogError(ex);
                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Customer not found."), Configuration.Formatters.JsonFormatter);
            }
        }

        [Route("GetAgentByName")]
        [HttpGet]
        public HttpResponseMessage GetAgentByName([FromUri] string AgentName)
        {
            try
            {
                List<Customer> CustomerLst = new List<Customer>();
                var Customers = _CustomerService.GetCustomers().Where(c => c.FirstName.ToLower().Trim().Contains(AgentName.ToLower().Trim()) && c.ParentId != null).ToList();
                if (Customers.Count == 0)
                {
                    Customers = _CustomerService.GetCustomers();
                    foreach (var Customer in Customers)
                    {
                        if (Customer.LastName != null)
                        {
                            if (Customer.LastName.ToLower().Trim() == AgentName.ToLower().Trim() && Customer.ParentId != null)
                            {

                                CustomerLst.Add(Customer);
                            }
                        }

                    }
                    Customers = CustomerLst;
                }

                // var Tests = _CustomerService.GetCustomers();//.Where(c => ( c.LastName != null && c.LastName.ToLower().Trim().IndexOf(AgentName, StringComparison.CurrentCultureIgnoreCase) > 0)).ToList();


                List<CustomerModel> CustomerModelList = new List<CustomerModel>();
                foreach (var Customer in Customers)
                {
                    Mapper.CreateMap<CommunicationApp.Entity.Customer, CommunicationApp.Models.CustomerModel>();
                    CommunicationApp.Models.CustomerModel CustomerModel = Mapper.Map<CommunicationApp.Entity.Customer, CommunicationApp.Models.CustomerModel>(Customer);
                    var Brokerage = _CustomerService.GetCustomers().Where(c => c.CustomerId == Customer.ParentId).FirstOrDefault();
                    if (Brokerage != null)
                    {
                        CustomerModel.BrokerageName = Brokerage.FirstName;
                        CustomerModel.BrokerageEmail = Brokerage.EmailId;
                        CustomerModel.BrokeragePhoto = Brokerage.PhotoPath;
                        CustomerModel.BrokeragePhoneNo = Brokerage.MobileNo;
                        var Company = _CompanyService.GetCompany(Brokerage.CompanyID);
                        if (Company != null)
                        {
                            CustomerModel.CompanyName = Company.CompanyName;
                        }
                    }
                    CustomerModelList.Add(CustomerModel);
                }


                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("success", CustomerModelList), Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.Message.ToString();
                ErrorLogging.LogError(ex);
                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Customer not found."), Configuration.Formatters.JsonFormatter);
            }

        }
        [Route("SaveCustomer")]
        [HttpPost]
        public HttpResponseMessage SaveCustomer([FromBody]CustomerModel CustomerModel)
        {
            string CustomerID = "-1";
            int UserID = 0;
            try
            {
                if (CustomerModel == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Model is empty."), Configuration.Formatters.JsonFormatter);
                }
                if ((CustomerModel.ApplicationId == null || CustomerModel.ApplicationId == "") && CustomerModel.DeviceType != "ios")
                {
                    return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Application Id is blank."), Configuration.Formatters.JsonFormatter);
                }
                if (CustomerModel.DeviceType == null || CustomerModel.DeviceType == "")
                {
                    return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Device Type is blank."), Configuration.Formatters.JsonFormatter);
                }
                else if ((CustomerModel.DeviceType != EnumValue.GetEnumDescription(EnumValue.DeviceType.Android)) && (CustomerModel.DeviceType != EnumValue.GetEnumDescription(EnumValue.DeviceType.Ios)))
                {
                    return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Device Type is incorrect."), Configuration.Formatters.JsonFormatter);
                }

                Mapper.CreateMap<CommunicationApp.Models.CustomerModel, CommunicationApp.Entity.Customer>();
                CommunicationApp.Entity.Customer Customer = Mapper.Map<CommunicationApp.Models.CustomerModel, CommunicationApp.Entity.Customer>(CustomerModel);
                Customer customerFound = _CustomerService.GetCustomers().Where(x => x.EmailId == Customer.EmailId || x.MobileNo == Customer.MobileNo).FirstOrDefault();

                if (customerFound == null)
                {
                    var TrebId = _UserService.GetUsers().Where(c => c.TrebId == CustomerModel.TrebId).FirstOrDefault();
                    if (TrebId == null)
                    {
                        string EmailId = Customer.EmailId;
                        string FirstName = CustomerModel.FirstName;

                        if (CustomerModel.FirstName == "" || CustomerModel.FirstName == null)
                        {
                            FirstName = Customer.EmailId.Split('@')[0].Trim();
                        }

                        Customer.Address = "";
                        Customer.CompanyID = 1; //There is no session in API Controller. So we will find solution in future
                        Customer.ZipCode = "";

                        //Insert User first
                        CommunicationApp.Entity.User user = new CommunicationApp.Entity.User();
                        //user.UserId =0; //New Case
                        user.FirstName = FirstName;
                        user.TrebId = Customer.TrebId;
                        user.LastName = Customer.LastName;
                        user.UserName = EmailId;
                        user.Password = SecurityFunction.EncryptString(CustomerModel.Password);
                        user.UserEmailAddress = EmailId;
                        user.CompanyID = Customer.CompanyID;
                        user.CreatedOn = DateTime.Now;
                        user.LastUpdatedOn = DateTime.Now;
                        user.IsActive = true;
                        _UserService.InsertUser(user);
                        //End : Insert User first

                        UserID = user.UserId;
                        if (user.UserId > 0)
                        {
                            //Insert User Role
                            CommunicationApp.Entity.UserRole userRole = new CommunicationApp.Entity.UserRole();
                            userRole.UserId = user.UserId;
                            userRole.RoleId = 3; //By Default set new Customer/user role id=3
                            _UserRoleService.InsertUserRole(userRole);
                            //End : Insert User Role

                            //Insert the Customer
                            Customer.FirstName = FirstName;
                            Customer.UserId = user.UserId;
                            Customer.MobileVerifyCode = CommonCls.GetNumericCode();
                            Customer.EmailVerifyCode = CommonCls.GetNumericCode();
                            Customer.MobileVerifyCode = "9999";
                            Customer.EmailVerifyCode = "9999";
                            Customer.CreatedOn = DateTime.Now;
                            Customer.LastUpdatedOn = DateTime.Now;
                            if (CustomerModel.IsEmailVerified != null)
                            {
                                Customer.IsEmailVerified = CustomerModel.IsEmailVerified;
                            }
                            else
                            {
                                Customer.IsEmailVerified = true;
                            }
                            Customer.IsEmailVerified = true;
                            if ((CustomerModel.PhotoPath != null) && (CustomerModel.PhotoPath != ""))
                            {
                                if (!CustomerModel.PhotoPath.Contains('.'))
                                {
                                    Customer.PhotoPath = SaveImage(CustomerModel.PhotoPath);
                                }
                            }
                            Customer.IsMobileVerified = false;
                            Customer.ApplicationId = CustomerModel.ApplicationId;
                            Customer.DeviceSerialNo = CustomerModel.DeviceSerialNo;
                            Customer.DeviceType = CustomerModel.DeviceType;
                            Customer.IsUpdated = false;
                            Customer.IsAvailable = true;
                            Customer.IsAppLike = false;
                            Customer.RiskAssessmentExpireDate = null;
                            Customer.IsNotificationSoundOn = true;
                            if (Customer.FirstName == null)
                            {
                                Customer.FirstName = "";
                            }
                            if (Customer.LastName == null)
                            {
                                Customer.LastName = "";
                            }
                            if (Customer.MiddleName == null)
                            {
                                Customer.MiddleName = "";
                            }
                            //save parentId
                            Customer.ParentId = CustomerModel.ParentId;
                            //End
                            _CustomerService.InsertCustomer(Customer);

                            CustomerID = Customer.CustomerId.ToString();
                            CustomerModel.CustomerId = Customer.CustomerId;

                            //For DeActivate Duplicate ApplicationId
                            var customers = _CustomerService.GetCustomers().Where(x => x.CustomerId != Customer.CustomerId && x.ApplicationId == CustomerModel.ApplicationId);
                            foreach (var custormer in customers)
                            {
                                var customer = _CustomerService.GetCustomer(custormer.CustomerId);
                                customer.ApplicationId = "";
                                customer.DeviceSerialNo = "";
                                customer.DeviceType = "";
                                //customer.IsActive = false;
                                _CustomerService.UpdateCustomer(customer);
                            }
                            //End : Insert the Customer
                            var SubjectForAdmin = "New user registration";
                            var SubjectForUser = "You have successfully registered.";
                            string AdminBody = "New user has registered and waiting for your approval.";
                            var UserBody = "Thank you for registration. You will be notified once approved.";
                            var LowerContent = "";
                            //Send Verify Code to Customer
                            if (CustomerModel.IsEmailVerified != null)
                            {
                                if (CustomerModel.IsEmailVerified == false)
                                {
                                    string UserName = Customer.FirstName + " " + Customer.MiddleName + " " + Customer.LastName;
                                    SendMailToAdmin(UserName, Customer.EmailId, Customer.EmailVerifyCode, SubjectForAdmin, AdminBody, Customer.TrebId, LowerContent);
                                    SendMailToUser(UserName, Customer.EmailId, Customer.TrebId, SubjectForUser, UserBody, "");
                                }
                            }
                            //End : Send Verify Code to Customer

                        }
                        Mapper.CreateMap<CommunicationApp.Entity.Customer, CommunicationApp.Models.CustomerModel>();
                        CustomerModel CustomerModels = Mapper.Map<CommunicationApp.Entity.Customer, CommunicationApp.Models.CustomerModel>(Customer);
                        // return Request.CreateResponse(HttpStatusCode.OK, Common.CreateMessage("success", CustomerModels), Configuration.Formatters.JsonFormatter);
                        return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("success", "You are successfully registered and your request has been sent for approval to admin. "), Configuration.Formatters.JsonFormatter);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Treb id is already exist."), Configuration.Formatters.JsonFormatter);
                    }

                }
                else
                {
                    //Mapper.CreateMap<CommunicationApp.Entity.Customer, CommunicationApp.Models.CustomerModel>();
                    //CustomerModel CustomerModels = Mapper.Map<CommunicationApp.Entity.Customer, CommunicationApp.Models.CustomerModel>(Customer);
                    if (customerFound.EmailId == CustomerModel.EmailID)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Email Id is already exist."), Configuration.Formatters.JsonFormatter);
                    }
                    else if (customerFound.MobileNo == CustomerModel.MobileNo)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Mobile No. is already exist."), Configuration.Formatters.JsonFormatter);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Mobile No. is already exist."), Configuration.Formatters.JsonFormatter);
                    }

                }
            }
            catch (Exception ex)
            {
                var UserRole = _UserRoleService.GetUserRoles().Where(x => x.UserId == UserID).FirstOrDefault();
                if (UserRole != null)
                {
                    _UserRoleService.DeleteUserRole(UserRole); // delete user role
                }
                var User = _UserService.GetUsers().Where(x => x.UserId == UserID).FirstOrDefault();
                if (User != null)
                {
                    _UserService.DeleteUser(User); // delete user 
                }
                string ErrorMsg = ex.Message.ToString();
                ErrorLogging.LogError(ex);
                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", CustomerID), Configuration.Formatters.JsonFormatter);
            }
        }
        [Route("ForgotPassword")]
        [HttpPost]
        public HttpResponseMessage ForgotPassword([FromBody]CustomerModel CustomerModel)
        {
            try
            {
                string UserName = "";
                var user = _UserService.GetUserByTrebId(CustomerModel.TrebId);

                if (user != null)
                {
                    //Send Email to User                   
                    string Password = CommonCls.GetAlphaNumericCode();
                    user.Password = SecurityFunction.EncryptString(Password);
                    _UserService.UpdateUser(user);
                    UserName = user.FirstName + " " + user.LastName;
                    CommonCls.SendMailToUser(UserName, user.UserEmailAddress, Password, 0, user.TrebId);
                    return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("success", "Password send to your email. Please check your email."), Configuration.Formatters.JsonFormatter);
                }

                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Incorrect Treb Id."), Configuration.Formatters.JsonFormatter);

                }

            }

            catch (Exception ex)
            {

                string ErrorMsg = ex.Message.ToString();
                ErrorLogging.LogError(ex);
                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Please try later."), Configuration.Formatters.JsonFormatter);
            }


        }

        [Route("DeleteCustomer/{Customerid}")]
        [HttpGet]
        public HttpResponseMessage DeleteCustomer(int CustomerId)
        {
            try
            {
                //Delete from Customer, It will delete from user, user role & Customers
                var Customer = _CustomerService.GetCustomer(CustomerId);

                if (Customer != null)
                {
                    var UserRole = _UserRoleService.GetUserRoles().Where(x => x.UserId == Customer.UserId).FirstOrDefault();
                    if (UserRole != null)
                    {
                        _UserRoleService.DeleteUserRole(UserRole); // delete user role 
                    }
                    _CustomerService.DeleteCustomer(Customer); // delete Customer

                    var User = _UserService.GetUsers().Where(x => x.UserId == Customer.UserId).FirstOrDefault();
                    if (User != null)
                    {
                        _UserService.DeleteUser(User); // delete user 
                    }
                    return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("success", "Customer deleted successfully."), Configuration.Formatters.JsonFormatter);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "error", Configuration.Formatters.JsonFormatter);
                }

            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.Message.ToString();
                ErrorLogging.LogError(ex);
                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Please try later."), Configuration.Formatters.JsonFormatter);
            }
        }
        [Route("ValidateMobileCode")]
        [HttpPost]
        public HttpResponseMessage ValidateMobileCode([FromBody] CustomerMobileVerifyModel CustomerMobileVerify)
        {
            try
            {
                CustomerResponseModel CustomerResponse = new CustomerResponseModel();
                var Customer = _CustomerService.GetCustomer(CustomerMobileVerify.CustomerId);
                if (Customer != null)
                {
                    if (Customer.MobileVerifyCode == CustomerMobileVerify.MobileVerifyCode)
                    {
                        Customer.IsMobileVerified = true;
                        Customer.IsActive = true;
                        _CustomerService.UpdateCustomer(Customer); //Update IsMobileVerified & IsActive  Operation
                        //Set Values for Response
                        CustomerResponse.CustomerId = Customer.CustomerId;
                        CustomerResponse.MobileVerifyCode = Customer.MobileVerifyCode;
                        CustomerResponse.IsMobileVerified = Customer.IsMobileVerified;
                        CustomerResponse.MobileNo = Customer.MobileNo;
                        CustomerResponse.Latitude = Customer.Latitude;
                        CustomerResponse.Longitude = Customer.Longitude;

                        return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("success", CustomerResponse), Configuration.Formatters.JsonFormatter);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Please enter correct code."), Configuration.Formatters.JsonFormatter);
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "User does not exist."), Configuration.Formatters.JsonFormatter);
                }
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.Message.ToString();
                ErrorLogging.LogError(ex);
                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Please try later."), Configuration.Formatters.JsonFormatter);
            }
        }

        [Route("ValidateEmailCode")]
        [HttpPost]
        public HttpResponseMessage ValidateEmailCode([FromBody]ValidateEmailCodeModel ValidateEmailCodeModel)
        {
            try
            {
                var Customer = _CustomerService.GetCustomer(ValidateEmailCodeModel.CustomerId);
                if (Customer != null)
                {
                    if (Customer.EmailVerifyCode == ValidateEmailCodeModel.EmailVerifyCode)
                    {
                        Customer.IsEmailVerified = true;
                        Customer.IsActive = true;

                        _CustomerService.UpdateCustomer(Customer); //Update IsMobileVerified & IsActive  Operation


                        return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("success", "Email is verified."), Configuration.Formatters.JsonFormatter);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Please enter correct code."), Configuration.Formatters.JsonFormatter);
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Please try later."), Configuration.Formatters.JsonFormatter);
                }
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.Message.ToString();
                ErrorLogging.LogError(ex);
                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Please try later."), Configuration.Formatters.JsonFormatter);
            }
        }

        [Route("SetLatLongLocation")]
        [HttpPost]
        public HttpResponseMessage SetLatLongLocation([FromBody]UserCustomerModel userCustomer)
        {
            try
            {
                CommunicationApp.Entity.Customer Customer = _CustomerService.GetCustomers().Where(x => x.CustomerId == userCustomer.CustomerId).FirstOrDefault();
                if ((userCustomer.ApplicationId != null) && (userCustomer.ApplicationId != ""))
                {
                    Customer.ApplicationId = userCustomer.ApplicationId;
                }
                if ((userCustomer.Latitude != null) && (userCustomer.Latitude != 0) && (userCustomer.Longitude != null) && (userCustomer.Longitude != 0))
                {
                    Customer.Latitude = userCustomer.Latitude;
                    Customer.Longitude = userCustomer.Longitude;
                }
                _CustomerService.UpdateCustomer(Customer);

                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("success", "User updated successfully."), Configuration.Formatters.JsonFormatter);

            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.Message.ToString();
                ErrorLogging.LogError(ex);
                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Please try later."), Configuration.Formatters.JsonFormatter);
            }
        }
        [Route("UpdateProfile")]
        [HttpPost]
        public HttpResponseMessage UpdateProfile([FromBody]CustomerModel userCustomer)
        {
            try
            {
                CommunicationApp.Entity.Customer Customer = _CustomerService.GetCustomers().Where(x => x.CustomerId == userCustomer.CustomerId).FirstOrDefault();

                if (Customer != null)
                {
                    Customer.FirstName = userCustomer.FirstName;
                    Customer.LastName = userCustomer.LastName;
                    Customer.MiddleName = userCustomer.MiddleName;
                    Customer.CityName = userCustomer.CityName;
                    Customer.EmailId = userCustomer.EmailID;
                    Customer.MobileNo = userCustomer.MobileNo;
                    Customer.WebsiteUrl = userCustomer.WebsiteUrl;
                    Customer.RecoNumber = userCustomer.RecoNumber;
                    Customer.RecoExpireDate = userCustomer.RecoExpireDate;
                    //Customer.RiskAssessmentExpireDate =ConvertDate( userCustomer.RiskAssessmentExpireDate);
                    Customer.DOB = userCustomer.DOB;
                    Customer.OpenHouseAvalibility = userCustomer.OpenHouseAvalibility;
                    //
                    if (userCustomer.PhotoPath != null)
                    {
                        Customer.UpdateStatus = false;
                        if (!userCustomer.PhotoPath.Contains('.'))
                        {
                            if ((userCustomer.PhotoPath != "") && (Customer.PhotoPath != "") && (Customer.PhotoPath != null))
                            {
                                DeleteImage(Customer.PhotoPath);
                                Customer.PhotoPath = SaveImage(userCustomer.PhotoPath);
                            }
                            else if ((userCustomer.PhotoPath != "") && (Customer.PhotoPath == "" || Customer.PhotoPath != null))
                            {

                                Customer.PhotoPath = SaveImage(userCustomer.PhotoPath);
                            }


                        }


                    }
                    Customer.IsAvailable = true;
                    Customer.LastUpdatedOn = DateTime.Now;
                    Customer.IsUpdated = true;
                    _CustomerService.UpdateCustomer(Customer);

                    string FirstName = Customer.FirstName + " " + Customer.MiddleName + " " + Customer.LastName;
                    var SubjectForAdmin = "Profile update Request";
                    var SubjectForUser = "Profile update request - Pending";
                    string AdminBody = "";
                    var UserBody = "Your profile will be updated once approved.";
                    var LowerContent = "Updated 'Profile', kindly review & approve.";
                    if (Customer.UpdateStatus == false)
                    {
                        //Send mail
                        SendMailToAdmin(FirstName, Customer.EmailId, Customer.EmailVerifyCode, SubjectForAdmin, AdminBody, Customer.TrebId, LowerContent);
                        SendMailToUser(FirstName, Customer.EmailId, Customer.TrebId, SubjectForUser, UserBody, "");
                        //End
                    }


                    var User = _UserService.GetUserById(Customer.UserId);
                    if (userCustomer.Password != null && userCustomer.Password != "")
                    {
                        User.Password = SecurityFunction.EncryptString(userCustomer.Password);

                    }
                    User.UserEmailAddress = userCustomer.EmailID;

                    _UserService.UpdateUser(User);
                    Mapper.CreateMap<CommunicationApp.Entity.Customer, CommunicationApp.Models.CustomerModel>();
                    CustomerModel CustomerModels = Mapper.Map<CommunicationApp.Entity.Customer, CommunicationApp.Models.CustomerModel>(Customer);
                    CustomerModels.Password = User.Password;
                    CustomerModels.PhotoPath = Customer.PhotoPath;
                    return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("success", CustomerModels), Configuration.Formatters.JsonFormatter);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("success", "RideId is not found."), Configuration.Formatters.JsonFormatter);
                }
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.Message.ToString();
                ErrorLogging.LogError(ex);
                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Please try later."), Configuration.Formatters.JsonFormatter);
            }
        }
        [Route("ValidateUserCustomer")]
        [HttpPost]
        public HttpResponseMessage ValidateUserCustomer([FromBody]UserCustomerModel userCustomer)
        {
            string UserID = "-1";
            try
            {
                if ((userCustomer.ApplicationId == null || userCustomer.ApplicationId == "") && userCustomer.DeviceType != "ios")
                {
                    return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Application Id is blank."), Configuration.Formatters.JsonFormatter);
                }
                if (userCustomer.DeviceType == null || userCustomer.DeviceType == "")
                {
                    return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Device Type is blank."), Configuration.Formatters.JsonFormatter);
                }
                else if ((userCustomer.DeviceType != EnumValue.GetEnumDescription(EnumValue.DeviceType.Android)) && (userCustomer.DeviceType != EnumValue.GetEnumDescription(EnumValue.DeviceType.Ios)))
                {
                    return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Device Type is incorrect."), Configuration.Formatters.JsonFormatter);
                }

                CommunicationApp.Entity.User users = null;


                users = _UserService.ValidateUserByTrebId(userCustomer.TrebId, userCustomer.Password);
                var IsTrebFound = _UserService.GetUsers().Where(c => c.TrebId == userCustomer.TrebId).ToList();
                if (IsTrebFound.Count == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "TrebId does not exist."), Configuration.Formatters.JsonFormatter);
                }
                if (users != null)
                {

                    UserID = users.UserId.ToString();
                    CommunicationApp.Entity.Customer Customer = _CustomerService.GetCustomers().Where(x => x.UserId == users.UserId).FirstOrDefault();
                    if (Customer != null)
                    {
                        if (Customer.IsActive == false)
                        {
                            return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Your treb id yet not approved by admin."), Configuration.Formatters.JsonFormatter);
                        }
                        if ((userCustomer.DeviceSerialNo != null) && (userCustomer.DeviceSerialNo != ""))
                        {
                            Customer.DeviceSerialNo = userCustomer.DeviceSerialNo; //Reset the Value
                        }
                        if (userCustomer.ApplicationId != null && userCustomer.ApplicationId != "")
                        {
                            Customer.ApplicationId = userCustomer.ApplicationId;
                        }
                        if (userCustomer.Latitude != null && userCustomer.Latitude.ToString() != "")
                        {
                            Customer.Latitude = userCustomer.Latitude; //Reset the Value
                        }
                        if (userCustomer.Longitude != null && userCustomer.Latitude.ToString() != "")
                        {
                            Customer.Longitude = userCustomer.Longitude; //Reset the Value
                        }
                        if (userCustomer.DeviceType != null && userCustomer.DeviceType != "")
                        {
                            Customer.DeviceType = userCustomer.DeviceType;
                        }

                        //for off notification sound.
                        Customer.IsNotificationSoundOn = true;
                        Customer.IsAvailable = true;
                        Customer.LastUpdatedOn = DateTime.Now;
                        //End

                        _CustomerService.UpdateCustomer(Customer);
                    }


                    //End : Insert/Update in Customer Device
                    //Update all applicationId
                    var Customerss = _CustomerService.GetCustomers().Where(c => c.CustomerId != Customer.CustomerId && c.ApplicationId == Customer.ApplicationId).ToList();
                    foreach (var Customers in Customerss)
                    {
                        Customers.ApplicationId = "";
                        Customers.DeviceSerialNo = "";
                        Customers.DeviceType = "";
                        _CustomerService.UpdateCustomer(Customers);
                    }
                    //End


                    Mapper.CreateMap<CommunicationApp.Entity.Customer, CommunicationApp.Models.CustomerModel>();
                    CustomerModel CustomerModel = Mapper.Map<CommunicationApp.Entity.Customer, CommunicationApp.Models.CustomerModel>(Customer);

                    var Admin = _CustomerService.GetCustomer(Convert.ToInt32(Customer.ParentId));
                    if (Admin != null)
                    {
                        var Company = _CompanyService.GetCompany(Convert.ToInt32(Admin.CompanyID));
                        if (Company != null)
                        {
                            CustomerModel.CompanyName = Company.CompanyName;
                            CustomerModel.Logo = Company.LogoPath;
                        }
                    }
                    if (CustomerModel.RiskAssessmentExpireDate != null && CustomerModel.RiskAssessmentExpireDate != "" && CustomerModel.RiskAssessmentExpireDate != "1")
                    {

                        //CustomerModel.RiskAssessmentExpireDate =  CustomerModel.RiskAssessmentExpireDate ;
                        CustomerModel.RiskAssessmentExpireDate = Customer.RiskAssessmentExpireDate.Split(' ').First();
                        CustomerModel.RiskExpiryDateNew = Convert.ToDateTime(Customer.RiskAssessmentExpireDate.Split(' ').First());

                    }

                    else
                    {
                        string date = DateTime.Now.Date.ToString().Split('+').First();
                        CustomerModel.RiskExpiryDateNew = Convert.ToDateTime(date);
                        CustomerModel.RiskAssessmentExpireDate = DateTime.Now.ToString().Split(' ').First();
                    }
                    CustomerModel.RiskAssessmentExpireDate = Customer.RiskAssessmentExpireDate.Split(' ').First();
                    CustomerModel.RiskExpiryDateNew = Convert.ToDateTime(Customer.RiskAssessmentExpireDate.Split(' ').First());
                   
                    return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("success", CustomerModel), Configuration.Formatters.JsonFormatter);
                }
                else
                {
                    if ((users == null) && (userCustomer.Flag == "facebook"))
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Email Id not found."), Configuration.Formatters.JsonFormatter);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Incorrect TrebId or Password."), Configuration.Formatters.JsonFormatter);
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.Message.ToString();
                ErrorLogging.LogError(ex);
                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Please try later."), Configuration.Formatters.JsonFormatter);
            }
        }
        [Route("TestMail")]
        [HttpGet]
        public HttpResponseMessage TestMail()
        {

            //string UserName = "Maninder";
            //string EmailAddress = "maninder.singh.2114@gmail.com";
            //string TrebId = "123456";
            //string PropertyType = "New Hot Listing";
            //string Subject = "New Property Register.";
            //int MessageType = 2;
            //string EmailVerifyCode = "";
            //string subject = "";
            //var SubjectForAdmin = "Approve user updated record.";
            //var SubjectForUser = "Your record pending for approval.";
            //var Body = "User waiting for your approval.";
            //ForgetPassword(UserName, EmailAddress, TrebId, MessageType, TrebId);
            // SendMailToAdmin(UserName, EmailAddress, EmailVerifyCode, SubjectForAdmin, Body, TrebId);
            //  SendMailToUser(UserName, EmailAddress, TrebId, subject, Body);
            return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Please try later."), Configuration.Formatters.JsonFormatter);
        }
        [Route("SignOut")]
        [HttpGet]
        public HttpResponseMessage SignOut([FromUri] int CustomerId)
        {
            try
            {
                if (CustomerId != 0)
                {
                    var Customer = _CustomerService.GetCustomer(CustomerId);
                    if (Customer != null)
                    {
                        Customer.ApplicationId = "";
                        Customer.DeviceSerialNo = "";
                        Customer.DeviceType = "";
                        Customer.IsAvailable = false;
                        Customer.LastUpdatedOn = DateTime.Now;
                        //for off the notification sound in ios.
                        Customer.IsNotificationSoundOn = false;
                        //End
                        _CustomerService.UpdateCustomer(Customer);
                        return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("success", "User sign out successfully."), Configuration.Formatters.JsonFormatter);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "User not found."), Configuration.Formatters.JsonFormatter);
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Customer id not found."), Configuration.Formatters.JsonFormatter);
                }
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Please try later."), Configuration.Formatters.JsonFormatter);
            }


        }
        [Route("GetPdf")]
        [HttpGet]
        public HttpResponseMessage GetPdf(int FormNo)
        {
            try
            {


                string URL = CommonCls.GetURL() + "/FdfForms/RiskAssessmentForm-" + FormNo + ".pdf";
                if (URL != "")
                {
                    return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("success", URL), Configuration.Formatters.JsonFormatter);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "No pdf available."), Configuration.Formatters.JsonFormatter);
                }
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Please try later."), Configuration.Formatters.JsonFormatter);
            }


        }
        [Route("SavePdf")]
        [HttpGet]
        public HttpResponseMessage SavePdf([FromBody] CustomerResponseModel PdfModel)
        {
            try
            {
                if (PdfModel.CustomerId != 0)
                {

                    var URL = SaveFile(PdfModel.File, "FdfForms", ".pdf", Convert.ToInt32(PdfModel.CustomerId));
                    PdfForm PdfForm = new PdfForm();
                    PdfForm.Url = URL;
                    PdfForm.CustomerId = PdfModel.CustomerId;
                    PdfForm.IsActive = true;
                    PdfForm.CreatedOn = DateTime.Now;
                    PdfForm.LastUpdatedon = DateTime.Now;
                    _PdfFormService.InsertPdfForm(PdfForm);
                    return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("success", URL), Configuration.Formatters.JsonFormatter);
                }
                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "No pdf available."), Configuration.Formatters.JsonFormatter);

            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Please try later."), Configuration.Formatters.JsonFormatter);
            }

        }

        [Route("UserActiveOrNot")]
        [HttpGet]
        public HttpResponseMessage UserActiveOrNot([FromUri] int CustomerId)
        {
            try
            {
                if (CustomerId != 0)
                {
                    List<string> Status = new List<string>();

                    var Customer = _CustomerService.GetCustomer(CustomerId);
                    if (Customer != null)
                    {
                        if (Customer.IsActive == true)
                        {
                            Status.Add("Active");
                            Status.Add("");
                            return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("Success", Status), Configuration.Formatters.JsonFormatter);
                        }
                        else
                        {
                            Status.Add("DeActive");
                            Status.Add("Your account is currently deactivated by admin.");
                            return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("Success", Status), Configuration.Formatters.JsonFormatter);
                        }


                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "User not found."), Configuration.Formatters.JsonFormatter);
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Customer id not found."), Configuration.Formatters.JsonFormatter);
                }
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Please try later."), Configuration.Formatters.JsonFormatter);
            }


        }


        [Route("UpdateApplicationId")]
        [HttpGet]
        public HttpResponseMessage UpdateApplicationId([FromUri] int CustomerId, string ApplicationId,string DeviceType)
        {
            try
            {
                if (CustomerId != 0 && ApplicationId != null)
                {
                    if (DeviceType == null || DeviceType == "")
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Device Type is blank."), Configuration.Formatters.JsonFormatter);
                    }
                    else if ((DeviceType != EnumValue.GetEnumDescription(EnumValue.DeviceType.Android)) && (DeviceType != EnumValue.GetEnumDescription(EnumValue.DeviceType.Ios)))
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Device Type is incorrect."), Configuration.Formatters.JsonFormatter);
                    }
                    var Customer = _CustomerService.GetCustomer(CustomerId);
                    if (Customer != null)
                    {
                        Customer.DeviceType = DeviceType;
                        Customer.ApplicationId = ApplicationId;
                        _CustomerService.UpdateCustomer(Customer);
                        return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("Success", "Application id successfully updated."), Configuration.Formatters.JsonFormatter);
                    }
                    else
                    {

                        return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("Error", "Customer/Device id is blank"), Configuration.Formatters.JsonFormatter);
                    }


                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "User not found."), Configuration.Formatters.JsonFormatter);
                }

            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Please try later."), Configuration.Formatters.JsonFormatter);
            }


        }


        [Route("ReadNotification")]
        [HttpGet]
        public HttpResponseMessage ReadNotification([FromUri] int CustomerId)
        {
            try
            {
                if (CustomerId != 0 )
                {
                    
                    var Customer = _CustomerService.GetCustomer(CustomerId);
                    if (Customer != null)
                    {
                        var notifications = _Notification.GetNotifications().Where(c => c.NotificationSendTo == CustomerId && c.IsRead == false).ToList();
                        foreach (var item in notifications)
                        {
                            item.IsRead = true;
                            _Notification.UpdateNotification(item);
                        }
                        
                        return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("Success", "Notification successfully read."), Configuration.Formatters.JsonFormatter);
                    }
                    else
                    {

                        return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("Error", "Customer id is blank"), Configuration.Formatters.JsonFormatter);
                    }


                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "User not found."), Configuration.Formatters.JsonFormatter);
                }

            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Please try later."), Configuration.Formatters.JsonFormatter);
            }


        }


        [Route("GetNotificationCount")]
        [HttpGet]
        public HttpResponseMessage GetNotificationCount([FromUri] int CustomerId)
        {
            try
            {
                if (CustomerId != 0)
                {
                    List<NotificationCount> list = new List<NotificationCount>();
                    var Customer = _CustomerService.GetCustomer(CustomerId);
                    if (Customer != null)
                    {
                        var notifications = _Notification.GetNotifications().Where(c=>c.NotificationSendTo==CustomerId && c.Flag!=null)
                           .GroupBy(n => n.Flag)
    .Select(g => new NotificationCount
    {
        Flag = g.Key,
        Count = g.Where(n => n.IsRead==false ).Count(),
    }).ToList();
                        
                        
                        return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("Done", notifications), Configuration.Formatters.JsonFormatter);
                    }
                    else
                    {

                        return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("Error", "Customer id is blank"), Configuration.Formatters.JsonFormatter);
                    }


                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "User not found."), Configuration.Formatters.JsonFormatter);
                }

            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Please try later."), Configuration.Formatters.JsonFormatter);
            }


        }

        [Route("GoToBrokerageForm")]
        [HttpGet]
        public HttpResponseMessage GoToBrokerageForm([FromUri] int CustomerId)
        {
            try
            {
                if (CustomerId != 0)
                {
                    var Url = "http://169.45.133.92:85/Brokerage/Brokerage?CustomerId=" + CustomerId;
                    var Customer = _CustomerService.GetCustomer(CustomerId);
                    if (Customer != null)
                    {

                        var subject = "Brokerage form link";
                        SendMailForBrokerage(Customer.FirstName, Customer.EmailId, Url, subject);
                    }

                    return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("Success", "Link sent to your email. please check email."), Configuration.Formatters.JsonFormatter);

                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Customer id not found."), Configuration.Formatters.JsonFormatter);
                }
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Please try later."), Configuration.Formatters.JsonFormatter);
            }


        }
        [Route("IsAppLike")]
        [HttpGet]
        public HttpResponseMessage IsAppLike([FromUri] int CustomerId)
        {
            try
            {
                if (CustomerId != 0)
                {

                    var Customer = _CustomerService.GetCustomer(CustomerId);
                    if (Customer != null)
                    {

                        Customer.IsAppLike = true;
                        _CustomerService.UpdateCustomer(Customer);

                    }

                    return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("Success", "You successfully liked this app."), Configuration.Formatters.JsonFormatter);

                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Customer id not found."), Configuration.Formatters.JsonFormatter);
                }
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Please try later."), Configuration.Formatters.JsonFormatter);
            }


        }



        [Route("GetRiskAssessmentExpiryDate")]
        [HttpGet]
        public HttpResponseMessage GetRiskAssessmentExpiryDate([FromUri] int CustomerId, string DeviceType)
        {
            try
            {
                if (DeviceType == null || DeviceType == "")
                {
                    return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Device Type is blank."), Configuration.Formatters.JsonFormatter);
                }
                else if ((DeviceType != EnumValue.GetEnumDescription(EnumValue.DeviceType.Android)) && (DeviceType != EnumValue.GetEnumDescription(EnumValue.DeviceType.Ios)))
                {
                    return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Device Type is incorrect."), Configuration.Formatters.JsonFormatter);
                }
                if (CustomerId != 0)
                {

                    var Customer = _CustomerService.GetCustomer(CustomerId);
                    if (Customer != null)
                    {
                        CustomerRiskExpiryModel obj = new CustomerRiskExpiryModel();
                        obj.CustomerId = CustomerId;
                        if (Customer.RiskAssessmentExpireDate != null)
                        {
                            obj.RiskExpiryDateNew = Convert.ToDateTime(Customer.RiskAssessmentExpireDate.Split(' ').First());
                            obj.RiskAssessmentExpireDate = Customer.RiskAssessmentExpireDate.Split(' ').First();
                        }
                        else
                        {
                            string date = DateTime.Now.Date.ToString().Split('+').First();
                            obj.RiskExpiryDateNew = Convert.ToDateTime(date);
                            obj.RiskAssessmentExpireDate = DateTime.Now.ToString();
                        }
                        if (DeviceType == EnumValue.GetEnumDescription(EnumValue.DeviceType.Android))
                        {
                            return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("Success", obj), Configuration.Formatters.JsonFormatter);
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("Successfull", obj), Configuration.Formatters.JsonFormatter);
                        }


                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Customer id not found."), Configuration.Formatters.JsonFormatter);
                    }


                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Customer id not found."), Configuration.Formatters.JsonFormatter);
                }
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Please try later."), Configuration.Formatters.JsonFormatter);
            }


        }
       




        public string SaveImage(string Base64String)
        {
            string fileName = Guid.NewGuid() + ".png";
            Image image = CommonCls.Base64ToImage(Base64String);
            var subPath = HttpContext.Current.Server.MapPath("~/CustomerPhoto");
            var path = Path.Combine(subPath, fileName);
            image.Save(path, System.Drawing.Imaging.ImageFormat.Png);

            string URL = CommonCls.GetURL() + "/CustomerPhoto/" + fileName;
            return URL;
        }
        public void DeleteImage(string filePath)
        {
            try
            {
                var uri = new Uri(filePath);
                var fileName = Path.GetFileName(uri.AbsolutePath);
                var subPath = HttpContext.Current.Server.MapPath("~/CustomerPhoto");
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

        public string SaveFile(string Base64String, string FolderName, string FileExt, int CustomerId)
        {
            string fileName = Guid.NewGuid() + "_" + CustomerId.ToString() + FileExt;
            var subPath = HttpContext.Current.Server.MapPath("~/" + FolderName);
            var path = Path.Combine(subPath, fileName);
            byte[] imageBytes = Convert.FromBase64String(Base64String);
            File.WriteAllBytes(path, imageBytes);
            string URL = CommonCls.GetURL() + "/" + FolderName + "/" + fileName;

            return URL;
        }
        public void SendMailToAdmin(string UserName, string EmailAddress, string EmailVerifyCode, string subject, string Body, string TrebId, string LowerContent)
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
                mail.To.Add(new MailAddress(ToEmailID));
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
                msgbody += "<p style='float:left;width:100%; font-size:14px; font-family:arial; line-height:25px; margin:0;'>" + LowerContent + "";
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
        public void SendMailToUser(string UserName, string EmailAddress, String TrebId, string subject, string Body, string EmailRecieverName)
        {
            try
            {
                // Send mail.
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
                mail.Subject = subject;
                //string LogoPath = Common.GetURL() + "/images/logo.png";

                string msgbody = "";
                msgbody += "";
                msgbody += " <div>";
                msgbody += " <div style='float:left;width:100%;'>";
                msgbody += "<h2 style='float:left; width:100%; font-size:16px; font-family:arial; margin:0 0 10px;'>Dear " + EmailRecieverName == "" ? UserName : EmailRecieverName + ",</h2>";

                msgbody += " </div>";
                msgbody += "<p style='float:left;width:100%; font-size:14px; font-family:arial; line-height:25px; margin:0;'>" + Body + "</p>";//Thank you for registration. You will be notified once approved.
                msgbody += "<span style='float:left; font-size:15px; font-family:arial; margin:12px 0 0 0; width:100%;'>Team</span>";
                msgbody += "<div style='width:auto; float:left;'>";
                msgbody += " <p style='font-family:arial; font-size:12px; font-weight:bold; margin:0px; padding:0px;'>Call: 416 844 5725 | 647 977 3268 </p>";
                msgbody += " <p style='font-family:arial; font-size:12px; font-weight:bold; margin:0px; padding:0px;'> Fax: 647 497 5646 </p>";
                msgbody += " <a style='color:red; width:100%; font-size:13px; float:left; margin:5px 0 0 0px; text-decoration:none; font-family:arial;' href='www.http://www.only4agents.com'>Web:www.only4agents.com</a>";
                msgbody += " <a style='color:red; width:100%;font-size:13px; float:left; margin:5px 0 0 0px; text-decoration:none; font-family:arial;' href='info@only4agents.com'>Email: info@only4agents.com</a>";

                msgbody += " </div>";
                msgbody += "<div style='float:left; width:100%; margin:5px 0;'>";
                msgbody += "<img style='float:left; width:500px;' src='" + Logourl + "' />";
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


        public void SendMailForBrokerage(string UserName, string EmailAddress, String Url, string subject)
        {
            try
            {
                // Send mail.
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
                mail.Subject = subject;
                //string LogoPath = Common.GetURL() + "/images/logo.png";
                string msgbody = "";
                msgbody += "";
                msgbody += " <div>";
                msgbody += " <div style='float:left;width:100%;'>";
                msgbody += "<div style='float:left;width:100%; margin:4px 0;'><p style='float:left; font-size:14px; font-family:arial; line-height:25px; margin:0 11px 0 0;'>Click on this link for go to brokerage form :</p> <span style='float:left; font-size:16px; font-family:arial; margin:5px 0 0 0;'>" + Url + "</span>";
                msgbody += " </div>";
                msgbody += "<span style='float:left; font-size:15px; font-family:arial; margin:12px 0 0 0; width:100%;'>Team</span>";
                msgbody += "<div style='width:auto; float:left;'>";
                msgbody += " <p style='font-family:arial; font-size:12px; font-weight:bold; margin:0px; padding:0px;'>Call: 416 844 5725 | 647 977 3268 </p>";
                msgbody += " <p style='font-family:arial; font-size:12px; font-weight:bold; margin:0px; padding:0px;'> Fax: 647 497 5646 </p>";
                msgbody += " <a style='color:red; width:100%; font-size:13px; float:left; margin:5px 0 0 0px; text-decoration:none; font-family:arial;' href='www.http://www.only4agents.com'>Web:www.only4agents.com</a>";
                msgbody += " <a style='color:red; width:100%;font-size:13px; float:left; margin:5px 0 0 0px; text-decoration:none; font-family:arial;' href='info@only4agents.com'>Email: info@only4agents.com</a>";

                msgbody += " </div>";
                msgbody += "<div style='float:left; width:100%; margin:5px 0;'>";
                msgbody += "<img style='float:left; width:500px;' src='" + Logourl + "' />";
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