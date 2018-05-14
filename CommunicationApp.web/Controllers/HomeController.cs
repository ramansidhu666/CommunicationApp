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

namespace CommunicationApp.Controllers
{
    public class HomeController : BaseController
    {
        public ICompanyService _CompanyService { get; set; }
        public IPropertyService _PropertyService { get; set; }
        public IPropertyImageService _PropertyImageService { get; set; }
        public ICustomerService _CustomerService { get; set; }
        public IAgentService _AgentService { get; set; }
        //public IBrokerageServices _BrokerageService { get; set; }
        //public IBrokerageServiceServices _BrokerageServiceServices { get; set; }


        public HomeController(IAgentService AgentService, ICustomerService CustomerService, IUserService UserService, IRoleService RoleService, IFormService FormService, IRoleDetailService RoleDetailService, IRoleService _RoleService, IUserRoleService UserroleService, IPropertyImageService PropertyImageService, IPropertyService PropertyService, ICompanyService CompanyService)
            : base(CustomerService, UserService, RoleService, FormService, RoleDetailService, UserroleService)
        {
            this._CompanyService = CompanyService;
            this._PropertyService = PropertyService;
            this._PropertyImageService = PropertyImageService;
            this._CustomerService = CustomerService;
            this._AgentService = AgentService;
            //this._BrokerageService = BrokerageService;
            //this._BrokerageServiceServices = BrokerageServiceServices;
        }

        private void CheckPermission()
        {
            RoleDetailModel roleDetail = UserPermission("home");
            TempData["Create"] = roleDetail.IsCreate;
            TempData["Delete"] = roleDetail.IsDelete;
            TempData["Detail"] = roleDetail.IsDetail;
            TempData["Edit"] = roleDetail.IsEdit;
            TempData["View"] = roleDetail.IsView;
        }
        [ValidateInput(false)]
        public ActionResult Index(string UserName, string MobileNo, string StartDate, string EndDate, string TrebId, string PropertyType)
        {

            UserPermissionAction("home", RoleAction.view.ToString());
            CheckPermission();
            List<PropertyModel> PropertyModelList = new List<PropertyModel>();
            PropertyListModel PropertyListModel = new PropertyListModel();
            PropertyListModel.UserName = UserName;
            PropertyListModel.StartDate = StartDate;
            PropertyListModel.EndDate = EndDate;
            PropertyListModel.PropertyModelList = PropertyModelList;

            if (!string.IsNullOrEmpty(StartDate))
            {
                if (CheckDate(StartDate) == false)
                {
                    TempData["ShowMessage"] = "error";
                    TempData["MessageBody"] = "Please select a valid From Date.";
                    return View("WelcomeHome", PropertyListModel);
                }
            }
            if (!string.IsNullOrEmpty(EndDate))
            {
                if (CheckDate(EndDate) == false)
                {
                    TempData["ShowMessage"] = "error";
                    TempData["MessageBody"] = "Please select a valid To Date.";
                    return View("WelcomeHome", PropertyListModel);
                }
            }

            if (!string.IsNullOrEmpty(StartDate) && !string.IsNullOrEmpty(EndDate))
            {
                if (Convert.ToDateTime(Convert.ToDateTime(StartDate).ToString("MM/dd/yyyy")) > Convert.ToDateTime(Convert.ToDateTime(EndDate).ToString("MM/dd/yyyy")))
                {
                    TempData["ShowMessage"] = "error";
                    TempData["MessageBody"] = "Please select a valid date range.";

                    return View("WelcomeHome", PropertyListModel);
                }

            }
            var PropertyList = _PropertyService.GetPropertys().OrderByDescending(c => c.IsActive == false).ToList();

            //For Search By Name
            if (!string.IsNullOrEmpty(UserName))
            {
                PropertyList = PropertyList.Where(c => c.Customers.FirstName.ToLower().Contains(UserName.ToLower().Trim())).ToList();//.Where(c => c.FirstName.ToLower().Contains(FirstName.ToLower()));
            }
            //For Search By PropertyType
            if (!string.IsNullOrEmpty(PropertyType))
            {
                if (Convert.ToInt32(PropertyType) == 0)
                {
                    PropertyList = PropertyList.ToList();
                }
                else
                {
                    PropertyList = PropertyList.Where(c => c.PropertyStatusId == Convert.ToInt32(PropertyType)).ToList();
                }

            }

            //For Search By TrebId
            if (!string.IsNullOrEmpty(TrebId) && !string.IsNullOrEmpty(TrebId) && TrebId.Trim() != "" && TrebId.Trim() != "")
            {
                PropertyList = PropertyList.Where(c => c.Customers.TrebId == TrebId).ToList();
            }
            PropertyModelList = GetPropertyList(PropertyList);
            PropertyListModel.PropertyModelList = PropertyModelList;
            IEnumerable<EnumValue.PropertySatus> propertyStatusValues = System.Enum.GetValues(typeof(EnumValue.PropertySatus)).Cast<EnumValue.PropertySatus>();

            List<SelectListItem> PropertyTypeList = new List<SelectListItem>();
            PropertyTypeList.Add(new SelectListItem() { Text = "--All--", Value = "0", Selected = false });
            PropertyTypeList.Add(new SelectListItem() { Text = "Exclusive Residential", Value = "1", Selected = false });
            PropertyTypeList.Add(new SelectListItem() { Text = "Exclusive Commercial", Value = "2", Selected = true });
            PropertyTypeList.Add(new SelectListItem() { Text = "Exclusive Condo", Value = "9", Selected = false });
            PropertyTypeList.Add(new SelectListItem() { Text = "New/Hot Commercial", Value = "3", Selected = false });
            PropertyTypeList.Add(new SelectListItem() { Text = "New/Hot Residential", Value = "4", Selected = false });
            PropertyTypeList.Add(new SelectListItem() { Text = "New/Hot Condo", Value = "8", Selected = false });
            PropertyTypeList.Add(new SelectListItem() { Text = "Looking For Commercial", Value = "5", Selected = true });
            PropertyTypeList.Add(new SelectListItem() { Text = "Looking For Residential", Value = "6", Selected = false });
            PropertyTypeList.Add(new SelectListItem() { Text = "Looking For Condo", Value = "7", Selected = false });


            ViewBag.PropertyType = new SelectList(PropertyTypeList, "Value", "Text");
            return View("WelcomeHome", PropertyListModel);
        }

        public ActionResult Company()
        {
            Session["CompanyID"] = 0; //Set Company Id to 0, so company will be choose from Choose Company View
            Session["CompanyName"] = "";//Set Company Name Blank
            Session["LogoPath"] = ""; //Set Logo
            ViewBag.CompanyList = _CompanyService.GetCompanies().OrderBy(x => x.CompanyName);

            return View();
        }
        public ActionResult WelcomeHome(int? id)
        {
            var CustomerId = 0;
            if (id != 0 && id != null)
            {
                CustomerId = Convert.ToInt32(id);
                Session["CustomerId"] = CustomerId;
                Session["UserName"] = "";
                
            }
            else
            {
                CustomerId = Convert.ToInt32(Session["CustomerId"]);
                return RedirectToAction("Index", "Admin");
            }
            var Customer = _CustomerService.GetCustomer(CustomerId);
            if (Customer != null)
            {
                Session["CompanyID"] = Customer.CompanyID;
                Session["adminphoto"] = Customer.PhotoPath;
                Session["FullUserName"] = (Customer.FirstName + " " + Customer.LastName).Trim();
            }

            UserPermissionAction("home", RoleAction.view.ToString());
            CheckPermission();
            List<PropertyModel> PropertyModelList = new List<PropertyModel>();
            CommonClass CommonClass = new CommonClass();
            string QStr = "";
            DataTable dt = new DataTable();
            QStr = "select * from Property ";
            QStr += "Inner join Customer on Customer.CustomerId=Property.CustomerId ";
            if (CustomerId != 0)
            {
                QStr += "Where Customer.ParentId='" + CustomerId + "'  Order By Property.IsActive asc, Property.CreatedOn desc";
            }
            else
            {
                QStr += "Order By Property.IsActive asc, Property.CreatedOn desc";
            }

            dt = CommonClass.GetDataSet(QStr).Tables[0];
            List<Property> PropertyList = new List<Property>();
            foreach (DataRow dr in dt.Rows)
            {
                Property Property = new Property();
                Property.PropertyId = Convert.ToInt32(dr["PropertyId"]);
                Property.CustomerId = Convert.ToInt32(dr["CustomerId"]);

                Property.PropertyStatusId = Convert.ToInt32(dr["PropertyStatusId"]);
                Property.MLS = (dr["MLS"]).ToString();
                Property.Price = (dr["Price"]).ToString();
                Property.MininumPrice = (dr["MininumPrice"]).ToString();
                Property.MaximumPrice = (dr["MaximumPrice"]).ToString();
                Property.LocationPrefered = (dr["LocationPrefered"]).ToString();
                Property.Style = (dr["Style"]).ToString();
                Property.Age = (dr["Age"]).ToString();
                Property.Garage = (dr["Garage"]).ToString();
                Property.Bedrooms = (dr["Bedrooms"]).ToString();
                Property.Bathrooms = (dr["Bathrooms"]).ToString();
                Property.PropertyType = (dr["PropertyType"]).ToString();
                Property.Basement = (dr["Basement"]).ToString();
                Property.BasementValue = (dr["BasementValue"]).ToString();
                Property.Community = (dr["Community"]).ToString();
                Property.Size = (dr["Size"]).ToString();
                Property.Remark = (dr["Remark"]).ToString();
                Property.Kitchen = (dr["Kitchen"]).ToString();
                Property.Type = (dr["Type"]).ToString();
                Property.Alivator = (dr["Alivator"]).ToString();
                Property.GarageType = (dr["GarageType"]).ToString();
                Property.SideDoorEntrance = (dr["SideDoorEntrance"]).ToString();
                Property.Loundry = (dr["Loundry"]).ToString();
                Property.Level = (dr["Level"]).ToString();
                Property.ListPriceCode = (dr["ListPriceCode"]).ToString();
                Property.TypeTaxes = (dr["TypeTaxes"]).ToString();
                Property.TypeCommercial = (dr["TypeCommercial"]).ToString();
                Property.CategoryCommercial = (dr["CategoryCommercial"]).ToString();
                Property.Use = (dr["Use"]).ToString();
                Property.Zoning = (dr["Zoning"]).ToString();
                if (dr["CreatedOn"] != DBNull.Value)
                {
                    Property.CreatedOn = Convert.ToDateTime((dr["CreatedOn"]));
                }
                //Property.LastUpdatedon = Convert.ToDateTime((dr["LastUpdatedon"]));
                Property.IsActive = Convert.ToBoolean(dr["IsActive"]);


                PropertyList.Add(Property);
            }

            PropertyModelList = GetPropertyList(PropertyList);
            ViewBag.Title = "Welcome";


            SessionBatches(id);
            PropertyListModel PropertyListModel = new PropertyListModel();
            PropertyListModel.UserName = "";
            PropertyListModel.StartDate = "";
            PropertyListModel.EndDate = "";
            PropertyListModel.PropertyModelList = PropertyModelList;

            IEnumerable<EnumValue.PropertySatus> propertyStatusValues = System.Enum.GetValues(typeof(EnumValue.PropertySatus)).Cast<EnumValue.PropertySatus>();
            List<SelectListItem> PropertyTypeList = new List<SelectListItem>();
            PropertyTypeList.Add(new SelectListItem() { Text = "-- All --", Value = "0", Selected = false });
            PropertyTypeList.Add(new SelectListItem() { Text = "Exclusive Residential", Value = "1", Selected = false });
            PropertyTypeList.Add(new SelectListItem() { Text = "Exclusive Commercial", Value = "2", Selected = true });
            PropertyTypeList.Add(new SelectListItem() { Text = "Exclusive Condo", Value = "9", Selected = false });
            PropertyTypeList.Add(new SelectListItem() { Text = "New/Hot Commercial", Value = "3", Selected = false });
            PropertyTypeList.Add(new SelectListItem() { Text = "New/Hot Residential", Value = "4", Selected = false });
            PropertyTypeList.Add(new SelectListItem() { Text = "New/Hot Condo", Value = "8", Selected = false });
            PropertyTypeList.Add(new SelectListItem() { Text = "Looking For Commercial", Value = "5", Selected = true });
            PropertyTypeList.Add(new SelectListItem() { Text = "Looking For Residential", Value = "6", Selected = false });
            PropertyTypeList.Add(new SelectListItem() { Text = "Looking For Condo", Value = "7", Selected = false });
            ViewBag.PropertyType = new SelectList(PropertyTypeList, "Value", "Text");

            return View(PropertyListModel);
        }
        public List<PropertyModel> GetPropertyList(List<Entity.Property> Properties)
        {
            List<PropertyModel> PropertyModelList = new List<PropertyModel>();
            Mapper.CreateMap<CommunicationApp.Entity.Property, CommunicationApp.Web.Models.PropertyModel>();
            foreach (var Property in Properties)
            {
                CommunicationApp.Web.Models.PropertyModel Propertymodel = Mapper.Map<CommunicationApp.Entity.Property, CommunicationApp.Web.Models.PropertyModel>(Property);
                var PropertyImages = _PropertyImageService.GetPropertyImages().Where(c => c.PropertyId == Property.PropertyId).ToList();


                // passing 'SaleOfBusinessType' to return model according to PropertyType.
                if (Property.PropertyStatusId != null)
                {
                    if (Property.PropertyStatusId == (int)EnumValue.PropertySatus.ExclusiveCommercial)
                    {
                        Propertymodel.TypeOfProperty = EnumValue.GetEnumDescription(EnumValue.PropertySatus.ExclusiveCommercial);
                    }
                    else if (Property.PropertyStatusId == (int)EnumValue.PropertySatus.ExclusiveResidential)
                    {
                        Propertymodel.TypeOfProperty = EnumValue.GetEnumDescription(EnumValue.PropertySatus.ExclusiveResidential);
                    }
                    else if (Property.PropertyStatusId == (int)EnumValue.PropertySatus.ExclusiveResidentialCondo)
                    {
                        Propertymodel.TypeOfProperty = EnumValue.GetEnumDescription(EnumValue.PropertySatus.ExclusiveResidentialCondo);
                    }
                    else if (Property.PropertyStatusId == (int)EnumValue.PropertySatus.LookingForCommercial)
                    {
                        Propertymodel.TypeOfProperty = EnumValue.GetEnumDescription(EnumValue.PropertySatus.LookingForCommercial);
                    }
                    else if (Property.PropertyStatusId == (int)EnumValue.PropertySatus.LookingForResidential)
                    {
                        Propertymodel.TypeOfProperty = EnumValue.GetEnumDescription(EnumValue.PropertySatus.LookingForResidential);
                    }
                    else if (Property.PropertyStatusId == (int)EnumValue.PropertySatus.LookingForResidentialCondo)
                    {
                        Propertymodel.TypeOfProperty = EnumValue.GetEnumDescription(EnumValue.PropertySatus.LookingForResidentialCondo);
                    }
                    else if (Property.PropertyStatusId == (int)EnumValue.PropertySatus.NewHotListingCommercial)
                    {
                        Propertymodel.TypeOfProperty = EnumValue.GetEnumDescription(EnumValue.PropertySatus.NewHotListingCommercial);
                    }
                    else if (Property.PropertyStatusId == (int)EnumValue.PropertySatus.NewHotListingResidential)
                    {
                        Propertymodel.TypeOfProperty = EnumValue.GetEnumDescription(EnumValue.PropertySatus.NewHotListingResidential);
                    }
                    else if (Property.PropertyStatusId == (int)EnumValue.PropertySatus.NewHotListingResidentialCondo)
                    {
                        Propertymodel.TypeOfProperty = EnumValue.GetEnumDescription(EnumValue.PropertySatus.NewHotListingResidentialCondo);
                    }


                }
                List<PropertyImages> PropertyImagesList = new List<PropertyImages>();
                foreach (var PropertyImage in PropertyImages)
                {
                    PropertyImages PropertyImg = new PropertyImages();
                    PropertyImg.imagelist = PropertyImage.ImagePath;
                    PropertyImagesList.Add(PropertyImg);
                }
                Propertymodel.PropertyPhotolist = PropertyImagesList;
                var customer = _CustomerService.GetCustomer(Convert.ToInt32(Property.CustomerId));
                Propertymodel.CustomerName = customer.FirstName;
                Propertymodel.CustomerTrebId = customer.TrebId;
                Propertymodel.CustomerPhoto = customer.PhotoPath;
                PropertyModelList.Add(Propertymodel);
            }

            return PropertyModelList.ToList();
        }
        public JsonResult UpdateProperty(string propertyId, string IsCheked)
        {
            try
            {
                //For get Property For StatusId with sql.
                CommonClass CommonClass = new CommonClass();
                string QStr = "";
                DataTable dt = new DataTable();
                QStr = "Select PropertyStatusId, IsActive From Property where PropertyId=" + propertyId;
                dt = CommonClass.GetDataSet(QStr).Tables[0];
                List<CustomerModel> CustomerList = new List<CustomerModel>();
                var PropertyForStatusId = Convert.ToInt32(dt.Rows[0]["PropertyStatusId"]);
                var IsActive = Convert.ToBoolean(dt.Rows[0]["IsActive"]);
                //End

                var Status = "";
                var Property = _PropertyService.GetProperty(Convert.ToInt32(propertyId));
                if (Property != null)
                {
                    if (IsCheked == "Yes")
                    {
                        if (IsActive == false)
                        {
                            Property.IsActive = true;
                            Status = "Yes";
                            PropertyModel PropertyModel = new Web.Models.PropertyModel();
                            if (PropertyForStatusId == (int)EnumValue.PropertySatus.ExclusiveCommercial)
                            {
                                PropertyModel.PropertyTypeStatus = EnumValue.GetEnumDescription(EnumValue.PropertySatus.ExclusiveCommercial);
                            }
                            else if (PropertyForStatusId == (int)EnumValue.PropertySatus.ExclusiveResidential)
                            {
                                PropertyModel.PropertyTypeStatus = EnumValue.GetEnumDescription(EnumValue.PropertySatus.ExclusiveResidential);
                            }
                            else if (PropertyForStatusId == (int)EnumValue.PropertySatus.ExclusiveResidentialCondo)
                            {
                                PropertyModel.PropertyTypeStatus = EnumValue.GetEnumDescription(EnumValue.PropertySatus.ExclusiveResidentialCondo);
                            }
                            else if (PropertyForStatusId == (int)EnumValue.PropertySatus.LookingForCommercial)
                            {
                                PropertyModel.PropertyTypeStatus = EnumValue.GetEnumDescription(EnumValue.PropertySatus.LookingForCommercial);
                            }
                            else if (PropertyForStatusId == (int)EnumValue.PropertySatus.LookingForResidential)
                            {
                                PropertyModel.PropertyTypeStatus = EnumValue.GetEnumDescription(EnumValue.PropertySatus.LookingForResidential);
                            }
                            else if (PropertyForStatusId == (int)EnumValue.PropertySatus.LookingForResidentialCondo)
                            {
                                PropertyModel.PropertyTypeStatus = EnumValue.GetEnumDescription(EnumValue.PropertySatus.LookingForResidentialCondo);
                            }
                            else if (PropertyForStatusId == (int)EnumValue.PropertySatus.NewHotListingCommercial)
                            {
                                PropertyModel.PropertyTypeStatus = EnumValue.GetEnumDescription(EnumValue.PropertySatus.NewHotListingCommercial);
                            }
                            else if (PropertyForStatusId == (int)EnumValue.PropertySatus.NewHotListingResidential)
                            {
                                PropertyModel.PropertyTypeStatus = EnumValue.GetEnumDescription(EnumValue.PropertySatus.NewHotListingResidential);
                            }
                            else if (PropertyForStatusId == (int)EnumValue.PropertySatus.NewHotListingResidentialCondo)
                            {
                                PropertyModel.PropertyTypeStatus = EnumValue.GetEnumDescription(EnumValue.PropertySatus.NewHotListingResidentialCondo);
                            }
                            PropertyModel.PropertyType = Property.PropertyType;
                            PropertyModel.Style = Property.Style;
                            PropertyModel.LocationPrefered = Property.LocationPrefered;
                            PropertyModel.MLS = Property.MLS;
                            PropertyModel.PropertyTypeStatus = PropertyModel.PropertyTypeStatus;
                            // PropertyModel.prop
                            var Customer = _CustomerService.GetCustomer(Convert.ToInt32(Property.CustomerId));
                            var FirstName = Customer.FirstName + " " + Customer.MiddleName + " " + Customer.LastName;
                            PropertyModel.CustomerTrebId = Customer.TrebId;
                            if (Customer != null)
                            {
                                if (Property.IsPropertyUpdated == false)
                                {


                                    SendMailToUser(FirstName, Customer.EmailId, PropertyModel);
                                }
                                else
                                {

                                    SendMailToUpdatedUser(FirstName, Customer.EmailId, PropertyModel);
                                }

                            }

                        }
                    }
                    else if (IsCheked == "No")
                    {
                        Property.IsActive = false;
                        Status = "No";
                    }

                    _PropertyService.UpdateProperty(Property);
                }

                return Json(Status);
            }


            catch (Exception ex)
            {
                ErrorLogging.LogError(ex);
                return Json("error");
            }


        }

        public JsonResult UpdateAgent(string PropertyId, string IsCheked)
        {

            try
            {
                var Status = "";
                var Agent = _AgentService.GetAgent(Convert.ToInt32(PropertyId));
                if (Agent != null)
                {
                    if (IsCheked == "Yes")
                    {
                        if (Agent.IsActive == false)
                        {
                            Agent.IsActive = true;
                            Status = "Yes";
                            PropertyModel PropertyModel = new Web.Models.PropertyModel();
                            if (Agent.AgentStatusId == (int)EnumValue.AgentSatus.AgentAvailable)
                            {
                                PropertyModel.PropertyType = EnumValue.GetEnumDescription(EnumValue.AgentSatus.AgentAvailable);
                            }
                            else if (Agent.AgentStatusId == (int)EnumValue.AgentSatus.AgentRequired)
                            {
                                PropertyModel.PropertyType = EnumValue.GetEnumDescription(EnumValue.AgentSatus.AgentRequired);
                            }
                            PropertyModel.MLS = Agent.MLS;
                            PropertyModel.PropertyTypeStatus = "Open House property";
                            var Customer = _CustomerService.GetCustomer(Convert.ToInt32(Agent.CustomerId));
                            var FirstName = Customer.FirstName + " " + Customer.MiddleName + " " + Customer.LastName;
                            if (Customer != null)
                            {
                                SendMailToUser(FirstName, Customer.EmailId, PropertyModel);
                            }
                        }
                    }
                    else if (IsCheked == "No")
                    {
                        Agent.IsActive = false;
                        Status = "No";
                    }

                    _AgentService.UpdateAgent(Agent);
                }

                return Json(Status);
            }

            catch (Exception ex)
            {
                ErrorLogging.LogError(ex);
                return Json("error");
            }

        }
        public void SessionBatches(int? customerid)
        {
            var CustomerId = 0;
            if (customerid != 0)
            {
                CustomerId = Convert.ToInt32(customerid);
            }
            else
            {
                CustomerId = Convert.ToInt32(Session["CustomerId"]);
            }
            var Customer = _CustomerService.GetCustomer(CustomerId);
            if (Customer != null)
            {
                Session["LogoPath"] = Customer.PhotoPath;
            }

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
                QStr += "(Select Count(*) From Property inner join Customer on Customer.CustomerId=Property.CustomerId  where Property.IsActive=1 and Customer.ParentId=" + CustomerId + ") AS ActivePropertyCount ,";
                QStr += "(Select Count(*) From Customer where IsAppLike=1) AS Likes ,";
                QStr += "(Select Count(*) From Customer where IsAvailable=1) AS Available, ";
                QStr += "(Select Count(*) From Customer join Brokerage on Brokerage.CustomerId=Customer.CustomerId Where ParentId=" + CustomerId + " and ApprovalDate is NULL) AS NewRiskForm";
            }
            else
            {
                QStr = "SELECT (Select Count(*) From Agent inner join Customer on Customer.CustomerId=Agent.CustomerId  where Agent.IsActive=0 ) AS DeactiveAgentCount,";
                QStr += "(Select Count(*) From Agent inner join Customer on Customer.CustomerId=Agent.CustomerId  where Agent.IsActive=1) AS ActiveAgentCount,";
                QStr += "(Select Count(*) From Customer where  IsActive=0) AS DeactiveCustomerCount,";
                QStr += "(Select Count(*) From Customer Where  IsActive=1) AS ActiveCustomerCount,";
                QStr += "(Select Count(*) From Property inner join Customer on Customer.CustomerId=Property.CustomerId  where Property.IsActive=0 ) AS DeactivePropertyCount,";
                QStr += "(Select Count(*) From Property inner join Customer on Customer.CustomerId=Property.CustomerId  where Property.IsActive=1 ) AS ActivePropertyCount ,";
                QStr += "(Select Count(*) From Customer where IsAppLike=1) AS Likes, ";
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
            Session["Likes"] = dt.Rows[0]["Likes"].ToString();
            Session["NewRiskForm"] = dt.Rows[0]["NewRiskForm"].ToString();
        }
        public ActionResult About()
        {
            ViewBag.HideSideContactUs = "F";
            ViewBag.Title = "About Us";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
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

        public void SendMailToUser(string UserName, string EmailAddress, PropertyModel PropertyType)
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
                mail.Subject = "Post approved - " + PropertyType.PropertyTypeStatus + ".";
                //string LogoPath = Common.GetURL() + "/images/logo.png";
                string msgbody = "";
                msgbody += "";
                msgbody += " <div>";
                msgbody += " <div style='float:left;width:100%;'>";
                msgbody += "<h2 style='float:left; width:100%; font-size:16px; font-family:arial; margin:0 0 10px;'>Dear " + UserName + ",</h2>";
                msgbody += "<div style='float:left;width:100%; margin:4px 0;'><p style='float:left; font-size:14px; font-family:arial; line-height:25px; margin:0 11px 0 0;'>This message refers to your post:</p> <span style='float:left; font-size:14px; font-family:arial; margin:5px 0 0 0;'><b>" + PropertyType.PropertyTypeStatus + "</b></span>";
                msgbody += " </div>";
                msgbody += "<p style='float:left;width:100%; font-size:14px; font-family:arial; line-height:25px; margin:0;'>Treb Id: " + PropertyType.CustomerTrebId + "";
                msgbody += " </p>";
                msgbody += "<p style='float:left;width:100%; font-size:14px; font-family:arial; line-height:25px; margin:0;'>Your post has been approved and published.";
                msgbody += " </p>";
                msgbody += "<div style='float:left; width:100%; margin:5px 0;'>";
                msgbody += "<span style='float:left; font-size:15px; font-family:arial; margin:12px 0 0 0; width:100%;'>Thanks</span><span style='float:left; font-size:15px; font-family:arial; margin:8px 0 0 0; width:100%;'></span>";
                msgbody += " </div>";
                msgbody += "<div style='width:auto; float:left;'>";
                msgbody += " <p style='font-family:arial; font-size:12px; font-weight:bold; margin:0px; padding:0px;'>Call: 416 844 5725 | 647 977 3268 </p>";
                msgbody += " <p style='font-family:arial; font-size:12px; font-weight:bold; margin:0px; padding:0px;'> Fax: 647 497 5646 </p>";
                msgbody += " <a style='color:red; width:100%; font-size:13px; float:left; margin:5px 0 0 0px; text-decoration:none; font-family:arial;' href='#'>Web: http://www.only4agents.com</a>";
                msgbody += " <a style='color:red; width:100%; font-size:13px; float:left; margin:5px 0 0 0px; text-decoration:none; font-family:arial;' href='#'>Email: info@only4agents.com</a>";
                msgbody += " </div>";
                msgbody += "<div style='float:left; width:100%; margin:5px 0;'>";
                //msgbody += "<img style='float:left; width:150px;' src='data:image/png;base64," + LogoBase64 + "' /> <img style='float:left; width:310px; margin:30px 0 0 30px;' src='data:image/png;base64," + ImageBase64 + "' />";
                msgbody += "<img style='float:left; width:300px;' src='" + Logourl + "' /> ";


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

        public void SendMailToAdmin(string UserName, string EmailAddress, string PropertyType)
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
                mail.Subject = "Your property approved by admin.  ";
                //string LogoPath = Common.GetURL() + "/images/logo.png";
                string msgbody = "";
                msgbody += "";
                msgbody += " <div>";
                msgbody += " <div style='float:left;width:100%;'>";
                msgbody += "<h2 style='float:left; width:100%; font-size:16px; font-family:arial; margin:0 0 10px;'>Dear " + UserName + ",</h2>";
                msgbody += "<div style='float:left;width:100%; margin:4px 0;'><p style='float:left; font-size:14px; font-family:arial; line-height:25px; margin:0 11px 0 0;'>This message refers to your post :</p> <span style='float:left; font-size:16px; font-family:arial; margin:10px 0 0 0;'>" + PropertyType + "</span>";
                msgbody += " </div>";
                msgbody += "<p style='float:left;width:100%; font-size:14px; font-family:arial; line-height:25px; margin:0;'>Your post is now in review on Homelife Miracle App.</p>";
                msgbody += "<p style='float:left;width:100%; font-size:14px; font-family:arial; line-height:25px; margin:0;'>Your post has been approved and published. You can see your post on Homelife Miracle App .";
                msgbody += " </p>";
                msgbody += "<div style='float:left; width:100%; margin:5px 0;'>";
                msgbody += "<span style='float:left; font-size:15px; font-family:arial; margin:12px 0 0 0; width:100%;'>Thanks</span><span style='float:left; font-size:15px; font-family:arial; margin:8px 0 0 0; width:100%;'></span>";
                msgbody += " </div>";
                msgbody += "<div style='width:auto; float:left;'>";
                msgbody += " <p style='font-family:arial; font-size:12px; font-weight:bold; margin:0px; padding:0px;'>Call: 416 844 5725 | 647 977 3268 </p>";
                msgbody += " <p style='font-family:arial; font-size:12px; font-weight:bold; margin:0px; padding:0px;'> Fax: 647 497 5646 </p>";
                msgbody += " <a style='color:red; width:100%; font-size:13px; float:left; margin:5px 0 0 0px; text-decoration:none; font-family:arial;' href='#'>Web: http://www.only4agents.com</a>";
                msgbody += " <a style='color:red; width:100%; font-size:13px; float:left; margin:5px 0 0 0px; text-decoration:none; font-family:arial;' href='#'>Email: info@only4agents.com</a>";
                msgbody += " </div>";
                msgbody += "<div style='float:left; width:100%; margin:5px 0;'>";
                //msgbody += "<img style='float:left; width:150px;' src='data:image/png;base64," + LogoBase64 + "' /> <img style='float:left; width:310px; margin:30px 0 0 30px;' src='data:image/png;base64," + ImageBase64 + "' />";
                msgbody += "<img style='float:left; width:300px;' src='" + Logourl + "' /> ";


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

        public void SendMailToUpdatedUser(string UserName, string EmailAddress, PropertyModel PropertyType)
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
                mail.Subject = "Property update - Approved";
                //string LogoPath = Common.GetURL() + "/images/logo.png";
                string msgbody = "";
                msgbody += "";
                msgbody += " <div>";
                msgbody += " <div style='float:left;width:100%;'>";
                msgbody += "<h2 style='float:left; width:100%; font-size:16px; font-family:arial; margin:0 0 10px;'>Dear " + UserName + ",</h2>";
                msgbody += "<div style='float:left;width:100%; margin:4px 0;'><p style='float:left; font-size:14px; font-family:arial; line-height:25px; margin:0 11px 0 0;'>Treb Id:</p> <span style='float:left; font-size:14px; font-family:arial; margin:5px 0 0 0;'><b>" + PropertyType.CustomerTrebId + "</b></span>";
                msgbody += " </div>";
                msgbody += "<div style='float:left;width:100%; margin:4px 0;'><p style='float:left; font-size:14px; font-family:arial; line-height:25px; margin:0 11px 0 0;'>This message refers to your post:</p> <span style='float:left; font-size:14px; font-family:arial; margin:5px 0 0 0;'><b>" + PropertyType.PropertyTypeStatus + "</b></span>";
                msgbody += " </div>";
                msgbody += "<p style='float:left;width:100%; font-size:14px; font-family:arial; line-height:25px; margin:0;'>Your post has been approved and published.";
                msgbody += " </p>";
                msgbody += "<div style='float:left; width:100%; margin:5px 0;'>";
                msgbody += "<span style='float:left; font-size:15px; font-family:arial; margin:12px 0 0 0; width:100%;'>Thanks</span><span style='float:left; font-size:15px; font-family:arial; margin:8px 0 0 0; width:100%;'></span>";
                msgbody += " </div>";
                msgbody += "<div style='width:auto; float:left;'>";
                msgbody += " <p style='font-family:arial; font-size:12px; font-weight:bold; margin:0px; padding:0px;'>Call: 416 844 5725 | 647 977 3268 </p>";
                msgbody += " <p style='font-family:arial; font-size:12px; font-weight:bold; margin:0px; padding:0px;'> Fax: 647 497 5646 </p>";
                msgbody += " <a style='color:red; width:100%; font-size:13px; float:left; margin:5px 0 0 0px; text-decoration:none; font-family:arial;' href='#'>Web: http://www.only4agents.com</a>";
                msgbody += " <a style='color:red; width:100%; font-size:13px; float:left; margin:5px 0 0 0px; text-decoration:none; font-family:arial;' href='#'>Email: info@only4agents.com</a>";
                msgbody += " </div>";
                msgbody += "<div style='float:left; width:100%; margin:5px 0;'>";
                //msgbody += "<img style='float:left; width:150px;' src='data:image/png;base64," + LogoBase64 + "' /> <img style='float:left; width:310px; margin:30px 0 0 30px;' src='data:image/png;base64," + ImageBase64 + "' />";
                msgbody += "<img style='float:left; width:300px;' src='" + Logourl + "' /> ";


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