using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EveryWhereCars.Controllers
{
    public class AuthenticationServiceController : Controller
    {
        //
        // GET: /AuthenticationService/
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Success(string ShowMessage, string MessageBody)
        {
            TempData["ShowMessage"] = ShowMessage;
            TempData["MessageBody"] = MessageBody;
            return View();
        }
        public ActionResult Error(string ShowMessage, string MessageBody)
        {
            TempData["ShowMessage"] = ShowMessage;
            TempData["MessageBody"] = MessageBody;
            return View();
        }
	}
}