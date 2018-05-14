using AutoMapper;
using CommunicationApp.Controllers;
using CommunicationApp.Entity;
using CommunicationApp.Infrastructure;
using CommunicationApp.Models;
using CommunicationApp.Services;
using CommunicationApp.Web.Infrastructure.AsyncTask;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace CommunicationApp.Web.Controllers
{
    public class NewsLetterController : BaseController
    {

        public ICompanyService _CompanyService { get; set; }
        public IPropertyService _PropertyService { get; set; }
        public IPropertyImageService _PropertyImageService { get; set; }
        public ICustomerService _CustomerService { get; set; }
        public IAgentService _AgentService { get; set; }
        public IBrokerageServices _BrokerageService { get; set; }
        public IBrokerageServiceServices _BrokerageServiceServices { get; set; }
        public IBrokerageDetailServices _BrokerageDetailServices { get; set; }
        public INewsLetterService _NewsLetterService { get; set; }

        public NewsLetterController(IAgentService AgentService, ICustomerService CustomerService, IUserService UserService, IRoleService RoleService, IFormService FormService, IRoleDetailService RoleDetailService, IRoleService _RoleService, IUserRoleService UserroleService, IPropertyImageService PropertyImageService, IPropertyService PropertyService, ICompanyService CompanyService, IBrokerageServices BrokerageService, IBrokerageServiceServices BrokerageServiceServices, IBrokerageDetailServices BrokerageDetailServices, INewsLetterService NewsLetterService)
            : base(CustomerService, UserService, RoleService, FormService, RoleDetailService, UserroleService)
        {
            this._CompanyService = CompanyService;
            this._PropertyService = PropertyService;
            this._PropertyImageService = PropertyImageService;
            this._CustomerService = CustomerService;
            this._AgentService = AgentService;
            this._BrokerageService = BrokerageService;
            this._BrokerageServiceServices = BrokerageServiceServices;
            this._BrokerageDetailServices = BrokerageDetailServices;
            this._NewsLetterService = NewsLetterService;
        }

        private void CheckPermission()
        {
            RoleDetailModel roleDetail = UserPermission("newsletter");
            TempData["Create"] = roleDetail.IsCreate;
            TempData["Delete"] = roleDetail.IsDelete;
            TempData["Detail"] = roleDetail.IsDetail;
            TempData["Edit"] = roleDetail.IsEdit;
            TempData["View"] = roleDetail.IsView;
        }
        public ActionResult Index()
        {
            NewsletterModel model = new NewsletterModel();
            var Admin = _CustomerService.GetCustomer(Convert.ToInt32(Session["CustomerID"]));
            if (Admin != null)
            {
                model.AdminPhoto = Admin.PhotoPath;
                var AdminLogo = _CompanyService.GetCompany(Admin.CompanyID);
                if (AdminLogo != null)
                {
                    model.AdminLogo = AdminLogo.LogoPath;
                }

                var AdminId = Convert.ToInt32(Session["CustomerID"]) == null ? 0 : Convert.ToInt32(Session["CustomerID"]);
                var CustomerList = _CustomerService.GetCustomers().Where(c => c.ParentId == AdminId);
                ViewBag.Customerlist = CustomerList.Select(x => new SelectListItem { Value = x.CustomerId.ToString(), Text = x.FirstName + " " + x.LastName }).AsEnumerable();

            }
            return View(model);
        }

        [HttpPost]
        public ActionResult SendNewsLetter(NewsletterModel model, HttpPostedFileBase Logofile, HttpPostedFileBase Imgfile)
        {

            if (model.SelectedCustomer == null)
            {
                TempData["ShowMessage"] = "error";
                TempData["MessageBody"] = "Must select users from List.";
                return RedirectToAction("Index");
            }
            if (Logofile != null)
            {

                //Save the photo in Folder
                var fileExt = Path.GetExtension(Logofile.FileName);
                string fileName = Guid.NewGuid() + fileExt;
                var subPath = Server.MapPath("~/NewsLetterImages");

                //Check SubPath Exist or Not
                if (!Directory.Exists(subPath))
                {
                    Directory.CreateDirectory(subPath);
                }
                //End : Check SubPath Exist or Not

                var path = Path.Combine(subPath, fileName);
                Logofile.SaveAs(path);
                var URL = ConfigurationManager.AppSettings["LiveURL"].ToString();
                model.AdminLogo = URL + "/NewsLetterImages/" + fileName;
            }
            else
            {
                model.AdminLogo = model.AdminLogo;
            }
            if (Imgfile != null)
            {

                //Save the photo in Folder
                var fileExt = Path.GetExtension(Imgfile.FileName);
                string fileName = Guid.NewGuid() + fileExt;
                var subPath = Server.MapPath("~/NewsLetterImages");

                //Check SubPath Exist or Not
                if (!Directory.Exists(subPath))
                {
                    Directory.CreateDirectory(subPath);
                }
                //End : Check SubPath Exist or Not

                var path = Path.Combine(subPath, fileName);
                Imgfile.SaveAs(path);
                var URL = ConfigurationManager.AppSettings["LiveURL"].ToString();
                model.PropertyPhoto = URL + "/NewsLetterImages/" + fileName;
            }
            else
            {
                model.PropertyPhoto = "http://communicationapp.only4agents.com/NewsLetterImages/082886c2-edc3-41c8-9f5d-5c68c188bef2.jpg";
            }

            var AdminId = Convert.ToInt32(Session["CustomerID"]) == null ? 0 : Convert.ToInt32(Session["CustomerID"]);

            var Customerids = "";

            if (model.SelectedCustomer != null)
            {
                Customerids = string.Join(",", model.SelectedCustomer);
            }
            else
            {
                var CustomerIdList = _CustomerService.GetCustomers().Select(c => c.CustomerId);
                Customerids = string.Join(",", CustomerIdList);
            }


            JobScheduler.SendNewsLetter(AdminId.ToString(), Customerids, model);

            TempData["ShowMessage"] = "success";
            TempData["MessageBody"] = "Mail send successfully.";
            return RedirectToAction("Index");
        }


        public ActionResult NewsletterList(string NewsLetterName)
        {

             var AdminId = Convert.ToInt32(Session["CustomerID"]) == null ? 0 : Convert.ToInt32(Session["CustomerID"]);
            var CustomerList = _CustomerService.GetCustomers().Where(c => c.ParentId == AdminId);
            List<NewsLetter_Model> ModelList = new List<NewsLetter_Model>();
            var NewsLetters = _NewsLetterService.GetNewsLetters().OrderBy(c => c.OrderNo);
            if (!string.IsNullOrEmpty(NewsLetterName))
            {
                NewsLetters = NewsLetters.Where(c => c.NewsLetterName.ToLower().Contains(NewsLetterName.ToLower())).OrderBy(c => c.OrderNo);
            }
            Mapper.CreateMap<CommunicationApp.Entity.NewsLetter_Entity, CommunicationApp.Models.NewsLetter_Model>();
            foreach (var NewsLetter in NewsLetters)
            {
                CommunicationApp.Models.NewsLetter_Model NewLetterModel = Mapper.Map<CommunicationApp.Entity.NewsLetter_Entity, CommunicationApp.Models.NewsLetter_Model>(NewsLetter);
                if (NewsLetter.Image == null || NewsLetter.Image == "")
                {
                    NewLetterModel.Image = CommonCls.GetURL() + "/images/noImage.jpg";
                }
                else if (NewsLetter.Image.IndexOf(',') > -1)
                {
                    var splitimgs = NewsLetter.Image.Split(',');
                    NewLetterModel.first_img = splitimgs[0];
                    NewLetterModel.second_img = splitimgs[1];
                }
                else
                {

                }

                if (NewLetterModel.SelectedUsers != "" && NewLetterModel.SelectedUsers!=null)
                {
                    List<string> selectedusers = new List<string>();
                    var UsersIds = NewLetterModel.SelectedUsers.Split(',').Select(int.Parse).ToList();
                    var Users = CustomerList.Where(c => UsersIds.Contains(c.CustomerId));
                    foreach(var usr in Users)
                    {
                        selectedusers.Add(usr.FirstName +" "+usr.LastName);
                    }
                    NewLetterModel.Select_users = selectedusers;
                }
                

                ModelList.Add(NewLetterModel);
            }
            ViewBag.OrderList = OrderNoList();

           
            ViewBag.Customerlist = CustomerList.Select(x => new SelectListItem { Value = x.CustomerId.ToString(), Text = x.FirstName + " " + x.LastName}).AsEnumerable();
            return View(ModelList);
        }

        public ActionResult Create()
        {
            NewsLetter_Model Model = new NewsLetter_Model();
            var CustomerList = _CustomerService.GetCustomers().Where(c => c.ParentId == null);
            Model.CustomersList = CustomerList.Select(x => new SelectListItem { Value = x.UserId.ToString(), Text = x.FirstName }).ToList();
            Model.AdminId = Convert.ToInt32(Session["CustomerId"]);
            //var OrderNos = _NewsLetterService.GetNewsLetters().Select(c => c.OrderNo).ToList();
            //var OrderList=OrderNoList().Where(c =>!OrderNos.Contains(Convert.ToInt32(c.Value)));

            ViewBag.OrderList = OrderNoList();


            return View(Model);
        }



        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(NewsLetter_Model Model, HttpPostedFileBase file, HttpPostedFileBase SecondFile)
        {
            UserPermissionAction("newsletter", RoleAction.view.ToString());
            CheckPermission();
            TempData["ShowMessage"] = "";
            TempData["MessageBody"] = "";
            try
            {
                if (ModelState.IsValid)
                {


                    if (file == null)
                    {
                        TempData["ShowMessage"] = "error";
                        TempData["MessageBody"] = "Please choose newsletter image.";
                        return View("Create", Model);
                    }


                    Mapper.CreateMap<CommunicationApp.Models.NewsLetter_Model, CommunicationApp.Entity.NewsLetter_Entity>();
                    CommunicationApp.Entity.NewsLetter_Entity NewsLetter = Mapper.Map<CommunicationApp.Models.NewsLetter_Model, CommunicationApp.Entity.NewsLetter_Entity>(Model);
                    string NewsletterImage = "";
                    if (file != null)
                    {

                        if (NewsLetter.Image != "")
                        {   //Delete Old Image
                            string pathDel = Server.MapPath("~/NewsLetterImages");

                            FileInfo objfile = new FileInfo(pathDel);
                            if (objfile.Exists) //check file exsit or not
                            {
                                objfile.Delete();
                            }
                            //End :Delete Old Image
                        }

                        NewsletterImage = SaveImage(file);
                    }

                    if (SecondFile != null)
                    {

                        if (NewsLetter.Image != "")
                        {   //Delete Old Image
                            string pathDel = Server.MapPath("~/NewsLetterImages");

                            FileInfo objfile = new FileInfo(pathDel);
                            if (objfile.Exists) //check file exsit or not
                            {
                                objfile.Delete();
                            }
                            //End :Delete Old Image
                        }

                        NewsletterImage += ',' + SaveImage(SecondFile);
                    }

                    NewsLetter.AdminId = Convert.ToInt32(Session["CustomerId"]);//CutomerId                    
                    NewsLetter.Image = NewsletterImage;
                    _NewsLetterService.InsertNewsLetter(NewsLetter);



                    TempData["ShowMessage"] = "success";
                    TempData["MessageBody"] = "NewsLetter successfully saved.";
                    return RedirectToAction("NewsletterList");
                }
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Key, x.Value.Errors }).ToArray();
                var modelStateErrors = this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors);
                var CustomerList1 = _CustomerService.GetCustomers();
                Model.CustomersList = CustomerList1.Select(x => new SelectListItem { Value = x.UserId.ToString(), Text = x.FirstName }).ToList();
                Model.AdminId = 1;
                return View(Model);
            }
            catch (Exception ex)
            {
                ErrorLogging.LogError(ex);
                Model.CustomersList = _CustomerService.GetCustomers().Select(x => new SelectListItem { Value = x.UserId.ToString(), Text = x.FirstName }).ToList();
                return View(Model);
            }

        }


        [HttpPost]
        public JsonResult UpdateNewsLetter(string NewsLetterId, string DropdownId)
        {
            if (NewsLetterId != null && DropdownId != null)
            {

                var NewsLetters = _NewsLetterService.GetNewsLetters();

                //change the old orderno.
                var Old_OrderNo = NewsLetters.Where(c => c.OrderNo == Convert.ToInt32(DropdownId)).FirstOrDefault();
                if (Old_OrderNo != null)
                {
                    var newsletter = NewsLetters.Where(c => c.NewsLetterId == Convert.ToInt32(NewsLetterId)).FirstOrDefault();
                    Old_OrderNo.OrderNo = newsletter.OrderNo;
                    _NewsLetterService.UpdateNewsLetter(Old_OrderNo);
                }


                foreach (var newsletter in NewsLetters)
                {
                    //set the new order
                    if (newsletter.NewsLetterId == Convert.ToInt32(NewsLetterId))
                    {
                        newsletter.OrderNo = Convert.ToInt32(DropdownId);
                        _NewsLetterService.UpdateNewsLetter(newsletter);
                    }


                }

            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult UpdateSelectedUsers(string NewsLetterId, List<int> SelectedUsers)
        {
            if (NewsLetterId != null)
            {
                var NewsLetters = _NewsLetterService.GetNewsLetter(Convert.ToInt32(NewsLetterId));
                if (NewsLetters != null)
                {
                    if (SelectedUsers != null)
                    {

                        NewsLetters.SelectedUsers = string.Join(",", SelectedUsers.Select(n => n.ToString()).ToArray());
                        _NewsLetterService.UpdateNewsLetter(NewsLetters);
                    }
                    else
                    {
                        NewsLetters.SelectedUsers = null;
                        _NewsLetterService.UpdateNewsLetter(NewsLetters);
                    }
                }
                


            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateNewsLetterDate(string NewsLetterId, string UpdatedDate)
        {
            if (NewsLetterId != null && UpdatedDate != null)
            {

                var NewsLetters = _NewsLetterService.GetNewsLetter(Convert.ToInt32(NewsLetterId));
                if (NewsLetters != null)
                {
                    NewsLetters.fwd_date = Convert.ToDateTime(UpdatedDate);
                    _NewsLetterService.UpdateNewsLetter(NewsLetters);
                }


            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }


        public List<SelectListItem> OrderNoList()
        {
            List<SelectListItem> OrderNoList = new List<SelectListItem>();
            OrderNoList.Add(new SelectListItem() { Text = "Order", Value = "Order", Selected = false });
            OrderNoList.Add(new SelectListItem() { Text = "1", Value = "1", Selected = false });
            OrderNoList.Add(new SelectListItem() { Text = "2", Value = "2", Selected = false });
            OrderNoList.Add(new SelectListItem() { Text = "3", Value = "3", Selected = false });
            OrderNoList.Add(new SelectListItem() { Text = "4", Value = "4", Selected = false });
            OrderNoList.Add(new SelectListItem() { Text = "5", Value = "5", Selected = false });
            OrderNoList.Add(new SelectListItem() { Text = "6", Value = "6", Selected = false });
            OrderNoList.Add(new SelectListItem() { Text = "7", Value = "7", Selected = false });
            OrderNoList.Add(new SelectListItem() { Text = "8", Value = "8", Selected = false });
            OrderNoList.Add(new SelectListItem() { Text = "9", Value = "9", Selected = false });
            OrderNoList.Add(new SelectListItem() { Text = "10", Value = "10", Selected = false });
            OrderNoList.Add(new SelectListItem() { Text = "11", Value = "11", Selected = false });
            OrderNoList.Add(new SelectListItem() { Text = "12", Value = "12", Selected = false });
            OrderNoList.Add(new SelectListItem() { Text = "13", Value = "13", Selected = false });
            OrderNoList.Add(new SelectListItem() { Text = "14", Value = "14", Selected = false });
            OrderNoList.Add(new SelectListItem() { Text = "15", Value = "15", Selected = false });
            OrderNoList.Add(new SelectListItem() { Text = "16", Value = "16", Selected = false });
            OrderNoList.Add(new SelectListItem() { Text = "17", Value = "17", Selected = false });
            OrderNoList.Add(new SelectListItem() { Text = "18", Value = "18", Selected = false });
            OrderNoList.Add(new SelectListItem() { Text = "19", Value = "19", Selected = false });
            OrderNoList.Add(new SelectListItem() { Text = "20", Value = "20", Selected = false });

            return OrderNoList;
        }

      


        public ActionResult DeleteNewsLetter(int id)
        {

            if (id != null && id != 0)
            {
                var Newsletter = _NewsLetterService.GetNewsLetter(id);
                if (Newsletter != null)
                {
                    _NewsLetterService.DeleteNewsLetter(Newsletter);
                }

            }
            return RedirectToAction("NewsletterList");
        }


        public string SaveImage(HttpPostedFileBase file)
        {
            var Url = "";

            //Save the photo in Folder
            var fileExt = Path.GetExtension(file.FileName);
            string fileName = Guid.NewGuid() + fileExt;
            var subPath = Server.MapPath("~/NewsLetterImages");

            //Check SubPath Exist or Not
            if (!Directory.Exists(subPath))
            {
                Directory.CreateDirectory(subPath);
            }
            //End : Check SubPath Exist or Not

            var path = Path.Combine(subPath, fileName);
            file.SaveAs(path);
            Url = CommonCls.GetURL() + "/NewsLetterImages/" + fileName;
            return Url;
        }
    }
}