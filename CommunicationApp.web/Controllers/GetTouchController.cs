using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using System.Web.Configuration;
using CommunicationApp.Models;
namespace CommunicationApp.Controllers
{
    public class GetTouchController : Controller
    {

        // GET: /GetTouch/
        public ActionResult Index()
        {
            ViewBag.IsPartial = "F";
            ViewBag.HideSideContactUs = "T";
            ViewBag.ModelPopup = "";
            TempData["EmailModel"] = null;
            return View();
        }
        public ActionResult _GetTouch(string IsPartial)
        {
            
            if ((IsPartial != null) && (IsPartial != ""))
            {
                ViewBag.IsPartial = "Y";
                ViewBag.ModelPopup = "Y";
                Session["ModelPopup"] = "Y";
            }
            else
            {
                ViewBag.IsPartial = "F";
                ViewBag.ModelPopup = "";
                Session["ModelPopup"] = "";
            }
           
            ViewBag.Title = "Contact Us";
            EmailModel model = (EmailModel)TempData["EmailModel"];
            return View(model);
        }

        [HttpPost]
        public ActionResult SendMail(EmailModel model)
        {
            ViewBag.Title = "Contact Us";
            EmailModel viewModel = new EmailModel();
            if (!ModelState.IsValid)
            {
                viewModel.FullName = model.FullName;
                viewModel.EmailID = model.EmailID;
                viewModel.Subject = model.Subject;
                viewModel.Message = model.Message;

                TempData["EmailModel"] = viewModel;
                return RedirectToAction("_GetTouch", viewModel);
            }
            
            // Send mail.
            MailMessage mail = new MailMessage();

            string ToEmailID = WebConfigurationManager.AppSettings["ToEmailID"];
            string FromEmailID = WebConfigurationManager.AppSettings["FromEmailID"];
            string FromEmailPassword = WebConfigurationManager.AppSettings["FromEmailPassword"];

            string _Host = WebConfigurationManager.AppSettings["Host"];
            int _Port =Convert.ToInt32(WebConfigurationManager.AppSettings["Port"].ToString());
            Boolean _UseDefaultCredentials =Convert.ToBoolean(WebConfigurationManager.AppSettings["UseDefaultCredentials"].ToString());
            Boolean _EnableSsl = Convert.ToBoolean(WebConfigurationManager.AppSettings["EnableSsl"].ToString());

            mail.To.Add(ToEmailID);
            mail.From = new MailAddress(FromEmailID);
            mail.Subject = model.Subject;
            string body = "";
            body = "<p>Person Name : " + model.FullName + "</p>";
            body = body + "<p>Email ID : " + model.EmailID + "</p>";
            body = body + "<p>Subject : " + model.Subject + "</p>";
            body = body + "<p>" + model.Message + "</p>";
            mail.Body = body;

            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = _Host;
            smtp.Port = _Port;
            smtp.UseDefaultCredentials = _UseDefaultCredentials;
            smtp.Credentials = new System.Net.NetworkCredential
            (FromEmailID, FromEmailPassword);// Enter senders User name and password
            smtp.EnableSsl = _EnableSsl;
            smtp.Send(mail);

            viewModel.ShowMessage = "Y";
            TempData["EmailModel"] = viewModel;
            if (Session["ModelPopup"].ToString().Equals('Y'))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("_GetTouch", viewModel);
            }
            
        }
	}
}