using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommunicationApp.Models;
using CommunicationApp.Services;
namespace CommunicationApp.Controllers
{
    public class LoginController : Controller
    {

        public IUserService _UserService { get; set; }
        public LoginController(IUserService UserService)
        {
            this._UserService = UserService;
        }
        //
        // GET: /Login/
        public ActionResult Index()
        {

            return View();
        }

        public string LoginMethod(string Email, string Password, string LoggedIn)
        {
            var user = _UserService.GetUsers();
            
            //Get Data from user table
            if (user != null)
            {
                var objUsers = (from m in user where m.UserEmailAddress == Email && m.Password == Password orderby m.UserId descending select m).ToList();

                if (objUsers.Count() > 0) //User Found
                {
                    Session["UserName"] = objUsers.FirstOrDefault().UserName;

                    if (LoggedIn == "on")
                    {
                        Response.Cookies["EmailID"].Value = Email;
                        Response.Cookies["EmailID"].Expires = DateTime.Now.AddDays(30);
                        Response.Cookies["Password"].Value = Password;
                        Response.Cookies["Password"].Expires = DateTime.Now.AddDays(30);
                    }
                    else
                    {
                        Response.Cookies["EmailID"].Value = " ";
                        Response.Cookies["Password"].Value = " ";
                    }
                    //Session["CompanyID"] =2 ;
                    Session["CompanyID"] = objUsers.FirstOrDefault().CompanyID;
                    Session["UserID"] = objUsers.FirstOrDefault().UserId;
                    return "OK";
                }
                else
                {
                    Session["CompanyID"] = null;
                    return "";
                }
            }
            else
            {
                return "";
            }            
        }
    }
}