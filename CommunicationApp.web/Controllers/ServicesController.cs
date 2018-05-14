using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Wherezat.Controllers
{
    public class ServicesController : Controller
    {
        //
        // GET: /Services/
        public ActionResult Index()
        {
            ViewBag.HideSideContactUs = "F";
            return View();
        }
        public ActionResult _Services()
        {
            ViewBag.HideSideContactUs = "F";
            ViewBag.IsPartial = "F";
            ViewBag.Title = "Our Services";
            return View();
        }
        public ActionResult WebDevelopment()
        {
            ViewBag.HideSideContactUs = "F";
            ViewBag.Title = "Web Design and Development Solutions";
            return View();
        }
        public ActionResult ApplicationDevelopment()
        {
            ViewBag.HideSideContactUs = "F";
            ViewBag.Title = "iPhone and Android Application development";
            return View();
        }
        public ActionResult SoftwareDevelopment()
        {
            ViewBag.HideSideContactUs = "F";
            ViewBag.Title = "Web Based Software Development";
            return View();
        }
        public ActionResult BrandDevelopment()
        {
            ViewBag.HideSideContactUs = "F";
            ViewBag.Title = "Brand, Logo and Identity Development";
            return View();
        }
        public ActionResult BusinessIntelligence()
        {
            ViewBag.HideSideContactUs = "F";
            ViewBag.Title = "Business Intelligence";
            return View();
        }
        public ActionResult SEO()
        {
            ViewBag.HideSideContactUs = "F";
            ViewBag.Title = "Search Engine Optimization";
            return View();
        }
	}
}