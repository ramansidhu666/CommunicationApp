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

namespace CommunicationApp.Web.Controllers
{
    public class PropertyController : BaseController
    {
        public IUserService _UserService { get; set; }
        public IUserRoleService _UserRoleService { get; set; }
        public IPropertyService _PropertyService { get; set; }
        public IPropertyImageService _PropertyImageService { get; set; }
        public IAgentService _AgentService { get; set; }
        //public ICustomerService _CustomerService { get; set; }
        public IFeedBackService _FeedBackService { get; set; }
        public ICityService _CityService { get; set; }
        public IStateService _StateService { get; set; }
        public ICountryService _CountryService { get; set; }
        public IRoleService _RoleService { get; set; }
        public IFormService _FormService { get; set; }
        public IRoleDetailService _RoleDetailService { get; set; }
        public ITipService _TipService { get; set; }
        public IEventService _EventService { get; set; }
        public IOfficeLocationService _OfficeLocationService { get; set; }
        public IEventCustomerService _EventCustomerService { get; set; }
        public IViewsService _ViewService { get; set; }

        public PropertyController(IOfficeLocationService OfficeLocationService, ICustomerService CustomerService, IEventCustomerService EventCustomerService, IEventService EventService, ITipService TipService, IRoleService RoleService, IFormService FormService, IRoleDetailService RoleDetailService, IRoleService _RoleService, IUserRoleService UserroleService, ICountryService CountryService, IStateService StateService, ICityService CityService, IFeedBackService FeedBackService, IAgentService AgentService, IPropertyImageService PropertyImageService, IPropertyService PropertyService, IUserService UserService, IUserRoleService UserRoleService, IViewsService ViewService)
            : base(CustomerService, UserService, RoleService, FormService, RoleDetailService, UserroleService)
        {
            this._PropertyService = PropertyService;
            this._UserService = UserService;
            this._UserRoleService = UserRoleService;
            this._PropertyImageService = PropertyImageService;
            this._AgentService = AgentService;
            this._CustomerService = CustomerService;
            this._FeedBackService = FeedBackService;
            this._CountryService = CountryService;
            this._StateService = StateService;
            this._CityService = CityService;
            this._TipService = TipService;
            this._EventService = EventService;
            this._EventCustomerService = EventCustomerService;
            this._OfficeLocationService = OfficeLocationService;//
            this._ViewService = ViewService;//
        }
        //
        // GET: /Property/
        public ActionResult Index()
        {
            return View();
        }
        private void CheckPermission()
        {
            RoleDetailModel roleDetail = UserPermission("property");
            TempData["Create"] = roleDetail.IsCreate;
            TempData["Delete"] = roleDetail.IsDelete;
            TempData["Detail"] = roleDetail.IsDetail;
            TempData["Edit"] = roleDetail.IsEdit;
            TempData["View"] = roleDetail.IsView;
        }
        public ActionResult PropertyList()
        {
            var CustomerId = Convert.ToInt32(Session["CustomerId"]);
            UserPermissionAction("property", RoleAction.view.ToString());
            CheckPermission();
            TempData["ShowMessage"] = "";
            TempData["MessageBody"] = "";
            List<PropertyModel> PropertyModelList = new List<PropertyModel>();
            var Properties = _PropertyService.GetPropertys().OrderByDescending(c => c.PropertyId).ToList();
            if (CustomerId != 0)
            {
                Properties = Properties.Where(c => c.Customers.ParentId == CustomerId).OrderByDescending(c => c.PropertyId).ToList();
            }
           
            PropertyModel PropertyModel = new Web.Models.PropertyModel();
            Mapper.CreateMap<CommunicationApp.Entity.Property, CommunicationApp.Web.Models.PropertyModel>();
            foreach (var Property in Properties)
            {
                CommunicationApp.Web.Models.PropertyModel Propertymodel = Mapper.Map<CommunicationApp.Entity.Property, CommunicationApp.Web.Models.PropertyModel>(Property);
                var PropertyImages = _PropertyImageService.GetPropertyImages().Where(c => c.PropertyId == Property.PropertyId).ToList();
                PropertyImages PropertyImg = new PropertyImages();
                List<PropertyImages> PropertyImagesList = new List<PropertyImages>();
                foreach (var PropertyImage in PropertyImages)
                {
                    PropertyImg.imagelist = PropertyImage.ImagePath;
                    PropertyImagesList.Add(PropertyImg);
                }
                Propertymodel.PropertyPhotolist = PropertyImagesList;
                PropertyModelList.Add(Propertymodel);
            }
            SessionBatches();
            ViewBag.Title = "Welcome";
            //return RedirectToAction("~/Home/WelcomeHome.cshtml", PropertyModelList);
            return RedirectToAction("WelcomeHome", "Home", PropertyModelList);

        }
        public ActionResult AgentList(string UserName, string StartDate, string EndDate, string TrebId, string MobileNo)
        {
            var CustomerId = Convert.ToInt32(Session["CustomerId"]);
            UserPermissionAction("property", RoleAction.view.ToString());
            CheckPermission();
            List<AgentModel> AgentModelLists = new List<AgentModel>();
            AgentListModel AgentModelList = new AgentListModel();
            AgentModelList.UserName = UserName;
            AgentModelList.StartDate = StartDate;
            AgentModelList.EndDate = EndDate;
            AgentModelList._AgentListModel = AgentModelLists;

            if (!string.IsNullOrEmpty(StartDate))
            {
                if (CheckDate(StartDate) == false)
                {
                    TempData["ShowMessage"] = "error";
                    TempData["MessageBody"] = "Please select a valid From Date.";
                    return View(AgentModelList);
                }
            }
            if (!string.IsNullOrEmpty(EndDate))
            {
                if (CheckDate(EndDate) == false)
                {
                    TempData["ShowMessage"] = "error";
                    TempData["MessageBody"] = "Please select a valid To Date.";
                    return View(AgentModelList);
                }
            }

            if (!string.IsNullOrEmpty(StartDate) && !string.IsNullOrEmpty(EndDate))
            {
                if (Convert.ToDateTime(Convert.ToDateTime(StartDate).ToString("dd/MM/yyyy")) > Convert.ToDateTime(Convert.ToDateTime(EndDate).ToString("dd/MM/yyyy")))
                {
                    TempData["ShowMessage"] = "error";
                    TempData["MessageBody"] = "Please select a valid date range.";
                    return View(AgentModelList);
                }

            }

            List<PropertyModel> PropertyModelList = new List<PropertyModel>();
            CommonClass CommonClass = new CommonClass();
            string QStr = "";
            DataTable dt = new DataTable();
            QStr = @"SELECT [AgentId],[Agent].[CompanyId],[Agent].[CustomerId],[City],[MLS],[Date],[FromTime],[ToTime],[Date2],[FromTime2],
                    [ToTime2],[Price],[AgentStatusId],[Comments],[Agent].[CreatedOn],[Agent].[LastUpdatedon],[Agent].[IsActive], 
                    [Customer].[FirstName], [Customer].[MobileNo], [Customer].[TrebId], [Customer].[EmailId],[Customer].[FirstName],[Customer].[PhotoPath]
                    FROM [Agent] INNER JOIN [Customer] ON [Agent].[CustomerId]= [Customer].[CustomerId] 
                    WHERE [AgentId]<>-1" ;
            if (CustomerId!=0)
            {
                QStr += "AND [Customer].[ParentId]=" + CustomerId + "";
            }
            
            //For Search By Name
            if (!string.IsNullOrEmpty(UserName))
            {
                QStr += " AND [Customer].[FirstName] LIKE '%" + UserName.Replace("'", "") + "%'";
            }
            //For Search By MobileNo
            if (!string.IsNullOrEmpty(MobileNo))
            {
                QStr += " AND [Customer].[MobileNo] LIKE '%" + MobileNo.Replace("'", "") + "%'";
            }
            //For Search By date
            if (!string.IsNullOrEmpty(StartDate) && !string.IsNullOrEmpty(EndDate) && StartDate.Trim() != "" && EndDate.Trim() != "")
            {
                CultureInfo culture = new CultureInfo("en-US");
                string format = "dd/MM/yyyy";

                DateTime startDate = DateTime.ParseExact(StartDate, format, culture);
                DateTime Enddate = DateTime.ParseExact(EndDate, format, culture);
                ////DateTime startDate = Convert.ToDateTime(Convert.ToDateTime(StartDate).ToString("MM/dd/yyyy"), culture);
                ////DateTime Enddate = Convert.ToDateTime(Convert.ToDateTime(EndDate).ToString("MM/dd/yyyy"), culture);
                QStr += " and convert(date, convert(datetime, [date], 103), 101)<= convert(date, convert(datetime, '" + startDate + "', 103), 101)";
                QStr += " and convert(date, convert(datetime, [date2], 103), 101) >= convert(date, convert(datetime, '" + Enddate + "', 103), 101)";

               //// QStr += " AND [Agent].[CreatedOn] BETWEEN '" + startDate + "' AND '" + Enddate + "'";
                //AgentList = AgentList.Where(c => Convert.ToDateTime(Convert.ToDateTime(c.CreatedOn).ToString("MM/dd/yyyy"), culture) >= startDate && Convert.ToDateTime(Convert.ToDateTime(c.CreatedOn).ToString("MM/dd/yyyy"), culture) <= Enddate).ToList();//.Where(c => c.FirstName.ToLower().Contains(FirstName.ToLower()));
            }
            //For Search By TrebId
            if (!string.IsNullOrEmpty(TrebId) && TrebId.Trim() != "")
            {
                QStr += " AND [Customer].[TrebId]='" + TrebId.Replace("'", "") + "'";
            }
            QStr += " Order By [Agent].[IsActive] asc, [Agent].[CreatedOn] desc ";


            dt = CommonClass.GetDataSet(QStr).Tables[0];
          
            List<AgentModel> AgentList = new List<AgentModel>();
            foreach (DataRow dr in dt.Rows)
            {
                AgentModel AgentEntity = new AgentModel();
                AgentEntity.AgentId = Convert.ToInt32(dr["AgentId"]);
                AgentEntity.CompanyId = Convert.ToInt32(dr["CompanyId"] as int?); ;
                AgentEntity.CustomerId = Convert.ToInt32(dr["CustomerId"]);
                AgentEntity.MLS = (dr["MLS"]).ToString();
                AgentEntity.City = (dr["City"]).ToString();
                AgentEntity.Date = (dr["Date"]).ToString();
                AgentEntity.FromTime = (dr["FromTime"]).ToString();
                AgentEntity.ToTime = (dr["ToTime"]).ToString();
                AgentEntity.Date2 = (dr["Date2"]).ToString();
                AgentEntity.FromTime2 = (dr["FromTime2"]).ToString();
                AgentEntity.AgentStatusId = Convert.ToInt32(dr["AgentStatusId"]);
                //Customer table data.
                AgentEntity.TrebId = (dr["TrebId"]).ToString();
                AgentEntity.CustomerMobileNo = (dr["MobileNo"]).ToString();
                AgentEntity.CustomerEmail = (dr["EmailId"]).ToString();
                AgentEntity.CustomerPhoto = (dr["PhotoPath"]).ToString();
                AgentEntity.CustomerName = (dr["FirstName"]).ToString();

                if (dr["Price"] != DBNull.Value)
                {
                    AgentEntity.Price = Convert.ToDecimal(dr["Price"]);
                }

                AgentEntity.Comments = (dr["Comments"]).ToString();
                if (dr["CreatedOn"] != DBNull.Value)
                {
                    AgentEntity.CreatedOn = Convert.ToDateTime((dr["CreatedOn"]));
                }
                //AgentEntity.LastUpdatedon = Convert.ToDateTime((dr["LastUpdatedon"]));
                AgentEntity.IsActive = Convert.ToBoolean(dr["IsActive"]);

                AgentList.Add(AgentEntity);

            }
            var models = new List<AgentModel>();


            // for DisplayMessage count
            Session["PendingAgentCount"] = AgentList.Where(c => c.IsActive == false).Count();
            //End
            foreach (var Agent in AgentList)
            {

                if (Agent.AgentStatusId == (int)EnumValue.AgentSatus.AgentAvailable)
                {
                    Agent.AgentType = EnumValue.GetEnumDescription(EnumValue.AgentSatus.AgentAvailable);
                }
                else if (Agent.AgentStatusId == (int)EnumValue.AgentSatus.AgentRequired)
                {
                    Agent.AgentType = EnumValue.GetEnumDescription(EnumValue.AgentSatus.AgentRequired);
                }

                var customer = _CustomerService.GetCustomer(Convert.ToInt32(Agent.CustomerId));

                if (Agent.CustomerPhoto == null || Agent.CustomerPhoto == "")
                {
                    Agent.CustomerPhoto = CommonCls.GetURL() + "/images/noImage.jpg";
                }
                models.Add(Agent);


            }
            AgentModelList._AgentListModel = models.ToList();
            SessionBatches();
            return View(AgentModelList);
        }

        public ActionResult DeleteCustomer(int id)
        {

            try
            {
                //Delete Agent
                var Agents = _AgentService.GetAgents().Where(c => c.CustomerId == id).ToList();
                foreach (var Agent in Agents)
                {
                    var AgentImages = _PropertyImageService.GetPropertyImages().Where(c => c.AgentId == Agent.AgentId).ToList();
                    foreach (var AgentImage in AgentImages)
                    {
                        _PropertyImageService.DeletePropertyImage(AgentImage);
                    }
                    _AgentService.DeleteAgent(Agent);
                }

                //Delete Property
                var Properties = _PropertyService.GetPropertys().Where(c => c.CustomerId == id).ToList();
                foreach (var Propertie in Properties)
                {
                    var PropertieImages = _PropertyImageService.GetPropertyImages().Where(c => c.AgentId == Propertie.PropertyId).ToList();
                    foreach (var propertyImage in PropertieImages)
                    {
                        _PropertyImageService.DeletePropertyImage(propertyImage);
                    }

                    //Delete Views
                    var Views = _ViewService.GetViewss().Where(c => c.PropertyId == Propertie.PropertyId ).ToList();
                    foreach (var View in Views)
                    {
                        _ViewService.DeleteViews(View);
                    }
                    _PropertyService.DeleteProperty(Propertie);
                }

                //Delete Tip
                var Tips = _TipService.GetTips().Where(c => c.CustomerId == id).ToList();
                foreach (var Tip in Tips)
                {
                    _TipService.DeleteTip(Tip);
                }
               
                //Delete Feedback

                var Feedbacks = _FeedBackService.GetFeedBacks().Where(c => c.CustomerId == id).ToList();
                foreach (var Feedback in Feedbacks)
                {
                    _FeedBackService.DeleteFeedBack(Feedback);
                }
                //Delete EventCustomer
                var EventCustomers = _EventCustomerService.GetEventCustomers().Where(c => c.CustomerId == id).ToList();
                foreach (var EventCustomer in EventCustomers)
                {
                    _EventCustomerService.DeleteEventCustomer(EventCustomer);
                }

                //Delete Views
                var ViewsInCustomer = _ViewService.GetViewss().Where(c => c.CustomerId == id).ToList();
                foreach (var View in ViewsInCustomer)
                {
                    _ViewService.DeleteViews(View);
                }
                //Delete Customer
                var Customer = _CustomerService.GetCustomer(id);
                if (Customer!=null)
                {
                    _CustomerService.DeleteCustomer(Customer);

                    //Delete UserRole
                    var UserRole = _UserRoleService.GetUserRoles().Where(c => c.UserId == Customer.UserId).FirstOrDefault();
                    if (UserRole != null)
                    {
                        _UserRoleService.DeleteUserRole(UserRole);
                    }

                    //Delete User
                    var Users = _UserService.GetUser(Customer.UserId);
                    if (Users != null)
                    {
                        _UserService.DeleteUser(Users);
                    }

                }
                
                TempData["ShowMessage"] = "success";
                TempData["MessageBody"] = "Customer succesfully deleted.";
                return RedirectToAction("CustomerList");
            }
            catch
            {

            }
            TempData["ShowMessage"] = "success";
            TempData["MessageBody"] = "Customer succesfully deleted.";
            return RedirectToAction("CustomerList");
        }

        public ActionResult FeedBackList()
        {
            UserPermissionAction("property", RoleAction.view.ToString());
            CheckPermission();
            TempData["ShowMessage"] = "";
            TempData["MessageBody"] = "";
            var FeedBackList = _FeedBackService.GetFeedBacks();
            Mapper.CreateMap<CommunicationApp.Entity.FeedBack, CommunicationApp.Models.FeedBackModel>();
            var models = new List<FeedBackModel>();
            foreach (var FeedBack in FeedBackList)
            {
                CommunicationApp.Models.FeedBackModel FeedBackModel = Mapper.Map<CommunicationApp.Entity.FeedBack, CommunicationApp.Models.FeedBackModel>(FeedBack);
                if (FeedBack.CustomerId != null)
                {
                    var customer = _CustomerService.GetCustomer(Convert.ToInt32(FeedBack.CustomerId));
                    FeedBackModel.CustomerName = customer.FirstName;
                    FeedBackModel.CustomerPhoto = customer.PhotoPath;
                }

                models.Add(FeedBackModel);
            }

            return View(models.OrderBy(c=>c.IsRead) );
        }


        public ActionResult UpdateFeedback(int id)
        {
            UserPermissionAction("property", RoleAction.view.ToString());
            CheckPermission();
            TempData["ShowMessage"] = "";
            TempData["MessageBody"] = "";
            var FeedBackList = _FeedBackService.GetFeedBack(id);
            if (FeedBackList!=null)
            {
                FeedBackList.IsRead = true;
                _FeedBackService.UpdateFeedBack(FeedBackList);
            }

            return RedirectToAction("FeedBackList", "property");
        }

        

        public ActionResult CustomerList(string FirstName, string Email, string PhoneNo, string TrebId)
        {
            var CustomerId = Convert.ToInt32(Session["CustomerId"]);
            UserPermissionAction("property", RoleAction.view.ToString());
            CheckPermission();
            TempData["ShowMessage"] = "";
            TempData["MessageBody"] = "";
            // var CustomerList = _CustomerService.GetCustomers().Where(c => c.UserId != 1).OrderByDescending(c => c.IsActive == false).ToList();


            CommonClass CommonClass = new CommonClass();
            string QStr = "";
            DataTable dt = new DataTable();
            if (CustomerId != 0)
            {
                QStr = "Select * From Customer where ParentId=" + CustomerId + "  Order By IsActive asc,UpdateStatus asc,CreatedOn desc   ";
            }
            else
            {
                QStr = "Select * From Customer Order By IsActive asc,UpdateStatus asc,CreatedOn desc   ";
            }

            dt = CommonClass.GetDataSet(QStr).Tables[0];
            List<CustomerModel> CustomerList = new List<CustomerModel>();
            foreach (DataRow dr in dt.Rows)
            {
                CustomerModel CustomerEntity = new CustomerModel();
                CustomerEntity.CustomerId = Convert.ToInt32(dr["CustomerId"]);
                CustomerEntity.UserId = Convert.ToInt32(dr["UserId"] as int?); ;
                CustomerEntity.TrebId = (dr["TrebId"]).ToString();
                CustomerEntity.PhotoPath = (dr["PhotoPath"]).ToString();
                CustomerEntity.FirstName = (dr["FirstName"]).ToString();
                CustomerEntity.LastName = (dr["LastName"]).ToString();
                CustomerEntity.MobileNo = (dr["MobileNo"]).ToString();
                CustomerEntity.EmailID = (dr["EmailId"]).ToString();
                CustomerEntity.WebsiteUrl = (dr["WebsiteUrl"]).ToString();
                
                CustomerEntity.CreatedOn = Convert.ToDateTime(((dr["CreatedOn"]).ToString()=="" ? "01/01/1900" : dr["CreatedOn"]));
                CustomerEntity.IsActive = Convert.ToBoolean((dr["IsActive"]));
                CustomerEntity.IsAppLike = Convert.ToBoolean((dr["IsAppLike"]));
                if (dr["UpdateStatus"] != DBNull.Value)
                {
                    CustomerEntity.UpdateStatus = Convert.ToBoolean((dr["UpdateStatus"]));
                }
                if (dr["LastUpdatedOn"] != DBNull.Value)
                {

                    TimeSpan ts = DateTime.Now - Convert.ToDateTime(dr["LastUpdatedOn"]);
                    if (ts.Days > 0 && ts.Days < 30)
                    {
                        CustomerEntity.ActiveTime = Convert.ToString(ts.Days) + " days ";
                    }
                    else if (ts.Days > 365)
                    {
                        CustomerEntity.ActiveTime = Convert.ToString(ts.Days / 365) + " yrs ";
                        CustomerEntity.ActiveTime += Convert.ToString(ts.Days % 365) + " mnths ";
                    }
                    if (ts.Hours > 0)
                    {
                        CustomerEntity.ActiveTime += Convert.ToString(ts.Hours) + " hrs ";
                    }
                    if (ts.Minutes > 0)
                    {
                        CustomerEntity.ActiveTime += Convert.ToString(ts.Minutes) + " mnts ";
                    }
                    CustomerEntity.ActiveTime += " ago";
                }
                else 
                {
                    CustomerEntity.LastUpdatedOn = DateTime.Now;
                    CustomerEntity.ActiveTime += "1 mint ago";
                }
                CustomerList.Add(CustomerEntity);

            }



            // Mapper.CreateMap<CommunicationApp.Entity.Customer, CommunicationApp.Models.CustomerModel>();

            //search by  FirstName.
            if (!string.IsNullOrEmpty(FirstName))
            {
                CustomerList = CustomerList.Where(c => c.FirstName.ToLower().Contains(FirstName.ToLower().Trim())).ToList();//.Where(c => c.FirstName.ToLower().Contains(FirstName.ToLower()));
            }
            //search by  Email
            if (!string.IsNullOrEmpty(Email))
            {
                CustomerList = CustomerList.Where(c => c.EmailID.ToLower().Contains(Email.ToLower().Trim())).ToList();//.Where(c => c.FirstName.ToLower().Contains(FirstName.ToLower()));
            }
            //search by  PhoneNo
            if (!string.IsNullOrEmpty(PhoneNo))
            {
                CustomerList = CustomerList.Where(c => c.MobileNo.ToLower().Contains(PhoneNo.ToLower().Trim())).ToList();//.Where(c => c.FirstName.ToLower().Contains(FirstName.ToLower()));
            }
            //For Search By TrebId
            if (!string.IsNullOrEmpty(TrebId) && !string.IsNullOrEmpty(TrebId) && TrebId.Trim() != "" && TrebId.Trim() != "")
            {
                CustomerList = CustomerList.Where(c => c.TrebId == TrebId).ToList();
            }
            var models = new List<CustomerModel>();
            foreach (var CustomerModel in CustomerList)
            {
                // CommunicationApp.Models.CustomerModel CustomerModel = Mapper.Map<CommunicationApp.Entity.Customer, CommunicationApp.Models.CustomerModel>(Customer);

                if (CustomerModel.PhotoPath == null || CustomerModel.PhotoPath == "")
                {
                    CustomerModel.PhotoPath = CommonCls.GetURL() + "/images/noImage.jpg";
                }
                if (CustomerModel.IsActive == false)
                {
                    CustomerModel.PhotoPath = CommonCls.GetURL() + "/images/noImage.jpg";
                }
                models.Add(CustomerModel);
            }
            SessionBatches();
            return View(models);
        }
        public ActionResult Delete(int id)
        {
            try
            {
                var Property = _PropertyService.GetProperty(id);
                if (Property != null)
                {
                    var PropertyImages = _PropertyImageService.GetPropertyImages().Where(c => c.PropertyId == Property.PropertyId);
                    foreach (var PropertyImage in PropertyImages)
                    {
                        _PropertyImageService.DeletePropertyImage(PropertyImage);
                    }
                    var Views = _ViewService.GetViewss().Where(c => c.PropertyId == Property.PropertyId);
                    foreach (var View in Views)
                    {
                        _ViewService.DeleteViews(View);
                    }
                    _PropertyService.DeleteProperty(Property);
                    TempData["ShowMessage"] = "success";
                    TempData["MessageBody"] = Property.PropertyType + " is deleted successfully.";
                    var adminid =Convert.ToInt32( Session["CustomerId"].ToString());
                    return RedirectToAction("WelcomeHome", "home", new { id = adminid });
                }
            }
            catch
            {
                TempData["MessageBody"] = "No record is deleted.";
                return RedirectToAction("WelcomeHome", "home");
            }


            return RedirectToAction("WelcomeHome", "home");
            // return RedirectToAction("home/WelcomeHome");
        }
        public ActionResult DeleteAgents(int? id)
        {
            var Id = Convert.ToInt32(id);
            try
            {
                var Agent = _AgentService.GetAgent(Id);
                if (Agent != null)
                {
                    var AgentImages = _PropertyImageService.GetPropertyImages().Where(c => c.AgentId == Agent.AgentId);
                    foreach (var AgentImage in AgentImages)
                    {
                        _PropertyImageService.DeletePropertyImage(AgentImage);
                    }
                    _AgentService.DeleteAgent(Agent);
                    TempData["ShowMessage"] = "success";
                    TempData["MessageBody"] = "Agent is delete successfully.";
                    return RedirectToAction("Agentlist");
                }
            }
            catch
            {
                TempData["MessageBody"] = "No record is deleted.";
                return RedirectToAction("Agentlist");
            }



            return RedirectToAction("Agentlist");
        }
        public void SessionBatches()
        {
            var CustomerId = Convert.ToInt32(Session["CustomerId"]);
            CommonClass CommonClass = new CommonClass();
            string QStr = "";
            DataTable dt = new DataTable();
            QStr = "SELECT (Select Count(*) From Agent inner join Customer on Customer.CustomerId=Agent.CustomerId  where Agent.IsActive=0 and Customer.ParentId=" + CustomerId + ") AS DeactiveAgentCount,";
            QStr += "(Select Count(*) From Agent inner join Customer on Customer.CustomerId=Agent.CustomerId  where Agent.IsActive=1 and Customer.ParentId=" + CustomerId + ") AS ActiveAgentCount,";
            QStr += "(Select Count(*) From Customer where ParentId=" + CustomerId + " AND IsActive=0) AS DeactiveCustomerCount,";
            QStr += "(Select Count(*) From Customer Where ParentId=" + CustomerId + " AND IsActive=1) AS ActiveCustomerCount,";
            QStr += "(Select Count(*) From Property inner join Customer on Customer.CustomerId=Property.CustomerId  where Property.IsActive=0 and Customer.ParentId=" + CustomerId + ") AS DeactivePropertyCount,";
            QStr += "(Select Count(*) From Property inner join Customer on Customer.CustomerId=Property.CustomerId  where Property.IsActive=1 and Customer.ParentId=" + CustomerId + ") AS ActivePropertyCount , ";
            QStr += "(Select Count(*) From Customer where IsAppLike=1) AS Likes, ";
            QStr += "(Select Count(*) From Customer join Brokerage on Brokerage.CustomerId=Customer.CustomerId Where ParentId=" + CustomerId + " and ApprovalDate is NULL) AS NewRiskForm";
            dt = CommonClass.GetDataSet(QStr).Tables[0];

            //Pass data to seesion.
            Session["PendingPropertyCount"] = dt.Rows[0]["DeactivePropertyCount"].ToString();
            Session["ApprovePropertyCount"] = dt.Rows[0]["ActivePropertyCount"].ToString();
            Session["PendingAgentCount"] = dt.Rows[0]["DeactiveAgentCount"].ToString();
            Session["ApproveAgentCount"] = dt.Rows[0]["ActiveAgentCount"].ToString();
            Session["PendingUserCount"] = dt.Rows[0]["DeactiveCustomerCount"].ToString();
            Session["ApproveUserCount"] = dt.Rows[0]["ActiveCustomerCount"].ToString();
            Session["Likes"] = dt.Rows[0]["Likes"].ToString();
            Session["NewRiskForm"] = dt.Rows[0]["NewRiskForm"].ToString();
        }
        public ActionResult GetPropertyById(int id)
        {
            UserPermissionAction("property", RoleAction.view.ToString());
            CheckPermission();

            CommunicationApp.Web.Models.PropertyModel PropertyModelData = new CommunicationApp.Web.Models.PropertyModel();
            try
            {
                var PropertyId = id;
                if (PropertyId != 0)
                {
                    //var PropertyEntity = _PropertyService.GetProperty(PropertyId);


                    CommonClass CommonClass = new CommonClass();
                    string QStr = "";
                    DataTable dt = new DataTable();
                    QStr = "Select * From Property where PropertyId =" + PropertyId;
                    dt = CommonClass.GetDataSet(QStr).Tables[0];
                    List<Property> PropertyList = new List<Property>();
                    Property PropertyEntity = new Property();
                    if (dt.Rows.Count > 0)
                    {


                        PropertyEntity.PropertyId = Convert.ToInt32(dt.Rows[0]["PropertyId"]);
                        PropertyEntity.CustomerId = Convert.ToInt32(dt.Rows[0]["CustomerId"]);
                        PropertyEntity.PropertyStatusId = Convert.ToInt32(dt.Rows[0]["PropertyStatusId"]);
                        //if (dr["SaleOfBusinessId"] != DBNull.Value)                

                        PropertyEntity.MLS = (dt.Rows[0]["MLS"]).ToString();
                        PropertyEntity.Price = (dt.Rows[0]["Price"]).ToString();
                        PropertyEntity.MininumPrice = (dt.Rows[0]["MininumPrice"]).ToString();
                        PropertyEntity.MaximumPrice = (dt.Rows[0]["MaximumPrice"]).ToString();
                        PropertyEntity.LocationPrefered = (dt.Rows[0]["LocationPrefered"]).ToString();
                        PropertyEntity.Style = (dt.Rows[0]["Style"]).ToString();
                        PropertyEntity.Age = (dt.Rows[0]["Age"]).ToString();
                        PropertyEntity.Garage = (dt.Rows[0]["Garage"]).ToString();
                        PropertyEntity.Bedrooms = (dt.Rows[0]["Bedrooms"]).ToString();
                        PropertyEntity.Bathrooms = (dt.Rows[0]["Bathrooms"]).ToString();
                        PropertyEntity.PropertyType = (dt.Rows[0]["PropertyType"]).ToString();
                        PropertyEntity.Basement = (dt.Rows[0]["Basement"]).ToString();
                        PropertyEntity.BasementValue = (dt.Rows[0]["BasementValue"]).ToString();
                        PropertyEntity.Community = (dt.Rows[0]["Community"]).ToString();
                        PropertyEntity.Size = (dt.Rows[0]["Size"]).ToString();
                        PropertyEntity.Remark = (dt.Rows[0]["Remark"]).ToString();
                        PropertyEntity.Kitchen = (dt.Rows[0]["Kitchen"]).ToString();
                        PropertyEntity.Type = (dt.Rows[0]["Type"]).ToString();
                        PropertyEntity.Alivator = (dt.Rows[0]["Alivator"]).ToString();
                        PropertyEntity.GarageType = (dt.Rows[0]["GarageType"]).ToString();
                        PropertyEntity.SideDoorEntrance = (dt.Rows[0]["SideDoorEntrance"]).ToString();
                        PropertyEntity.Loundry = (dt.Rows[0]["Loundry"]).ToString();
                        PropertyEntity.Level = (dt.Rows[0]["Level"]).ToString();
                        PropertyEntity.ListPriceCode = (dt.Rows[0]["ListPriceCode"]).ToString();
                        PropertyEntity.TypeTaxes = (dt.Rows[0]["TypeTaxes"]).ToString();
                        PropertyEntity.TypeCommercial = (dt.Rows[0]["TypeCommercial"]).ToString();
                        PropertyEntity.CategoryCommercial = (dt.Rows[0]["CategoryCommercial"]).ToString();
                        PropertyEntity.Use = (dt.Rows[0]["Use"]).ToString();
                        PropertyEntity.Zoning = (dt.Rows[0]["Zoning"]).ToString();
                        if (dt.Rows[0]["CreatedOn"] != DBNull.Value)
                        {
                            PropertyEntity.CreatedOn = Convert.ToDateTime((dt.Rows[0]["CreatedOn"]));
                        }
                        //PropertyEntity.CreatedOn = Convert.ToDateTime((dt.Rows[0]["CreatedOn"]));
                        //Property.LastUpdatedon = Convert.ToDateTime((dr["LastUpdatedon"]));
                        PropertyEntity.IsActive = Convert.ToBoolean(dt.Rows[0]["IsActive"]);
                    }
                    //PropertyList.Add(Property);


                    Mapper.CreateMap<CommunicationApp.Entity.Property, CommunicationApp.Web.Models.PropertyModel>();
                    PropertyModelData = Mapper.Map<CommunicationApp.Entity.Property, CommunicationApp.Web.Models.PropertyModel>(PropertyEntity);

                    var PropertyImages = _PropertyImageService.GetPropertyImages().Where(c => c.PropertyId == PropertyId).ToList();
                    PropertyModelData.PropertyImage = "";
                    if (PropertyImages.Count() > 0)
                    {
                        PropertyModelData.PropertyImage = PropertyImages.FirstOrDefault().ImagePath;
                    }
                    else
                    {
                        PropertyModelData.PropertyImage = CommonCls.GetURL() + "/images/no-image-available.jpg";
                    }

                    List<PropertyImages> PropertyImageList = new List<PropertyImages>();
                    foreach (var propertyImage in PropertyImages)
                    {
                        PropertyImages PropertyImage = new PropertyImages();
                        PropertyImage.imagelist = propertyImage.ImagePath;
                        PropertyImage.PropertyImageId = propertyImage.PropertyImageId;
                        PropertyImageList.Add(PropertyImage);
                    }

                    PropertyModelData.PropertyPhotolist = PropertyImageList;
                    var Customer = _CustomerService.GetCustomer(Convert.ToInt32(PropertyEntity.CustomerId));
                    PropertyModelData.CustomerName = Customer.FirstName;
                    PropertyModelData.CustomerPhoto = Customer.PhotoPath;
                    PropertyModelData.CustomerEmail = Customer.EmailId;
                    PropertyModelData.CustomerPhoneNo = Customer.MobileNo;
                    PropertyModelData.PropertyId = PropertyId;
                    return View(PropertyModelData);
                }
                Session["PendingPropertyCount"] = _AgentService.GetAgents().Where(c => c.IsActive == false).Count();


            }
            catch (Exception ex)
            {
                ErrorLogging.LogError(ex);

            }
            return View(PropertyModelData);
        }
        public JsonResult EditPropertyRemarks(int PropertyId, string Remarks)
        {
            try
            {
                if (PropertyId != 0)
                {
                    var Property = _PropertyService.GetProperty(PropertyId);
                    Property.Remark = Remarks;
                    //Property.IsActive = false;
                    _PropertyService.UpdateProperty(Property);
                    return Json(Remarks);
                }
                else
                {
                    return Json("errorss");
                }

            }
            catch (Exception ex)
            {
                TempData["ShowMessage"] = "error";
                TempData["MessageBody"] = "";
                ErrorLogging.LogError(ex);

            }

            return Json("success");
        }
        public JsonResult EditAgentRemarks(int PropertyId, string Remarks)
        {
            try
            {
                if (PropertyId != 0)
                {
                    var Property = _AgentService.GetAgent(PropertyId);
                    Property.Comments = Remarks;
                    //Property.IsActive = false;
                    _AgentService.UpdateAgent(Property);
                    return Json(Remarks);
                }
                else
                {
                    return Json("errors");
                }

            }
            catch (Exception ex)
            {
                TempData["ShowMessage"] = "error";
                TempData["MessageBody"] = "";
                ErrorLogging.LogError(ex);

            }

            return Json("success");
        }
        [HttpPost]
        public ActionResult EditPropertyImage(HttpPostedFileBase file, FormCollection form)
        {
            int propertyId = Convert.ToInt32(Request.Form["PropertyId"]);
            try
            {
                // int propertyId = Convert.ToInt32(form["PropertyId"].ToString());

                var PhotoPath = "";
                if (propertyId != 0)
                {
                    var Property = _PropertyImageService.GetPropertyImages().Where(c => c.PropertyId == propertyId).FirstOrDefault();
                    if (Property == null)
                    {
                        TempData["ShowMessage"] = "error";
                        TempData["MessageBody"] = "No image available for edit .";
                        return RedirectToAction("GetPropertyById", "Property", new { id = propertyId });
                    }
                    if (file != null)
                    {

                        if (Property.ImagePath != "")
                        {   //Delete Old Image
                            string pathDel = Server.MapPath("~/EventPhoto");

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
                        var subPath = Server.MapPath("~/EventPhoto");

                        //Check SubPath Exist or Not
                        if (!Directory.Exists(subPath))
                        {
                            Directory.CreateDirectory(subPath);
                        }
                        //End : Check SubPath Exist or Not

                        var path = Path.Combine(subPath, fileName);
                        file.SaveAs(path);

                        PhotoPath = CommonCls.GetURL() + "/EventPhoto/" + fileName;
                    }






                    Property.ImagePath = PhotoPath;
                    _PropertyImageService.UpdatePropertyImage(Property);
                    return RedirectToAction("GetPropertyById", "Property", new { id = Property.PropertyId });
                }
                else
                {
                    return RedirectToAction("GetPropertyById", "Property", new { id = propertyId });
                }

            }
            catch (Exception ex)
            {
                TempData["ShowMessage"] = "error";
                TempData["MessageBody"] = "";
                ErrorLogging.LogError(ex);

            }

            return RedirectToAction("GetPropertyById", "Property", new { id = propertyId });
        }
        [HttpPost]
        public ActionResult EditAgentImage(HttpPostedFileBase file, FormCollection form)//EditPropertyImage
        {
            int propertyId = Convert.ToInt32(Request.Form["PropertyId"]);
            try
            {
                // int propertyId = Convert.ToInt32(form["PropertyId"].ToString());

                var PhotoPath = "";
                if (propertyId != 0)
                {
                    var Property = _PropertyImageService.GetPropertyImages().Where(c => c.AgentId == propertyId).FirstOrDefault();
                    if (Property == null)
                    {
                        TempData["ShowMessage"] = "error";
                        TempData["MessageBody"] = "No image available for edit .";
                        return RedirectToAction("GetPropertyById", "Property", new { id = propertyId });
                    }
                    if (file != null)
                    {

                        if (Property.ImagePath != "")
                        {   //Delete Old Image
                            string pathDel = Server.MapPath("~/EventPhoto");

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
                        var subPath = Server.MapPath("~/EventPhoto");

                        //Check SubPath Exist or Not
                        if (!Directory.Exists(subPath))
                        {
                            Directory.CreateDirectory(subPath);
                        }
                        //End : Check SubPath Exist or Not

                        var path = Path.Combine(subPath, fileName);
                        file.SaveAs(path);

                        PhotoPath = CommonCls.GetURL() + "/EventPhoto/" + fileName;
                    }
                    Property.ImagePath = PhotoPath;
                    _PropertyImageService.UpdatePropertyImage(Property);
                    return RedirectToAction("GetAgentProperty", "Property", new { id = propertyId });
                }
                else
                {
                    return RedirectToAction("GetAgentProperty", "Property", new { id = propertyId });
                }

            }
            catch (Exception ex)
            {
                TempData["ShowMessage"] = "error";
                TempData["MessageBody"] = "";
                ErrorLogging.LogError(ex);

            }

            return RedirectToAction("GetAgentProperty", "Property", new { id = propertyId });
        }
        public ActionResult GetAgentProperty(int id)
        {
            UserPermissionAction("property", RoleAction.view.ToString());
            CheckPermission();
            TempData["ShowMessage"] = "";
            TempData["MessageBody"] = "";
            CommunicationApp.Models.AgentModel PropertyModelData = new CommunicationApp.Models.AgentModel();
            try
            {
                var PropertyId = id;
                if (PropertyId != 0)
                {

                    CommonClass CommonClass = new CommonClass();
                    string QStr = "";
                    DataTable dt = new DataTable();
                    QStr = "Select * From Agent  where AgentId= " + id;
                    dt = CommonClass.GetDataSet(QStr).Tables[0];
                    List<Agent> AgentList = new List<Agent>();
                    Agent AgentEntity = new Agent();
                    AgentEntity.AgentId = Convert.ToInt32(dt.Rows[0]["AgentId"]);
                    AgentEntity.CompanyId = Convert.ToInt32(dt.Rows[0]["CompanyId"] as int?); ;
                    AgentEntity.CustomerId = Convert.ToInt32(dt.Rows[0]["CustomerId"]);
                    AgentEntity.MLS = (dt.Rows[0]["MLS"]).ToString();
                    AgentEntity.City = (dt.Rows[0]["City"]).ToString();
                    AgentEntity.Date = (dt.Rows[0]["Date"]).ToString();
                    AgentEntity.FromTime = (dt.Rows[0]["FromTime"]).ToString();
                    AgentEntity.ToTime = (dt.Rows[0]["ToTime"]).ToString();
                    AgentEntity.Date2 = (dt.Rows[0]["Date2"]).ToString();
                    AgentEntity.FromTime2 = (dt.Rows[0]["FromTime2"]).ToString();
                    if (dt.Rows[0]["Price"] != DBNull.Value)
                    {
                        AgentEntity.Price = Convert.ToDecimal(dt.Rows[0]["Price"]);
                    }

                    AgentEntity.Comments = (dt.Rows[0]["Comments"]).ToString();
                   
                    if (dt.Rows[0]["CreatedOn"] != DBNull.Value)
                    {
                        AgentEntity.CreatedOn = Convert.ToDateTime((dt.Rows[0]["CreatedOn"]));
                    }
                    //AgentEntity.LastUpdatedon = Convert.ToDateTime((dr["LastUpdatedon"]));
                    AgentEntity.IsActive = Convert.ToBoolean(dt.Rows[0]["IsActive"]);


                    Mapper.CreateMap<CommunicationApp.Entity.Agent, CommunicationApp.Models.AgentModel>();
                    AgentModel AgentModelData = Mapper.Map<CommunicationApp.Entity.Agent, CommunicationApp.Models.AgentModel>(AgentEntity);

                    var PropertyImages = _PropertyImageService.GetPropertyImages().Where(c => c.AgentId == PropertyId).ToList();
                    AgentModelData.OpenHousePropertyImage = "";
                    if (PropertyImages.Count() > 0)
                    {
                        AgentModelData.OpenHousePropertyImage = PropertyImages.FirstOrDefault().ImagePath;
                    }
                    else
                    {
                        AgentModelData.OpenHousePropertyImage = CommonCls.GetURL() + "/images/no-image-available.jpg";
                    }

                    List<PropertyImages> PropertyImageList = new List<PropertyImages>();
                    foreach (var propertyImage in PropertyImages)
                    {
                        PropertyImages PropertyImage = new PropertyImages();
                        PropertyImage.imagelist = propertyImage.ImagePath;
                        PropertyImage.PropertyImageId = propertyImage.PropertyImageId;
                        PropertyImageList.Add(PropertyImage);
                    }

                    AgentModelData.PropertyPhotolist = PropertyImageList;
                    var Customer = _CustomerService.GetCustomer(Convert.ToInt32(AgentEntity.CustomerId));
                    AgentModelData.CustomerName = Customer.FirstName;
                    AgentModelData.CustomerPhoto = Customer.PhotoPath;
                    AgentModelData.CustomerEmail = Customer.EmailId;
                    AgentModelData.CustomerMobileNo = Customer.MobileNo;
                    return View(AgentModelData);
                }
                Session["PendingAgentCount"] = _AgentService.GetAgents().Where(c => c.IsActive == false).Count();


            }
            catch (Exception ex)
            {
                TempData["ShowMessage"] = "error";
                TempData["MessageBody"] = ex.Message.ToString();
            }
            return View(PropertyModelData);
        }//DeletePropertyImage
        [HttpPost]
        public JsonResult DeletePropertyImage(string id)
        {
            DeleteImageModel DeleteImageModel = new DeleteImageModel();
            DeleteImageModel.ImagePropertyId = id;
            UserPermissionAction("property", RoleAction.view.ToString());
            CheckPermission();
            TempData["ShowMessage"] = "";
            TempData["MessageBody"] = "";
            var PropertyImageId = Convert.ToInt32(id);
            var PropertyId = 0;
            var PropertyImagess = _PropertyImageService.GetPropertyImage(PropertyImageId);
            if (PropertyImagess != null)
            {
                PropertyId = Convert.ToInt32(PropertyImagess.PropertyId);
            }

            try
            {

                if (PropertyImageId != 0)
                {

                    if (PropertyImagess != null)
                    {

                        DeleteImageModel.DeletedImagePath = PropertyImagess.ImagePath;

                        if (PropertyImagess.ImagePath != "")
                        {   //Delete Old Image
                            string pathDel = Server.MapPath("~/EventPhoto");

                            FileInfo objfile = new FileInfo(pathDel);
                            if (objfile.Exists) //check file exsit or not
                            {
                                objfile.Delete();
                            }
                            //End :Delete Old Image
                        }

                        _PropertyImageService.DeletePropertyImage(PropertyImagess);
                        var PropertyImage = _PropertyImageService.GetPropertyImages().Where(c => c.PropertyId == PropertyId).FirstOrDefault();
                        if (PropertyImage != null)
                        {
                            DeleteImageModel.NewImagePath = PropertyImage.ImagePath;
                        }
                        return Json(DeleteImageModel);
                    }
                    else
                    {
                        return Json("error");

                    }

                }


            }
            catch (Exception ex)
            {
                TempData["ShowMessage"] = "error";
                TempData["MessageBody"] = ex.Message.ToString();
            }
            return Json(DeleteImageModel);
        }
        [HttpPost]
        public JsonResult DeleteAgentImage(string id)
        {
            DeleteImageModel DeleteImageModel = new DeleteImageModel();
            DeleteImageModel.ImagePropertyId = id;
            UserPermissionAction("property", RoleAction.view.ToString());
            CheckPermission();
            TempData["ShowMessage"] = "";
            TempData["MessageBody"] = "";
            var PropertyImageId = Convert.ToInt32(id);
            var PropertyId = 0;
            var PropertyImagess = _PropertyImageService.GetPropertyImage(PropertyImageId);
            if (PropertyImagess != null)
            {
                PropertyId = Convert.ToInt32(PropertyImagess.PropertyId);
            }

            try
            {

                if (PropertyImageId != 0)
                {

                    if (PropertyImagess != null)
                    {

                        DeleteImageModel.DeletedImagePath = PropertyImagess.ImagePath;

                        if (PropertyImagess.ImagePath != "")
                        {   //Delete Old Image
                            string pathDel = Server.MapPath("~/EventPhoto");

                            FileInfo objfile = new FileInfo(pathDel);
                            if (objfile.Exists) //check file exsit or not
                            {
                                objfile.Delete();
                            }
                            //End :Delete Old Image
                        }

                        _PropertyImageService.DeletePropertyImage(PropertyImagess);
                        var PropertyImage = _PropertyImageService.GetPropertyImages().Where(c => c.PropertyId == PropertyId).FirstOrDefault();
                        if (PropertyImage != null)
                        {
                            DeleteImageModel.NewImagePath = PropertyImage.ImagePath;
                        }
                        return Json(DeleteImageModel);
                    }
                    else
                    {
                        return Json("error");

                    }

                }


            }
            catch (Exception ex)
            {
                TempData["ShowMessage"] = "error";
                TempData["MessageBody"] = ex.Message.ToString();
            }
            return Json(DeleteImageModel);
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
        private ActionResult view()
        {
            throw new NotImplementedException();
        }
        protected bool CheckDate(String date)
        {
            try
            {
                DateTime dt = DateTime.Parse(date);
                return true;
            }
            catch
            {
                return false;
            }

        }
    }
}




