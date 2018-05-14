using System;
using System.Globalization;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Net;
using System.Net.Mail;
using System.Web.Configuration;
using System.Linq;
using CommunicationApp.Models;
using CommunicationApp.Services;
using CommunicationApp.Core.Infrastructure;
using AutoMapper;
using CommunicationApp.Infrastructure;
namespace CommunicationApp.Controllers
{
    public class AccountController : BaseController
    {
        public AccountController(ICustomerService CustomerService, IUserService UserService, IRoleService RoleService, IFormService FormService, IRoleDetailService RoleDetailService, IUserRoleService UserRoleService)
            : base(CustomerService, UserService, RoleService, FormService, RoleDetailService, UserRoleService)
        {            
        }
       
        //
        // GET: /Account/LogOn
        
        public ActionResult LogOn()
        {
            DateTime date = DateTime.Now.AddDays(-1);
           string gjh = String.Format("{0:M/d/yyyy}", date);
            //string appid = "eqfAxBSZeCo:APA91bGyWabm11I5pA0OOM-3d6wzTSNBMoJtXvnnnUgkO9f-VmH5Ic5URd3HFgqDMBfgQe__Lyc666D3Ibs_bAe2K_vKZsZTfzIBOg7kApQKdgRhSsqzqz6MqVjBegMH-2XyERlhTK3y";
            //string UserMessage = "hello";
            //string Message = "6";

            //CommonCls.TestSendFCM_Notifications(appid, Message, UserMessage);
            //string ApplicationId = "";
            //string JsonMessage = "{\"Flag\":\"" + "6" + "\",\"Message\":\"" + "hello" + "\"}";

            //CommonCls.SendGCM_Notifications(ApplicationId, JsonMessage, true);
            var pas = "28eRvLRXsnI=";
            var password = SecurityFunction.DecryptString(pas);
          

            return View();
        }

        // 
        // POST: /Account/LogOn

        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = _UserService.ValidateUser(model.UserName, model.Password);
                //UserModel user = db.ValidateUser(model.UserName, model.Password);
                if (user != null)
                {
                    SetupFormsAuthTicket(model.UserName, model.RememberMe);

                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                   
                   
                }
                ModelState.AddModelError("", "The user name or password provided is incorrect.");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        public JsonResult LoginMethod(string Email, string Password, string LoggedIn)
        {
            //var ghgh = SecurityFunction.DecryptString("kgNiGwMSC7c=");
            SuccessModel SuccessModel = new Models.SuccessModel();
            string UserName = "";
            var user = _UserService.GetUserByEmailId(Email);
            if (user != null) //By Email Id
            {
                UserName = user.UserName; //Get User Name
                
            }
            else //By User Name
            {
                UserName = Email; //Here Email is UserName, Set User Name
                user = _UserService.GetUserByName(UserName);
                if (user != null)  //Get Email Id
                {
                    //EmailId = user.UserEmailAddress; //Get Email Id
                    //CompanyID = user.CompanyID; //Get Company ID
                }
            }

            var objuser = _UserService.ValidateUser(UserName, Password);
            if (objuser != null)
            {
                SetupFormsAuthTicket(UserName, false);
                //Set Session Variables in Base Controller
                SetSessionVariables(UserName);
                if (objuser.UserName == "SuperAdmin")
                {
                    SuccessModel.Status = "SuperAdmin";
                    return Json(SuccessModel);
                }
                else
                {
                    var Customer = _CustomerService.GetCustomers().Where(c => c.UserId == objuser.UserId).FirstOrDefault();
                    if (Customer!=null)
                    {
                        SuccessModel.Status = "Admin";
                        SuccessModel.CustomerId = Customer.CustomerId;

                        Session["UserId"] = Customer.CustomerId;
                    
                        return Json(SuccessModel);
                    }
                    else
                    {
                        return Json(SuccessModel);
                    }
                    
                }
               
                
            }
            else
            {
                SuccessModel.Status = "failed";
                return Json(SuccessModel);
            }
        }
        //
        // GET: /Account/SetCompany
        public string SetCompany(string CompanyID,string CompanyName,string LogoPath)
        {
            try
            {
                Session["CompanyID"] = CompanyID; //Set Company Id
                Session["CompanyName"] = CompanyName;//Set Company Name
                Session["LogoPath"] = LogoPath; //Set Logo
                return "success";
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.Message.ToString();
                ErrorLogging.LogError(ex);
                return "failed";
            }
        }

        public ActionResult LogOff()
        {
            System.Web.HttpContext.Current.Response.Cookies.Clear();

            Session["UserPermission"] = null;
            Session["UserId"] = null;
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();            
            FormsAuthentication.SignOut();
            return RedirectToAction("LogOn", "Account");
        }

        //
        // GET: /Account/Register

        public ActionResult Register()
        {
            return View();
        }
        public ActionResult ForgotPassword()
        {
            return View();
        }
        public string ForgotPasswordMethod(string Email)
        {
            string UserName = "";
            var user = _UserService.GetUserByEmailId(Email);
            if (user != null) //By Email Id
            {
                UserName = user.UserName; //Get User Name
            }
            else //By User Name
            {
                UserName = Email; //Here Email is UserName, Set User Name
                user = _UserService.GetUserByName(UserName);
            }
            if (user != null)
            {
                //Send Email to User
                string Password = SecurityFunction.DecryptString(user.Password);
                SendMailToUser(UserName, Email, Password);

                TempData["ShowMessage"] = "success";
                TempData["MessageBody"] = "Password send to your email. Please check your email";
                return "success";
            }
            else
            {
                TempData["ShowMessage"] = "error";
                TempData["MessageBody"] = "User doesnot exist or Incorrect email id.";
                return "failed";
            }
        }
        public void SendMailToUser(string UserName, string Email, string Password)
        {
            try
            {
                // Send mail.
                MailMessage mail = new MailMessage();

                string FromEmailID = WebConfigurationManager.AppSettings["FromEmailID"];
                string FromEmailPassword = WebConfigurationManager.AppSettings["FromEmailPassword"];

                string ToBCCEmailID = WebConfigurationManager.AppSettings["ToBCCEmailID"];
                //Password = AmebaSoftwares.Infrastructure.CustomMembershipProvider.DecryptString(user.Password);
                string _Host = WebConfigurationManager.AppSettings["Host"];
                int _Port = Convert.ToInt32(WebConfigurationManager.AppSettings["Port"].ToString());
                Boolean _UseDefaultCredentials = Convert.ToBoolean(WebConfigurationManager.AppSettings["UseDefaultCredentials"].ToString());
                Boolean _EnableSsl = Convert.ToBoolean(WebConfigurationManager.AppSettings["EnableSsl"].ToString());
                mail.To.Add(Email);
                // mail.Bcc.Add(ToBCCEmailID);
                mail.From = new MailAddress(FromEmailID);
                mail.Subject = "Forgot Password Ameba";
                string msgbody = "";
                msgbody = msgbody + "<br />";
                msgbody = msgbody + "<table style='width:50%'>";
                msgbody = msgbody + "<tr>";

                msgbody = msgbody + "<td align='left' style=' font-family:Arial; font-weight:bold; font-size:15px;'>Please Find Blow Your Password<br /></td></tr>";
                msgbody = msgbody + "<tr><td align='left'>";
                msgbody = msgbody + "<br /><font style=' font-family:Arial; font-size:13px;'><b>Email Address: </b>" + Email + "</font><br /><br />";
                msgbody = msgbody + "<font style=' font-family:Arial; font-size:13px;'><b>Password: </b>" + Password + "</font><br /><br />";
                msgbody = msgbody + "<br />";
                mail.Body = msgbody;
                mail.IsBodyHtml = true;
                
                SmtpClient smtp = new SmtpClient();
                smtp.Host = _Host;
                smtp.Port = _Port;
                smtp.UseDefaultCredentials = _UseDefaultCredentials;
                smtp.Credentials = new System.Net.NetworkCredential
                (FromEmailID, FromEmailPassword);// Enter senders User name and password
                smtp.EnableSsl = _EnableSsl;
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
        }
        //
        // GET: /Account/ChangePassword

        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        // ChangePassword method not implemented in CustomMembershipProvider.cs
        // Feel free to update!

        //
        // POST: /Account/ChangePassword
        public string ChangePasswordMethod(string OldPassword, string NewPassword, string ConfirmPassword)
        {
            if (OldPassword == "") //Old Password Blank
            {
                TempData["ShowMessage"] = "error";
                TempData["MessageBody"] = "Please enter a valid old password.";
                return "Please enter a valid old password.";
            }
            else if ((NewPassword=="") || (ConfirmPassword==""))
            {
                TempData["ShowMessage"] = "error";
                TempData["MessageBody"] = "Please enter a valid new/confirm password.";
                return "Please enter a valid new/confirm password.";
            }
            else if (NewPassword != ConfirmPassword)
            {
                TempData["ShowMessage"] = "error";
                TempData["MessageBody"] = "New Password and confirm password doesnot matching.";
                return "New Password and confirm password doesnot matching.";
            }

            try
            {
                int UserId=Convert.ToInt32(Session["UserId"].ToString());

                var user = _UserService.GetUserById(UserId);
                if (user != null)
                {
                    if (SecurityFunction.DecryptString(user.Password) == OldPassword)
                    {
                        //Update the User Password
                        user.Password = SecurityFunction.EncryptString(NewPassword);
                        _UserService.UpdateUser(user);
                        //End : Update the User Password
                        TempData["ShowMessage"] = "success";
                        TempData["MessageBody"] = "Password changed successfully.";
                        return "success";
                    }
                    else
                    {
                        TempData["ShowMessage"] = "error";
                        TempData["MessageBody"] = "Please enter a valid old password.";
                        return "Please enter a valid old password.";
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.Message.ToString();
                ErrorLogging.LogError(ex);
            }

            TempData["ShowMessage"] = "error";
            TempData["MessageBody"] = "Unknown Error Occurred while processing request.";
            return "Unknown Error Occurred while processing request.";
        }

        //
        // GET: /Account/ChangePasswordSuccess

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        private UserModel SetupFormsAuthTicket(string userName, bool persistanceFlag)
        {
           

            var user = _UserService.GetUserByName(userName);
            Mapper.CreateMap<CommunicationApp.Entity.User, CommunicationApp.Models.UserModel>();
            CommunicationApp.Models.UserModel userModel = Mapper.Map<CommunicationApp.Entity.User, CommunicationApp.Models.UserModel>(user);

            var userId = userModel.UserId;
            var userData = userId.ToString(CultureInfo.InvariantCulture);
            var authTicket = new FormsAuthenticationTicket(1, //version
                                                        userName, // user name
                                                        DateTime.Now,             //creation
                                                        DateTime.Now.AddMinutes(30), //Expiration
                                                        persistanceFlag, //Persistent
                                                        userData);

            var encTicket = FormsAuthentication.Encrypt(authTicket);
            Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));
            return userModel;
        }

        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
