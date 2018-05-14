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
using CommunicationApp.Infrastructure;
using CommunicationApp.Services;
using CommunicationApp.Models;
using AutoMapper;
using System.Data.Entity.Infrastructure;
using System.Configuration;
using CommunicationApp.Core.UtilityManager;
using System.IO;
using CommunicationApp.Core.Infrastructure;
using System.Data.SqlClient;
using CommunicationApp.Controllers;

namespace CommunicationApp.Web.Controllers
{
    public class RegisterController : BaseController
    {

        public ICustomerService _CustomerService { get; set; }
        public ICompanyService _CompanyService { get; set; }
        public IUserRoleService _UserroleService { get; set; }

        public RegisterController(ICustomerService CustomerService, IUserRoleService UserroleService, IUserService _UserService, ICompanyService CompanyService, ICountryService CountryService, IStateService StateService, ICityService CityService, IUserService UserService, IRoleService RoleService, IFormService FormService, IRoleDetailService RoleDetailService)
            : base(CustomerService, UserService, RoleService, FormService, RoleDetailService, UserroleService)
        {
          
            this._CompanyService = CompanyService;
            this._CustomerService = CustomerService;
            this._UserroleService = UserroleService;
         }
        // GET: Register
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Register()
        {
           
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register([Bind(Include = "CustomerId,FirstName,LastName,EmailId,MobileNo,Password")] CustomerModel Customermodel)
        {
            try
            {
                TempData["ShowMessage"] = "error";
                TempData["MessageBody"] = "Please fill the required field with valid data";
                if (ModelState.IsValid)
                {
                    Mapper.CreateMap<CommunicationApp.Models.CustomerModel, CommunicationApp.Entity.Customer>();
                    CommunicationApp.Entity.Customer Customer = Mapper.Map<CommunicationApp.Models.CustomerModel, CommunicationApp.Entity.Customer>(Customermodel);

             
                     Customer Customers = _CustomerService.GetCustomers().Where(c => c.FirstName.Trim() == Customer.FirstName.Trim() || c.EmailId.Trim() == Customer.EmailId.Trim() || c.MobileNo.Trim()==Customer.MobileNo.Trim()).FirstOrDefault();

                    if (Customers == null)
                    {

                        //There is no session in API Controller. So we will find solution in future
                        Customer.CompanyID = 1;
                        //Insert User first
                        string Password = Customermodel.Password;
                        CommunicationApp.Entity.User user = new CommunicationApp.Entity.User();
                        //user.UserId =0; //New Case
                        user.FirstName = Customer.FirstName;
                        user.LastName = Customer.LastName;
                        user.UserName = Customer.MobileNo;// insert mobile number//
                        user.Password = SecurityFunction.EncryptString(Password); //No password right now. We will create send email procedure for password recovery
                        user.UserEmailAddress = Customer.EmailId;
                        user.CompanyID = Customer.CompanyID;
                        user.IsActive = true;
                        _UserService.InsertUser(user);
                        //End : Insert User first

                        if (user.UserId > 0)
                        {
                            //Insert User Role
                            CommunicationApp.Entity.UserRole userRole = new CommunicationApp.Entity.UserRole();
                            userRole.UserId = user.UserId;
                            userRole.RoleId = 3; //By Default set new Customer/user role id=3
                            _UserroleService.InsertUserRole(userRole);
                            //End : Insert User Role

                            //Insert the Customer

                            //Save the Driver photo in Folder
                           

                            //Check SubPath Exist or Not
                            
                           
                            Customer.UserId = user.UserId;
                           
                            Customer.Longitude = 0;
                            Customer.Latitude = 0;
                            Customer.ApplicationId = "";
                            _CustomerService.InsertCustomer(Customer); //Save Operation
                            //End : Insert the Customer
                        }
                        TempData["ShowMessage"] = "success";
                        TempData["MessageBody"] = Customer.FirstName + " is saved successfully.";
                        ModelState.Clear();
                        return RedirectToAction("LogOn", "Account");

                    }
                    else
                    {
                        TempData["ShowMessage"] = "error";
                        if (Customers.FirstName.Trim() == Customermodel.FirstName.Trim())
                        {
                            TempData["MessageBody"] = Customermodel.FirstName + " is already exists.";
                        }
                        else if (Customers.EmailId.Trim() == Customermodel.EmailID.Trim())
                        {
                            TempData["MessageBody"] = Customermodel.EmailID + " is already exists.";
                        }
                        else if (Customers.MobileNo.Trim() == Customermodel.MobileNo.Trim())
                        {
                            TempData["MessageBody"] = Customermodel.MobileNo + " is already exists.";
                        }
                        else
                        {
                            TempData["MessageBody"] = "Please fill the required field with valid data";
                        }

                        //_DriverService.UpdateDriver(driver); //Update Operation
                    }

                }

            }
                 
            catch (RetryLimitExceededException)
            {
                TempData["ShowMessage"] = "error";
                TempData["MessageBody"] = "Some unknown problem occured while proccessing save operation on " + Customermodel.FirstName + " ";

            }

          
            return View(Customermodel);
        }


    }
}