using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CommunicationApp.Entity;
using CommunicationApp.Data;
using CommunicationApp.Services;
using CommunicationApp.Controllers;
using CommunicationApp.Models;
using CommunicationApp.Infrastructure;
using AutoMapper;
using CommunicationApp.Core.Infrastructure;

namespace CommunicationApp.Web.Controllers
{
    public class UserController : BaseController
    {
        public ICityService _CityService { get; set; }
        public IStateService _StateService { get; set; }
        public ICountryService _CountryService { get; set; }
        public ICompanyService _CompanyService { get; set; }

        public UserController(ICustomerService CustomerService,IUserService UserService, ICompanyService CompanyService, ICountryService CountryService, IStateService StateService, ICityService CityService, IRoleService RoleService, IFormService FormService, IRoleDetailService RoleDetailService, IUserRoleService UserRoleService)
            : base( CustomerService,UserService, RoleService, FormService, RoleDetailService, UserRoleService)
        

        {
            this._UserService = UserService;
            this._StateService = StateService;
            this._CompanyService = CompanyService;

        }
        private void CheckPermission()
        {
            RoleDetailModel userRole = UserPermission("User");
            TempData["View"] = userRole.IsView;
            TempData["Create"] = userRole.IsCreate;
            TempData["Edit"] = userRole.IsEdit;
            TempData["Delete"] = userRole.IsDelete;
            TempData["Detail"] = userRole.IsDetail;
        }
        // GET: /User/
        public ActionResult Index(string FirstName, string Email, string operation, string ShowMessage, string MessageBody)
        {
            UserPermissionAction("user", RoleAction.view.ToString(), operation, ShowMessage, MessageBody);
            CheckPermission();

            int CompanyID = Convert.ToInt32(Session["CompanyID"]);
            ViewBag.CompanyID = new SelectList(_CompanyService.GetCompanies(), "CompanyID", "CompanyName", CompanyID.ToString());

            var users = _UserService.GetUsers().Where(c => c.UserName.ToLower() != "administrator" && c.CompanyID == CompanyID);//db.Users.Where(z => z.UserName.ToLower() != "administrator" && z.CompanyID == CompanyID);
            var models = new List<UserModel>();

            //For First Name
            if (!string.IsNullOrEmpty(FirstName))
            {
                users =users.Where(c => c.FirstName.ToLower().Contains(FirstName.ToLower()));
            }
            //For EmailID
            if(!string.IsNullOrEmpty(Email))
            {
                users = users.Where(c => c.UserEmailAddress.ToLower().Contains(Email.ToLower()));
            }

            Mapper.CreateMap<CommunicationApp.Entity.User, CommunicationApp.Models.UserModel>();
            foreach (var user in users)
            {
                var _user = Mapper.Map<CommunicationApp.Entity.User, CommunicationApp.Models.UserModel>(user);
                models.Add(_user);
            }


            return View(models);
        }



        // GET: /User/Details/5
        public ActionResult Details(int id)
        {
            UserPermissionAction("user", RoleAction.detail.ToString());
            CheckPermission();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = _UserService.GetUserById(id);// db.Users.Find(id);
            Mapper.CreateMap<CommunicationApp.Entity.User, CommunicationApp.Models.UserModel>();
            CommunicationApp.Models.UserModel usermodel = Mapper.Map<CommunicationApp.Entity.User, CommunicationApp.Models.UserModel>(user);
            usermodel.Password = SecurityFunction.DecryptString(user.Password);
            if (usermodel == null)
            {
                return HttpNotFound();
            }
            return View(usermodel);
        }

        public JsonResult CheckUserEmailID(string EmailID)
        {

            var email = _UserService.GetUsers().Where(C => C.UserEmailAddress == EmailID);

            //Boolean flag = true;

            if (email.Count() > 0)
                return Json(true, JsonRequestBehavior.AllowGet);

            return Json(false, JsonRequestBehavior.AllowGet);
        }

        // GET: /User/Create
        public ActionResult Create()
        {
            UserPermissionAction("user", RoleAction.create.ToString());
            CheckPermission();
            int CompanyID = Convert.ToInt32(Session["CompanyID"]);
            ViewBag.CompanyID = new SelectList(_CompanyService.GetCompanies(), "CompanyID", "CompanyName", CompanyID.ToString());

            return View();
        }


        // POST: /User/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserId,UserName,FirstName,Password,UserEmailAddress,CompanyID,IsActive")] UserModel usermodel)
        {
            UserPermissionAction("user", RoleAction.create.ToString());
            CheckPermission();

            try
            {
                TempData["ShowMessage"] = "error";
                TempData["MessageBody"] = "Please fill the required field with valid data";
                ViewBag.CompanyID = new SelectList(_CompanyService.GetCompanies(), "CompanyID", "CompanyName", usermodel.CompanyID);

                if (ModelState.IsValid)
                {
                    User objUser = _UserService.GetUsers().Where(c => c.UserName.ToLower() == usermodel.UserName.ToLower() || c.UserEmailAddress.ToLower() == usermodel.UserEmailAddress.ToLower()).FirstOrDefault();//db.Users.Where(x => x.UserName.ToLower() == users.UserName.ToLower() || x.UserEmailAddress.ToLower() == users.UserEmailAddress.ToLower()).FirstOrDefault();
                    if (objUser == null)
                    {
                        Mapper.CreateMap<CommunicationApp.Models.UserModel, CommunicationApp.Entity.User>();
                        CommunicationApp.Entity.User user = Mapper.Map<CommunicationApp.Models.UserModel, CommunicationApp.Entity.User>(usermodel);
                        _UserService.InsertUser(user);
                        TempData["ShowMessage"] = "success";
                        TempData["MessageBody"] = "user is saved successfully.";
                        return RedirectToAction("Index");

                    }
                    else
                    {
                        TempData["ShowMessage"] = "error";
                        if (objUser.UserName.ToLower() == objUser.UserName.ToLower()) //Check User Name
                        {
                            TempData["MessageBody"] = objUser.UserName + " is already exist.";
                        }
                        else if (objUser.UserEmailAddress.ToLower() == objUser.UserEmailAddress.ToLower()) //Check User Name
                        {
                            TempData["MessageBody"] = objUser.UserEmailAddress + " is already exist.";
                        }
                        else
                        {
                            TempData["MessageBody"] = "Some unknown problem occured while proccessing save operation on " + objUser.UserName + " user.";
                        }
                    }
                }
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Key, x.Value.Errors }).ToArray();
                var modelStateErrors = this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors);

            }
            catch (Exception ex)
            {
                TempData["ShowMessage"] = "error";
                TempData["MessageBody"] = "Some problem occured while proccessing save operation on " + usermodel.UserName + " user.";
            }


            return View(usermodel);
        }

        // GET: /User/Edit/5
        public ActionResult Edit(int id)
        {
            UserPermissionAction("user", RoleAction.edit.ToString());
            CheckPermission();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = _UserService.GetUserById(id);//db.Users.Find(id);
            Mapper.CreateMap<CommunicationApp.Entity.User, CommunicationApp.Models.UserModel>();
            CommunicationApp.Models.UserModel usermodel = Mapper.Map<CommunicationApp.Entity.User, CommunicationApp.Models.UserModel>(user);
            if (usermodel == null)
            {
                return HttpNotFound();
            }
            user.Password = SecurityFunction.EncryptString(user.Password);

            ViewBag.CompanyID = new SelectList(_CompanyService.GetCompanies(), "CompanyID", "CompanyName", user.CompanyID);
            return View(usermodel);
        }

        // POST: /User/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserId,UserName,Password,UserEmailAddress,CompanyID,IsActive")] UserModel usermodel, int id)
        {
            UserPermissionAction("user", RoleAction.edit.ToString());
            CheckPermission();

            try
            {
                ViewBag.CompanyID = new SelectList(_CompanyService.GetCompanies(), "CompanyID", "CompanyName", usermodel.CompanyID);
                TempData["ShowMessage"] = "error";
                TempData["MessageBody"] = "Please fill the required field with valid data";

                if (ModelState.IsValid)
                {
                    User objUser = _UserService.GetUsers().Where(c => c.UserName.ToLower() == usermodel.UserName.ToLower() || c.UserEmailAddress.ToLower() == usermodel.UserEmailAddress.ToLower() && c.UserId != usermodel.UserId).FirstOrDefault(); //db.Users.Where(x => (x.UserName.ToLower() == user.UserName.ToLower() || x.UserEmailAddress.ToLower() == user.UserEmailAddress.ToLower()) && x.UserId != user.UserId).FirstOrDefault();
                    if (objUser == null)
                    {
                        Mapper.CreateMap<CommunicationApp.Models.UserModel, CommunicationApp.Entity.User>();
                        CommunicationApp.Entity.User user = Mapper.Map<CommunicationApp.Models.UserModel, CommunicationApp.Entity.User>(usermodel);
                        //Update the User Info
                        User us = _UserService.GetUsers().Where(c => c.UserId == id).FirstOrDefault();//db.Users.Where(z => z.UserId == id).FirstOrDefault();
                        us.UserName = user.UserName;
                        us.Password = SecurityFunction.EncryptString(user.Password);
                        us.UserEmailAddress = user.UserEmailAddress;
                        us.CompanyID = user.CompanyID;
                        us.IsActive = user.IsActive;
                        _UserService.UpdateUser(us);
                        //db.SaveChanges();


                        TempData["ShowMessage"] = "success";
                        TempData["MessageBody"] = user.UserName + " is update successfully.";

                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["ShowMessage"] = "error";
                        if (objUser.UserName.ToLower() == usermodel.UserName.ToLower()) //Check User Name
                        {
                            TempData["MessageBody"] = usermodel.UserName + " is already exist.";
                        }
                        else if (objUser.UserEmailAddress.ToLower() == usermodel.UserEmailAddress.ToLower()) //Check User Name
                        {
                            TempData["MessageBody"] = usermodel.UserEmailAddress + " is already exist.";
                        }
                        else
                        {
                            TempData["MessageBody"] = "Some unknown problem occured while proccessing update operation on " + usermodel.UserName + " user.";
                        }
                    }
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                //Exception raise = dbEx;
                //foreach (var validationErrors in dbEx.EntityValidationErrors)
                //{
                //    foreach (var validationError in validationErrors.ValidationErrors)
                //    {
                //        string message = string.Format("{0}:{1}",
                //            validationErrors.Entry.Entity.ToString(),
                //            validationError.ErrorMessage);
                //         raise a new exception nesting  
                //         the current instance as InnerException  
                //        raise = new InvalidOperationException(message, raise);
                //    }
                //}
                //throw raise;

                TempData["ShowMessage"] = "error";
                TempData["MessageBody"] = "Some problem occured while proccessing update operation on " + usermodel.UserName + " user.";
            }
            return View(usermodel);

        }
        // GET: /User/Delete/5
        public ActionResult Delete(int id)
        {
            UserPermissionAction("user", RoleAction.delete.ToString());
            CheckPermission();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = _UserService.GetUserById(id);//db.Users.Find(id);
            Mapper.CreateMap<CommunicationApp.Entity.User, CommunicationApp.Models.UserModel>();
            CommunicationApp.Models.UserModel usermodel = Mapper.Map<CommunicationApp.Entity.User, CommunicationApp.Models.UserModel>(user);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(usermodel);
        }

        // POST: /User/Delete/5

        public ActionResult DeleteConfirmed(int id, User us)
        {
            UserPermissionAction("user", RoleAction.delete.ToString());
            CheckPermission();

            string UserName = "";
            try
            {
                User user = _UserService.GetUserById(id);//db.Users.Find(id);
                UserName = user.UserName;
                _UserService.DeleteUser(user);

                TempData["ShowMessage"] = "success";
                TempData["MessageBody"] = UserName + " is deleted successfully.";
            }
            catch (Exception ex)
            {
                if (CommonCls.ErrorLog(ex.InnerException.InnerException.Message.ToString()) == "fk")
                {
                    TempData["ShowMessage"] = "error";
                    TempData["MessageBody"] = "" + UserName + " cannot be deleted." + UserName + " is used in other pages.";
                }

                else
                {
                    TempData["ShowMessage"] = "error";
                    TempData["MessageBody"] = "Some problem occured while proccessing delete operation on " + UserName + " user.";
                }
            }
            return RedirectToAction("Index");
        }


        public ActionResult AppLikes(string FirstName, string Email, string PhoneNo, string TrebId)
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
                QStr = "Select * From Customer where IsAppLike=1  Order By IsActive asc,UpdateStatus asc,CreatedOn desc   ";
            }
            else
            {
                QStr = "Select * From Customer where IsAppLike=1 Order By IsActive asc,UpdateStatus asc,CreatedOn desc   ";
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
                CustomerEntity.CreatedOn = Convert.ToDateTime((dr["CreatedOn"]));
                if (dr["LastUpdatedOn"] != DBNull.Value)
                {

                    TimeSpan ts = DateTime.Now - Convert.ToDateTime(dr["LastUpdatedOn"]);
                    if (ts.Days > 0 && ts.Days<30)
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
                }
                CustomerEntity.IsActive = Convert.ToBoolean((dr["IsActive"]));
                CustomerEntity.IsAppLike = Convert.ToBoolean((dr["IsAppLike"]));
                if (dr["UpdateStatus"] != DBNull.Value)
                {
                    CustomerEntity.UpdateStatus = Convert.ToBoolean((dr["UpdateStatus"]));
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
            //SessionBatches();
            return View(models);
        }

        public ActionResult ActiveUsers(string FirstName, string Email, string PhoneNo, string TrebId)
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
                QStr = "Select Top(50) * From Customer   Order By  IsAvailable desc, LastUpdatedOn desc   ";
            }
            else
            {
                QStr = "Select Top(50) * From Customer   Order By  IsAvailable desc, LastUpdatedOn desc ";
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
                if (dr["CreatedOn"] != DBNull.Value)
                {
                    CustomerEntity.CreatedOn = Convert.ToDateTime((dr["CreatedOn"]));
                }
                CustomerEntity.IsActive = Convert.ToBoolean((dr["IsActive"]));
                CustomerEntity.IsAppLike = Convert.ToBoolean((dr["IsAppLike"]));
                CustomerEntity.IsAvailable = Convert.ToBoolean((dr["IsAvailable"]));
                if (dr["LastUpdatedOn"] != DBNull.Value)
                {

                    TimeSpan ts = DateTime.Now - Convert.ToDateTime(dr["LastUpdatedOn"]);
                    if (ts.Days == 0 )
                    {
                        if (ts.Hours > 0 && CustomerEntity.IsAvailable == false)
                        {
                            CustomerEntity.ActiveTime = Convert.ToString(ts.Hours) + " hr ";
                        }
                        else if (ts.Minutes > 0 && CustomerEntity.IsAvailable == false)
                        {
                            CustomerEntity.ActiveTime = Convert.ToString(ts.Minutes) + " mnt ";
                        }
                        else if ((ts.Minutes > 0 || ts.Minutes==0) && CustomerEntity.IsAvailable == true)
                        {
                            CustomerEntity.ActiveTime = " Online ";
                        }
                        
                    }
                    else if (ts.Days > 0 && ts.Days < 30)
                    {
                        CustomerEntity.IsAvailable = false;
                        CustomerEntity.ActiveTime = Convert.ToString(ts.Days) + " dy ";
                    }

                   
                   
                }
                else
                {
                    CustomerEntity.LastUpdatedOn = DateTime.Now;
                }
               
                if (dr["UpdateStatus"] != DBNull.Value)
                {
                    CustomerEntity.UpdateStatus = Convert.ToBoolean((dr["UpdateStatus"]));
                }
                if (dr["LastUpdatedOn"] != DBNull.Value)
                {

                    TimeSpan ts = DateTime.Now - Convert.ToDateTime(dr["LastUpdatedOn"]);
                    if (ts.Days < 5)
                    {

                        CustomerList.Add(CustomerEntity);
                    }
                }
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
            //SessionBatches();
            return View(models);
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //  db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
