using AutoMapper;
using CommunicationApp.Controllers;
using CommunicationApp.Infrastructure;
using CommunicationApp.Models;
using CommunicationApp.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CommunicationApp.Web.Controllers
{
    public class TipController : BaseController
    {
        public ICustomerService _CustomerService { get; set; }
        public ITipService _TipService { get; set; }

        public TipController(ICustomerService CustomerService, IRoleService RoleService, IFormService FormService, IRoleDetailService RoleDetailService, IRoleService _RoleService, IUserRoleService UserRoleService,  IUserService UserService, ITipService TipService)
            : base(CustomerService, UserService, RoleService, FormService, RoleDetailService, UserRoleService)
        {
            this._UserService = UserService;
            this._UserRoleService = UserRoleService;
            this._CustomerService = CustomerService;
            this._TipService = TipService;
        }

        // GET: /Tip/
        public ActionResult Index(string StartDate, string EndDate)
        {
            var models = new List<TipModel>();
            if (!string.IsNullOrEmpty(StartDate))
            {
                if (CheckDate(StartDate) == false)
                {
                    TempData["ShowMessage"] = "error";
                    TempData["MessageBody"] = "Please select a valid From Date.";
                    return View(models);
                }
            }
            if (!string.IsNullOrEmpty(EndDate))
            {
                if (CheckDate(EndDate) == false)
                {
                    TempData["ShowMessage"] = "error";
                    TempData["MessageBody"] = "Please select a valid To Date.";
                    return View(models);
                }
            }

            if (!string.IsNullOrEmpty(StartDate) && !string.IsNullOrEmpty(EndDate))
            {
                if (Convert.ToDateTime(Convert.ToDateTime(StartDate).ToString("MM/dd/yyyy")) > Convert.ToDateTime(Convert.ToDateTime(EndDate).ToString("MM/dd/yyyy")))
                {
                    TempData["ShowMessage"] = "error";
                    TempData["MessageBody"] = "Please select a valid date range.";

                    return View(models);
                }

            }
            UserPermissionAction("tip", RoleAction.create.ToString());
            CheckPermission();

            var TipList = _TipService.GetTips().OrderByDescending(c => c.CreatedOn).ToList();
            //For Search By date
            if (!string.IsNullOrEmpty(StartDate) && !string.IsNullOrEmpty(EndDate) && StartDate.Trim() != "" && EndDate.Trim() != "")
            {
                CultureInfo culture = new CultureInfo("en-US");
                DateTime startDate = Convert.ToDateTime(Convert.ToDateTime(StartDate).ToString("MM/dd/yyyy"), culture);
                DateTime Enddate = Convert.ToDateTime(Convert.ToDateTime(EndDate).ToString("MM/dd/yyyy"), culture);
                TipList = TipList.Where(c => Convert.ToDateTime(Convert.ToDateTime(c.CreatedOn).ToString("MM/dd/yyyy"), culture) >= startDate && Convert.ToDateTime(Convert.ToDateTime(c.CreatedOn).ToString("MM/dd/yyyy"), culture) <= Enddate).ToList();
            }
            Mapper.CreateMap<CommunicationApp.Entity.Tip, CommunicationApp.Models.TipModel>();
            foreach (var Tip in TipList)
            {
                CommunicationApp.Models.TipModel TipModel = Mapper.Map<CommunicationApp.Entity.Tip, CommunicationApp.Models.TipModel>(Tip);
                models.Add(TipModel);
            }
            return View(models);
        }
        //Pemission Method
        private void CheckPermission()
        {
            RoleDetailModel roleDetail = UserPermission("tip");
            TempData["View"] = roleDetail.IsView;
            TempData["Create"] = roleDetail.IsCreate;
            TempData["Edit"] = roleDetail.IsEdit;
            TempData["Delete"] = roleDetail.IsDelete;
            TempData["Detail"] = roleDetail.IsDetail;
        }
        //Create 
        [HttpGet]
        public ActionResult Create()
        {
            TipModel TipModel = new CommunicationApp.Models.TipModel();
            //Check User Permission
            UserPermissionAction("tip", RoleAction.create.ToString());
            CheckPermission();
            return View(TipModel);
        }

        [HttpPost]
        public ActionResult Create([Bind(Include = "TipId,CustomerId,TipUrl,Title,CreatedOn,LastUpdatedOn,IsActive")]TipModel tipModel, HttpPostedFileBase file)
        {
            UserPermissionAction("Tip", RoleAction.create.ToString());
            CheckPermission();
            try
            {
             
                //TempData["ShowMessage"] = "error";
                //TempData["MessageBody"] = "Please fill the required field with valid data.";
                if(file!=null)
                {
                    var fileExt = Path.GetExtension(file.FileName);
                    if (fileExt != ".pdf")
                    {
                        TempData["ShowMessage"] = "error";
                        TempData["MessageBody"] = "Please upload only .pdf file.";
                        return RedirectToAction("Create");
                    }

                    //if (tipModel.Title == "" || tipModel.Title == null )
                    //{
                    //    TempData["ShowMessage"] = "error";
                    //    TempData["MessageBody"] = "Please fill the title field.";
                    //    return RedirectToAction("Create");
                    //}
               
               
                if (ModelState.IsValid)
                {

                    Mapper.CreateMap<CommunicationApp.Models.TipModel, CommunicationApp.Entity.Tip>();
                    CommunicationApp.Entity.Tip tipEntity = Mapper.Map<CommunicationApp.Models.TipModel, CommunicationApp.Entity.Tip>(tipModel);

                    //Save the Logo in Folder
                   
                    string fileName = Guid.NewGuid() + fileExt;

                    var subPath = Server.MapPath("~/TipData");
                    //Check SubPath Exist or Not
                    if (!Directory.Exists(subPath))
                    {
                        Directory.CreateDirectory(subPath);
                    }
                    //End : Check SubPath Exist or Not

                    var path = Path.Combine(subPath, fileName);
                    file.SaveAs(path);

                    string URL = CommonCls.GetURL() + "/TipData/" + fileName;
                    tipEntity.CustomerId = Convert.ToInt32(Session["CustomerId"]);
                    tipEntity.TipUrl = URL;
                    tipEntity.IsActive = true;
                    tipEntity.CreatedOn = DateTime.Now;
                    _TipService.InsertTip(tipEntity);

                    var Flag = 13;//status for Tip of the day.
                    var Message = "New Tip of the day.";
                    ////send notification
                    //var CustomerList = _CustomerService.GetCustomers().Where(c => c.CustomerId != tipModel.CustomerId && c.IsActive == true).ToList();
                    //foreach (var Customer in CustomerList)
                    //{
                    //    if (Customer != null)
                    //    {
                    //        if (Customer.ApplicationId != null && Customer.ApplicationId != "")
                    //        {
                    //            string PropertyDetail = "{\"Flag\":\"" + Flag + "\",\"Message\":\"" + Message + "\"}";
                    //            Common.SendGCM_Notifications(Customer.ApplicationId, PropertyDetail, true);
                    //        }

                    //    }
                    //}
                    TempData["ShowMessage"] = "success";
                    TempData["MessageBody"] = "Tip is saved successfully.";
                    return RedirectToAction("Index");
                }
                }
                else
                {
                    TempData["ShowMessage"] = "error";
                    TempData["MessageBody"] = "Please upload a valid file.";
                    return RedirectToAction("Create");
                }
            }


            catch (Exception ex)
            {
                ErrorLogging.LogError(ex);

                TempData["ShowMessage"] = "error";
                TempData["MessageBody"] = "Some unknown problem occured while proccessing save operation on Tip.";

            }
          //  return RedirectToAction("Create", "Tip", new { id = tipModel.TipId });
            return View(tipModel);
        }

        public ActionResult Delete(int? id)
        {
            UserPermissionAction("Tip", RoleAction.view.ToString());
            CheckPermission();
            TempData["ShowMessage"] = "";
            TempData["MessageBody"] = "";
            var Id = Convert.ToInt32(id);
            try
            {
                var Tip = _TipService.GetTip(Id);
                if (Tip != null)
                {

                    _TipService.DeleteTip(Tip);
                    TempData["ShowMessage"] = "success";
                    TempData["MessageBody"] = "Tip is delete successfully.";
                    return RedirectToAction("index");
                }
            }
            catch (Exception ex)
            {
                ErrorLogging.LogError(ex);
                TempData["MessageBody"] = "No record is deleted.";
                return RedirectToAction("index");
            }



            return RedirectToAction("index");
        }

        protected bool CheckDate(String date)
        {
            try
            {
                DateTime dt = DateTime.Parse(date);
                return true;
            }
            catch
            {
                return false;
            }

        }
    }
}