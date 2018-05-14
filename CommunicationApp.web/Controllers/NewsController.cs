using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CommunicationApp.Controllers
{
    public class NewsController : Controller
    {
        //
        // GET: /Services/
        public ActionResult Index()
        {
            ViewBag.HideSideContactUs = "F";
            return View();
        }
        public ActionResult _News()
        {
            ViewBag.HideSideContactUs = "F";
            ViewBag.IsPartial = "F";
            ViewBag.Title = "News";
            return View();
        }
        public ActionResult Vuevent()
        {
            ViewBag.HideSideContactUs = "F";
            ViewBag.Title = "News";
            return View();
        }
	}
}