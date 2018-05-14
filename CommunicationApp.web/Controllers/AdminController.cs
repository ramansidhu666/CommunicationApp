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
using System.Web.Configuration;
using System.Web.Mvc;
using System.Drawing;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace CommunicationApp.Web.Controllers
{
    public class AdminController : BaseController
    {

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
        public IViewsService _ViewsService { get; set; }
        public IFeedBackService _FeedBackService { get; set; }
        public IBannerService _BannerService { get; set; }
        public AdminController(IAgentService AgentService, IEventCustomerService EventCustomerService, IEventService EventService, ITipService TipService, IPropertyService PropertyService, IPropertyImageService PropertyImageService, IFeedBackService FeedBackService, ICompanyService CompanyService, ICountryService CountryService, IStateService StateService, ICityService CityService, IOfficeLocationService OfficeLocationService, ICustomerService CustomerService, IUserService UserService, IFormService FormService, IRoleDetailService RoleDetailService, IRoleService RoleService, IUserRoleService UserRoleService, IViewsService ViewsService, IBannerService BannerService)
            : base(CustomerService, UserService, RoleService, FormService, RoleDetailService, UserRoleService)
        {
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
            this._ViewsService = ViewsService;
            this._FeedBackService = FeedBackService;
            this._BannerService = BannerService;
            //


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
        // GET: /Admin/
        public ActionResult Index(string FirstName, string Email, string operation, string ShowMessage, string MessageBody)
        {
            UserPermissionAction("admin", RoleAction.view.ToString(), operation, ShowMessage, MessageBody);
            CheckPermission();
            var Customers = _CustomerService.GetCustomers().Where(x => x.Designation == "Admin" && x.IsActive == true);
            var User = _UserService.GetUser(Convert.ToInt32(Session["UserId"]));
            int CompanyID = Convert.ToInt32(Session["CompanyID"]);
            if (User != null)
            {
                Session["UserName"] = User.UserName;
                Session["AdminPhoto"] = "";
            }

            var models = new List<AdminModel>();
            //For First Name
            if (!string.IsNullOrEmpty(FirstName))
            {
                Customers = Customers.Where(c => c.FirstName.ToLower().Contains(FirstName.ToLower()));
            }
            //For EmailID
            if (!string.IsNullOrEmpty(Email))
            {
                Customers = Customers.Where(c => c.EmailId.ToLower().Contains(Email.ToLower()));
            }

            Mapper.CreateMap<CommunicationApp.Entity.Customer, CommunicationApp.Models.AdminModel>();
            foreach (var Customer in Customers.OrderByDescending(c => c.CustomerId))
            {
                var _user = Mapper.Map<CommunicationApp.Entity.Customer, CommunicationApp.Models.AdminModel>(Customer);

                models.Add(_user);
            }

            SessionBatches();
            return View(models);

        }

        public ActionResult Create()
        {
            UserPermissionAction("admin", RoleAction.create.ToString());
            CheckPermission();
            AdminModel AdminModel = new CommunicationApp.Models.AdminModel();
            ViewBag.Countrylist = new SelectList(_CountryService.GetCountries(), "CountryID", "CountryName");
            ViewBag.Citylist = new SelectList(_CityService.GetCities(), "CityID", "CityName");
            ViewBag.UserId = new SelectList(_UserService.GetUsers(), "UserId", "FirstName");
            return View(AdminModel);
        }


        [HttpPost]
        public ActionResult Create([Bind(Include = "CustomerId,TrebId,WebsiteUrl,ApplicationID,Password,CompanyID,UserId,PhotoPath,FirstName,LastName,MiddleName,EmailID,MobileNo,CountryID,StateID,CityID,ZipCode,Latitude,Longitude,CreatedOn,LastUpdatedOn,MobileVerifyCode,EmailVerifyCode,IsMobileVerified,IsEmailVerified,IsActive,AdminCompanyAddress")] AdminModel AdminModel, HttpPostedFileBase file, HttpPostedFileBase Logo)
        {
            TempData["ShowMessage"] = "";
            TempData["MessageBody"] = "";
            UserPermissionAction("admin", RoleAction.create.ToString());
            CheckPermission();
            Mapper.CreateMap<CommunicationApp.Models.AdminModel, CommunicationApp.Entity.Customer>();
            CommunicationApp.Entity.Customer Customer = Mapper.Map<CommunicationApp.Models.AdminModel, CommunicationApp.Entity.Customer>(AdminModel);

            if (ModelState.IsValid)
            {
                var customerFound = _CustomerService.GetCustomers().Where(x => x.EmailId == Customer.EmailId || x.MobileNo == Customer.MobileNo || x.WebsiteUrl == Customer.WebsiteUrl).FirstOrDefault();
                if (customerFound == null)
                {

                    //Save Company of admin

                    Company Company = new Entity.Company();
                    Company.WebSite = Customer.WebsiteUrl;
                    Company.CountryID = 1;
                    Company.StateID = 1;
                    Company.CityID = 1;
                    Company.CompanyAddress = AdminModel.AdminCompanyAddress != "" ? AdminModel.AdminCompanyAddress : "Company@gmail.com";
                    Company.CompanyName = AdminModel.CompanyName != null ? AdminModel.CompanyName : "Company";
                    Company.EmailID = "Company@gmail.com";
                    Company.PhoneNo = "123456987";
                    Company.IsActive = true;
                    if (Logo != null)
                    {
                        Company.LogoPath = Savefile(Logo);
                    }

                    _CompanyService.InsertCompany(Company);

                    //Insert User first
                    CommunicationApp.Entity.User user = new CommunicationApp.Entity.User();
                    //user.UserId =0; //New Case
                    user.FirstName = Customer.FirstName;
                    user.TrebId = Customer.TrebId;
                    user.LastName = Customer.LastName;
                    user.UserName = Customer.EmailId;
                    user.Password = SecurityFunction.EncryptString(AdminModel.Password);
                    user.UserEmailAddress = Customer.EmailId;
                    user.CompanyID = Company.CompanyID;
                    user.CreatedOn = DateTime.Now;
                    user.LastUpdatedOn = DateTime.Now;
                    user.TrebId = "0000000";
                    user.IsActive = true;
                    _UserService.InsertUser(user);
                    //End : Insert User first

                    var UserID = user.UserId;
                    if (user.UserId > 0)
                    {
                        //Insert User Role
                        CommunicationApp.Entity.UserRole userRole = new CommunicationApp.Entity.UserRole();
                        userRole.UserId = user.UserId;
                        userRole.RoleId = 2; //By Default set new Admin/user role id=2
                        _UserRoleService.InsertUserRole(userRole);
                        //End : Insert User Role

                        //Insert the Customer
                        Customer.FirstName = Customer.FirstName;
                        Customer.UserId = user.UserId;
                        Customer.Designation = "Admin";
                        Customer.MobileVerifyCode = CommonCls.GetNumericCode();
                        Customer.EmailVerifyCode = CommonCls.GetNumericCode();
                        Customer.MobileVerifyCode = "9999";
                        Customer.EmailVerifyCode = "9999";
                        Customer.CreatedOn = DateTime.Now;
                        Customer.CompanyID = Company.CompanyID;
                        Customer.Address = "";
                        Customer.ZipCode = "";
                        Customer.IsEmailVerified = true;

                        var PhotoPath = "";
                        if (file != null)
                        {
                            PhotoPath = Savefile(file);
                        }

                        Customer.PhotoPath = PhotoPath;
                        Customer.IsMobileVerified = false;
                        Customer.ApplicationId = AdminModel.ApplicationId;
                        Customer.DeviceSerialNo = AdminModel.DeviceSerialNo;
                        Customer.DeviceType = AdminModel.DeviceType;
                        Customer.IsUpdated = false;
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
                        _CustomerService.InsertCustomer(Customer);

                        var CustomerID = Customer.CustomerId.ToString();
                        AdminModel.CustomerId = Customer.CustomerId;
                        TempData["ShowMessage"] = "Success";
                        TempData["MessageBody"] = "Admin successfully register.";

                    }

                }
                else
                {

                    if (customerFound.EmailId == AdminModel.EmailID)
                    {
                        TempData["ShowMessage"] = "Error";
                        TempData["MessageBody"] = "Email is already exist.";
                    }
                    else if (customerFound.MobileNo == AdminModel.MobileNo)
                    {
                        TempData["ShowMessage"] = "Error";
                        TempData["MessageBody"] = "MobileNos is already exist.";
                    }
                    else
                    {
                        TempData["ShowMessage"] = "Error";
                        TempData["MessageBody"] = "Some error occured.";
                    }

                }
            }
            else
            {
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Key, x.Value.Errors }).ToArray();
                var modelStateErrors = this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors);
                TempData["ShowMessage"] = "Error";
                TempData["MessageBody"] = "Please fill the required data.";
                return View(AdminModel);
            }


            return RedirectToAction("Index");
        }

        // GET: /Customer/Edit/5
        public ActionResult Edit(int id)
        {
            AdminModel AdminModel = new CommunicationApp.Models.AdminModel();
            var Customer = _CustomerService.GetCustomers().Where(c => c.CustomerId == id).FirstOrDefault();
            if (Customer != null)
            {
                var models = new List<AdminModel>();
                Mapper.CreateMap<CommunicationApp.Entity.Customer, CommunicationApp.Models.AdminModel>();
                AdminModel = Mapper.Map<CommunicationApp.Entity.Customer, CommunicationApp.Models.AdminModel>(Customer);
                var User = _UserService.GetUser(Customer.UserId);
                var Company = _CompanyService.GetCompany(Customer.CompanyID);
                if (Company != null)
                {
                    AdminModel.AdminCompanyLogo = Company.LogoPath;
                    AdminModel.CompanyName = Company.CompanyName;
                    AdminModel.AdminCompanyAddress = Company.CompanyAddress;
                }
                if (User != null)
                {
                    AdminModel.Password = SecurityFunction.DecryptString(User.Password);
                }
                if (AdminModel.PhotoPath != null && AdminModel.PhotoPath != "")
                {
                    AdminModel.PhotoPath = AdminModel.PhotoPath;
                }
                else
                {
                    AdminModel.PhotoPath = CommonCls.GetURL() + "/images/noImage.jpg";
                }
            }


            ViewBag.CityID = (Customer.CityID <= 0 ? "" : Customer.CityID.ToString());
            ViewBag.StateID = (Customer.StateID <= 0 ? "" : Customer.StateID.ToString());
            ViewBag.Countrylist = new SelectList(_CountryService.GetCountries(), "CountryID", "CountryName", Customer.CountryID);
            ViewBag.Citylist = new SelectList(_CityService.GetCities(), "CityID", "CityName", Customer.CityID);
            ViewBag.UserId = new SelectList(_UserService.GetUsers(), "UserId", "FirstName", Customer.UserId);

            return View(AdminModel);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit([Bind(Include = "CustomerId,TrebId,WebsiteUrl,ApplicationID,Password,CompanyID,CompanyName,UserId,PhotoPath,AdminCompanyLogo,FirstName,LastName,MiddleName,EmailID,MobileNo,CountryID,StateID,CityID,ZipCode,Latitude,Longitude,CreatedOn,LastUpdatedOn,MobileVerifyCode,EmailVerifyCode,IsMobileVerified,IsEmailVerified,IsActive,AdminCompanyAddress")] AdminModel AdminModel, HttpPostedFileBase file, HttpPostedFileBase Logo)
        {
            UserPermissionAction("property", RoleAction.view.ToString());
            CheckPermission();
            TempData["ShowMessage"] = "";
            TempData["MessageBody"] = "";
            try
            {

                if (ModelState.IsValid)
                {
                    var CustomerFound = _CustomerService.GetCustomers().Where(c => ((c.EmailId.Trim() == AdminModel.EmailID.Trim() || c.MobileNo.Trim() == AdminModel.MobileNo.Trim()) && c.CustomerId != AdminModel.CustomerId)).FirstOrDefault();
                    if (CustomerFound == null)
                    {
                        var PhotoPath = "";
                        var CompanyLogo = "";
                        var CustomerUpdate = _CustomerService.GetCustomer(AdminModel.CustomerId);//.Where(c => c.CustomerId == AdminModel.CustomerId).FirstOrDefault();
                        if (CustomerUpdate != null)
                        {
                            var Company = _CompanyService.GetCompany(CustomerUpdate.CompanyID);
                            if (Company != null)
                            {
                                Company.CompanyName = AdminModel.CompanyName != null ? AdminModel.CompanyName : "Company";
                                Company.CompanyAddress = AdminModel.AdminCompanyAddress;
                                if (Logo != null)
                                {
                                    CompanyLogo = SaveFile(AdminModel.PhotoPath, Logo);
                                    Company.LogoPath = CompanyLogo;
                                }

                                _CompanyService.UpdateCompany(Company);
                            }

                            if (file != null)
                            {
                                PhotoPath = SaveFile(AdminModel.PhotoPath, file);
                                CustomerUpdate.PhotoPath = PhotoPath;
                            }

                            CustomerUpdate.FirstName = AdminModel.FirstName;
                            CustomerUpdate.LastName = AdminModel.LastName;
                            CustomerUpdate.MiddleName = AdminModel.MiddleName;
                            CustomerUpdate.Address = AdminModel.Address;
                            CustomerUpdate.EmailId = AdminModel.EmailID;
                            CustomerUpdate.DOB = AdminModel.DOB;
                            CustomerUpdate.MobileNo = AdminModel.MobileNo;
                            CustomerUpdate.WebsiteUrl = AdminModel.WebsiteUrl;
                            CustomerUpdate.IsActive = true;
                            if (AdminModel.Designation != null && AdminModel.Designation != "")
                            {
                                CustomerUpdate.Designation = AdminModel.Designation;
                            }
                            _CustomerService.UpdateCustomer(CustomerUpdate);

                            //Update user table.
                            var User = _UserService.GetUser(CustomerUpdate.UserId);
                            User.FirstName = AdminModel.FirstName;
                            User.LastName = AdminModel.LastName;
                            if (AdminModel != null)
                            {
                                User.Password = SecurityFunction.EncryptString(AdminModel.Password);
                                User.TrebId = "0000000000000";
                            }
                            _UserService.UpdateUser(User);
                            TempData["ShowMessage"] = "success";
                            TempData["MessageBody"] = CustomerUpdate.FirstName + " is update successfully.";
                            return RedirectToAction("Index", "Admin");
                        }
                        else
                        {
                            TempData["ShowMessage"] = "error";
                            TempData["MessageBody"] = "Customer not found.";
                            return RedirectToAction("Index", "Admin");
                        }

                    }
                    else
                    {
                        TempData["ShowMessage"] = "error";

                        if (CustomerFound.EmailId.Trim() == AdminModel.EmailID.Trim())
                        {
                            TempData["MessageBody"] = AdminModel.EmailID + " is already exists.";
                        }
                        if (CustomerFound.TrebId.Trim() == AdminModel.TrebId.Trim())
                        {
                            TempData["MessageBody"] = AdminModel.TrebId + " is already exists.";
                        }
                        if (CustomerFound.MobileNo.Trim() == AdminModel.MobileNo.Trim())
                        {
                            TempData["MessageBody"] = "This" + " " + AdminModel.MobileNo + " is already exists.";
                        }
                        else
                        {
                            TempData["MessageBody"] = "Please fill the required field with valid data";
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                CommonCls.ErrorLog(ex.ToString());
                TempData["ShowMessage"] = "error";
                TempData["MessageBody"] = "Some unknown problem occured while proccessing save operation on " + AdminModel.FirstName + " client";

            }
            var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Key, x.Value.Errors }).ToArray();
            var modelStateErrors = this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors);
            ViewBag.UserId = new SelectList(_UserService.GetUsers(), "UserId", "FirstName", AdminModel.UserId);
            ViewBag.CityID = (AdminModel.CityID <= 0 ? "" : AdminModel.CityID.ToString());
            ViewBag.StateID = (AdminModel.StateID <= 0 ? "" : AdminModel.StateID.ToString());
            ViewBag.Countrylist = new SelectList(_CountryService.GetCountries(), "CountryID", "CountryName", AdminModel.CountryID);
            ViewBag.Statelist = new SelectList(_StateService.GetStates(), "StateID", "StateName", AdminModel.StateID);
            ViewBag.Citylist = new SelectList(_CityService.GetCities(), "CityID", "CityName", AdminModel.CityID);


            return View(AdminModel);
        }

        public ActionResult DeleteAdmin(int id)
        {

            try
            {
                //Delete Customer
                var Customers = _CustomerService.GetCustomers().Where(c => c.CustomerId == id).ToList();
                foreach (var Customer in Customers)
                {

                    //Delete Agent
                    var Agents = _AgentService.GetAgents().Where(c => c.CustomerId == Customer.CustomerId).ToList();
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
                    var Properties = _PropertyService.GetPropertys().Where(c => c.CustomerId == Customer.CustomerId).ToList();
                    foreach (var Propertie in Properties)
                    {
                        var PropertieImages = _PropertyImageService.GetPropertyImages().Where(c => c.AgentId == Propertie.PropertyId).ToList();
                        foreach (var propertyImage in PropertieImages)
                        {
                            _PropertyImageService.DeletePropertyImage(propertyImage);
                        }

                        //Delete Views
                        var Views = _ViewsService.GetViewss().Where(c => c.PropertyId == Propertie.PropertyId || c.CustomerId == Customer.CustomerId).ToList();
                        foreach (var View in Views)
                        {
                            _ViewsService.DeleteViews(View);
                        }
                        _PropertyService.DeleteProperty(Propertie);
                    }

                    //Delete Tip
                    var Tips = _TipService.GetTips().Where(c => c.CustomerId == Customer.CustomerId).ToList();
                    foreach (var Tip in Tips)
                    {
                        _TipService.DeleteTip(Tip);
                    }

                    //Delete Feedback

                    var Feedbacks = _FeedBackService.GetFeedBacks().Where(c => c.CustomerId == Customer.CustomerId).ToList();
                    foreach (var Feedback in Feedbacks)
                    {
                        _FeedBackService.DeleteFeedBack(Feedback);
                    }
                    //Delete EventCustomer
                    var EventCustomers = _EventCustomerService.GetEventCustomers().Where(c => c.CustomerId == Customer.CustomerId).ToList();
                    foreach (var EventCustomer in EventCustomers)
                    {
                        _EventCustomerService.DeleteEventCustomer(EventCustomer);
                    }
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
                return RedirectToAction("Index");
            }
            catch
            {

            }
            TempData["ShowMessage"] = "success";
            TempData["MessageBody"] = "Customer succesfully deleted.";
            return RedirectToAction("Index");
        }

        public ActionResult BannerDetail( string operation, string ShowMessage, string MessageBody,int? id)
        {
            UserPermissionAction("admin", RoleAction.view.ToString(), operation, ShowMessage, MessageBody);
            CheckPermission();
            List<BannerModel> BannerModelList = new List<BannerModel>();
            var Banners = _BannerService.GetBanners().Where(c => c.CustomerId == id);
          
            Mapper.CreateMap<CommunicationApp.Entity.Banner, CommunicationApp.Web.Models.BannerModel>();
            foreach (var Banner in Banners)
            {
                var _banner = Mapper.Map<CommunicationApp.Entity.Banner, CommunicationApp.Web.Models.BannerModel>(Banner);

                BannerModelList.Add(_banner);
            }

            SessionBatches();
            return View(BannerModelList);

        }

        public ActionResult CreateBanner()
        {
            UserPermissionAction("admin", RoleAction.create.ToString());
            CheckPermission();
            BannerModel BannerModel = new CommunicationApp.Web.Models.BannerModel();

            return View(BannerModel);
        }
        [HttpPost]
        public ActionResult CreateBanner([Bind(Include = "BannerId,Url,CreatedOn,LastUpdatedOn,IsActive")] BannerModel BannerModel, HttpPostedFileBase file)
        {
            TempData["ShowMessage"] = "";
            TempData["MessageBody"] = "";
            UserPermissionAction("admin", RoleAction.create.ToString());
            CheckPermission();
            Mapper.CreateMap<CommunicationApp.Web.Models.BannerModel, CommunicationApp.Entity.Banner>();
            CommunicationApp.Entity.Banner banner = Mapper.Map<CommunicationApp.Web.Models.BannerModel, CommunicationApp.Entity.Banner>(BannerModel);

            if (ModelState.IsValid)
            {                
                if (file != null)
                {
                    banner.Url = Savefile(file);
                }
                banner.CustomerId = Convert.ToInt32(Session["CustomerId"]);
                _BannerService.InsertBanner(banner);

                TempData["ShowMessage"] = "Success";
                TempData["MessageBody"] = "banner successfully saved.";

            }

            else
            {
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Key, x.Value.Errors }).ToArray();
                var modelStateErrors = this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors);
                TempData["ShowMessage"] = "Error";
                TempData["MessageBody"] = "Please fill the required data.";
                return View(BannerModel);
            }
            int id = Convert.ToInt32(Session["CustomerId"]);
            return RedirectToAction("BannerDetail/" + id + "");
        }

        public ActionResult DeleteBanner(int? id)
        {
            UserPermissionAction("admin", RoleAction.create.ToString());
            CheckPermission();
            var Banner = _BannerService.GetBanner(Convert.ToInt32(id));
            if (Banner!=null)
            {
                _BannerService.DeleteBanner(Banner);
            }
            id = Convert.ToInt32(Session["CustomerId"]);
            return RedirectToAction("BannerDetail/"+id+"");
        }
        public string SaveFile(string PhotoPath, HttpPostedFileBase file)
        {
            if (PhotoPath != "")
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

            return CommonCls.GetURL() + "/CustomerPhoto/" + fileName;
        }

        public string SaveImage(string Base64String)
        {
            string fileName = Guid.NewGuid() + ".png";
            Image image = CommonCls.Base64ToImage(Base64String);
            var subPath = Server.MapPath("~/CustomerPhoto");
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
                var subPath = Server.MapPath("~/CustomerPhoto");
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
        public string Savefile(HttpPostedFileBase file)
        {
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

            return CommonCls.GetURL() + "/CustomerPhoto/" + fileName;

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
        public IEnumerable<object> Customers { get; set; }
    }
}