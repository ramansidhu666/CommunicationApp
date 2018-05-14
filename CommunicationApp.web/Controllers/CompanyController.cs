
//using AmebaSoftwares.Interfaces;
using System;
using System.IO;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CommunicationApp.Services;
using CommunicationApp.Controllers;
using CommunicationApp.Infrastructure;
using CommunicationApp.Entity;
using AutoMapper;
using CommunicationApp.Models;

namespace CommunicationApp.Web.Controllers

{
    public class CompanyController : BaseController
    {

        public ICompanyService _CompanyService { get; set; }
        public ICityService _CityService { get; set; }
        public ICountryService _CountryService { get; set; }
        public IStateService _StateService { get; set; }

        public CompanyController(ICustomerService CustomerService,IUserService UserService, IRoleService RoleService, IUserRoleService UserRoleService, IFormService FormService, IRoleDetailService RoleDetailService, ICompanyService CompanyService,
           ICityService CityService, ICountryService CountryService, IStateService StateService)
            : base(CustomerService,UserService, RoleService, FormService, RoleDetailService, UserRoleService)
        {
            this._CompanyService = CompanyService;
            this._CityService = CityService;
            this._CountryService = CountryService;
            this._StateService = StateService;
        }
        //private UsersContext db = new UsersContext();
        //public IMembershipService MembershipService { get; set; }
        // GET: Company

        public ActionResult Index()
        {
            UserPermissionAction("company", RoleAction.create.ToString());
            CheckPermission();

            var companies = _CompanyService.GetCompanies();
            var models = new List<CompanyModel>();
            Mapper.CreateMap<CommunicationApp.Entity.Company, CommunicationApp.Models.CompanyModel>();
            foreach (var company in companies)
            {
                models.Add(Mapper.Map<CommunicationApp.Entity.Company, CommunicationApp.Models.CompanyModel>(company));
            }

            return View(models);
           // return View(db.tbCompanies.ToList());
        }

        public ActionResult DropDownTesting()
        {

            return View();
        }
        public JsonResult UploadFiles(IEnumerable<HttpPostedFileBase> files)
        {
            List<string> ret = new List<string>();

            foreach (var file in files)
            {
                var fileExt = Path.GetExtension(file.FileName);
                var fileName = file.FileName;
                var subPath = Server.MapPath("~/FileUpload/uploads");
                var path = Path.Combine(subPath, fileName);

                file.SaveAs(path);
                ret.Add(fileName);
            }
            return Json(ret);
        }

        public JsonResult DeleteFiles(string FileName)
        {
            string ret = "error";
            try
            {
                var subPath = Server.MapPath("~/FileUpload/uploads");
                var path = Path.Combine(subPath, FileName);
                FileInfo objfile = new FileInfo(path);
                if (objfile.Exists)//check file exsit or not
                {
                    objfile.Delete();
                }
                ret = "success";
            }
            catch (Exception ex)
            {
                ret = "error";
            }
            return Json(ret);
        }

        //Pemission Method
        private void CheckPermission()
        {
            RoleDetailModel roleDetail = UserPermission("company");
            TempData["View"] = roleDetail.IsView;
            TempData["Create"] = roleDetail.IsCreate;
            TempData["Edit"] = roleDetail.IsEdit;
            TempData["Delete"] = roleDetail.IsDelete;
            TempData["Detail"] = roleDetail.IsDetail;
        }
        //Create Compamy
        [HttpGet]
        public ActionResult Create()
        {
            //Check User Permission
            UserPermissionAction("company", RoleAction.create.ToString());
            CheckPermission();

            ViewBag.CountryID = new SelectList(_CountryService.GetCountries(), "CountryID", "CountryName");
            //ViewBag.CountryID = new SelectList(db.tbCountries, "CountryID", "CountryName");
            return View();
        }

        [HttpPost]
        public ActionResult Create([Bind(Include = "CompanyName,CompanyAddress,CountryID,StateID,CityID,EmailID,WebSite,PhoneNo,CreatedDate,ModifyDate,IsActive,LogoPath")]CompanyModel compnymodel, HttpPostedFileBase file)
        {
            UserPermissionAction("company", RoleAction.create.ToString());
            CheckPermission();
            try
            {
                TempData["ShowMessage"] = "error";
                TempData["MessageBody"] = "Please fill the required field with valid data";
                if (ModelState.IsValid)
                {
                    Company objcompany = _CompanyService.GetCompanies().Where(c => c.CompanyName.ToLower() == compnymodel.CompanyName).FirstOrDefault();

                    if (objcompany == null)
                    {
                        Mapper.CreateMap<CommunicationApp.Models.CompanyModel, CommunicationApp.Entity.Company>();
                        CommunicationApp.Entity.Company compnyentity = Mapper.Map<CommunicationApp.Models.CompanyModel, CommunicationApp.Entity.Company>(compnymodel);
                        //objcompany.LogoPath = ""; //this will reset after logo upload to local folder.
                       
                        
                         //Save the Logo in Folder
                        int CompanyID = compnymodel.CompanyID;
                        var fileExt = Path.GetExtension(file.FileName);
                        var fileName = CompanyID.ToString() + fileExt;
                        var subPath = Server.MapPath("~/CompanyLogo");
                        //Check SubPath Exist or Not
                        if (!Directory.Exists(subPath))
                        {
                            Directory.CreateDirectory(subPath);
                        }
                        //End : Check SubPath Exist or Not

                        var path = Path.Combine(subPath, fileName);
                        file.SaveAs(path);

                        var shortPath = "~/CompanyLogo/" + fileName;
                        CommonCls.CreateThumbnail(shortPath, 218, 84, false);
                        compnyentity.LogoPath = shortPath;
                        _CompanyService.InsertCompany(compnyentity);

                        //Update the Path in Database
                        //Company companyUpdate = _CompanyService.GetCompanies().Where(c => c.CompanyID == CompanyID).OrderBy(c => c.CompanyID).FirstOrDefault(); //(from m in db.tbCompanies where m.CompanyID == CompanyID orderby m.CompanyID descending select m).FirstOrDefault();
                        //companyUpdate.LogoPath = shortPath;
                        //_CompanyService.UpdateCompany(companyUpdate);
                        //// db.SaveChanges();
                        ////End : Update the Path in Database

                        TempData["ShowMessage"] = "success";
                        TempData["MessageBody"] = compnymodel.CompanyName + " is saved successfully.";
                        return RedirectToAction("Index");

                    }
                    else
                    {
                        TempData["ShowMessage"] = "error";
                        TempData["MessageBody"] = compnymodel.CompanyName + " is already exists.";
                    }
                }
            }

            catch (RetryLimitExceededException)
            {
                TempData["ShowMessage"] = "error";
                TempData["MessageBody"] = "Some unknown problem occured while proccessing save operation on " + compnymodel.CompanyName + " Company";

            }

            //ViewBag.CountryID = new SelectList(db.tbCountries, "CountryID", "CountryName");
            ViewBag.CountryID = new SelectList(_CountryService.GetCountries(), "CountryID", "CountryName", compnymodel.CountryID);

            return RedirectToAction("Create", "Company", new { id = compnymodel.CompanyID });

        }

        //Edit Company

        public ActionResult Edit(short id)
        {
            UserPermissionAction("company", RoleAction.edit.ToString());
            CheckPermission();

            if (id == null)
            {

                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Company company = _CompanyService.GetCompany(id);//db.tbCompanies.Find(id);
            Mapper.CreateMap<CommunicationApp.Entity.Company, CommunicationApp.Models.CompanyModel>();
            CommunicationApp.Models.CompanyModel compnymodel = Mapper.Map<CommunicationApp.Entity.Company, CommunicationApp.Models.CompanyModel>(company);
            if (company == null)
            {

                return HttpNotFound();
            }

            ViewBag.CityID = (compnymodel.CityID <= 0 ? "" : company.CityID.ToString());
            ViewBag.StateID = (compnymodel.StateID <= 0 ? "" : company.StateID.ToString());
            ViewBag.Countrylist = new SelectList(_CountryService.GetCountries(), "CountryID", "CountryName", company.CountryID);
            ViewBag.Citylist = new SelectList(_CityService.GetCities(), "CityID", "CityName", company.CityID);

            ViewBag.Statelist = new SelectList(_StateService.GetStates(), "StateID", "StateName", company.StateID);
            

            //SetFieldsForEdit(compnymodel);
            return View(compnymodel);

        }

        public void SetFieldsForEdit(CompanyModel compnymodel)
        {
            // ViewBag.CountryID = new SelectList(db.tbCountries, "CountryID", "CountryName", tbcompany.CountryID);
            ViewBag.StateID = (compnymodel.StateID <= 0 ? "" : compnymodel.StateID.ToString());
            ViewBag.CityID = (compnymodel.CityID <= 0 ? "" : compnymodel.CityID.ToString());
            ViewBag.CountryID = new SelectList(_CompanyService.GetCompanies(), "CountryID", "CountryName", compnymodel.CountryID);


        }
        [HttpPost]
        public ActionResult Edit([Bind(Include = "CompanyID,CompanyName,CompanyAddress,CountryID,StateID,CityID,EmailID,WebSite,PhoneNo,CreatedDate,ModifyDate,IsActive,LogoPath")]CompanyModel compnymodel, HttpPostedFileBase file)
        {
            UserPermissionAction("company", RoleAction.edit.ToString());
            CheckPermission();

            try
            {
                TempData["ShowMessage"] = "error";
                TempData["MessageBody"] = "Please fill the required field with valid data";
                if (ModelState.IsValid)
                {
                    Company objcompany = _CompanyService.GetCompanies().Where(c =>((c.CompanyName.ToLower().Trim() == compnymodel.CompanyName.ToLower().Trim() ) && c.CompanyID != compnymodel.CompanyID)).FirstOrDefault();
                   
                    
                    //db.tbCompanies.Where(c => c.CompanyName.ToLower().Trim() == tbcompany.CompanyName.ToLower().Trim() && c.CompanyID != tbcompany.CompanyID).FirstOrDefault();
                    if (objcompany == null)
                    {
                        Mapper.CreateMap<CommunicationApp.Models.CompanyModel, CommunicationApp.Entity.Company>();
                        CommunicationApp.Entity.Company compnyentity = Mapper.Map<CommunicationApp.Models.CompanyModel, CommunicationApp.Entity.Company>(compnymodel);
                        _CompanyService.UpdateCompany(compnyentity);
                       // db.Entry(tbcompany).State = EntityState.Modified;

                        var companyList = _CompanyService.GetCompanies().Where(c => c.CompanyID == compnymodel.CompanyID).FirstOrDefault(); //(from m in db.tbCompanies where m.CompanyID == tbcompany.CompanyID select m).FirstOrDefault();
                        var shortPath = companyList.LogoPath;
                        if (file != null)
                        {
                            if (companyList.LogoPath != "")
                            {   //Delete Old Image
                                string pathDel = Server.MapPath(companyList.LogoPath);
                                FileInfo objfile = new FileInfo(pathDel);
                                if (objfile.Exists) //check file exsit or not
                                {
                                    objfile.Delete();
                                }
                                //End :Delete Old Image
                            }

                            var fileExt = Path.GetExtension(file.FileName);
                            var fileName = compnymodel.CompanyID.ToString() + fileExt;
                            var subPath = Server.MapPath("~/CompanyLogo");
                            //Check SubPath Exist or Not
                            if (!Directory.Exists(subPath))
                            {
                                Directory.CreateDirectory(subPath);
                            }
                            //End : Check SubPath Exist or Not

                            var path = Path.Combine(subPath, fileName);
                            shortPath = "~/CompanyLogo/" + fileName;
                            file.SaveAs(path);
                            CommonCls.CreateThumbnail(shortPath, 218, 84, false);

                            //Update the Path in Database
                            Company compnyupdate = _CompanyService.GetCompanies().Where(c=>c.CompanyID==compnymodel.CompanyID).OrderByDescending(c=>c.CompanyID).FirstOrDefault();//(from m in db.tbCompanies where m.CompanyID == tbcompany.CompanyID orderby m.CompanyID descending select m).FirstOrDefault();
                            compnyupdate.LogoPath = shortPath;
                            _CompanyService.UpdateCompany(compnyupdate);
                           
                            //End : Update the Path in Database
                        }


                        TempData["ShowMessage"] = "success";
                        TempData["MessageBody"] = compnymodel.CompanyName + " is update successfully.";
                        return RedirectToAction("Index");

                    }
                    else
                    {
                        TempData["ShowMessage"] = "error";
                        TempData["MessageBody"] = compnymodel.CompanyName + " is already exists.";
                    }

                }

            }

            catch (RetryLimitExceededException)
            {

                TempData["ShowMessage"] = "error";
                TempData["MessageBody"] = "Some unknown problem occured while proccessing save operation on " + compnymodel.CompanyName + " ";

            }
            ViewBag.CityID = (compnymodel.CityID <= 0 ? "" : compnymodel.CityID.ToString());
            ViewBag.StateID = (compnymodel.StateID <= 0 ? "" : compnymodel.StateID.ToString());
            ViewBag.Countrylist = new SelectList(_CountryService.GetCountries(), "CountryID", "CountryName", compnymodel.CountryID);
            ViewBag.Citylist = new SelectList(_CityService.GetCities(), "CityID", "CityName", compnymodel.CityID);

            ViewBag.Statelist = new SelectList(_StateService.GetStates(), "StateID", "StateName", compnymodel.StateID);
            //SetFieldsForEdit(compnymodel);
            return View(compnymodel);
        }


        //public void SetFieldsForEdit(tbCompany tbcompany)
        //{
        //    // ViewBag.CountryID = new SelectList(db.tbCountries, "CountryID", "CountryName", tbcompany.CountryID);
        //    ViewBag.StateID = (tbcompany.StateID <= 0 ? "" : tbcompany.StateID.ToString());
        //    ViewBag.CityID = (tbcompany.CityID <= 0 ? "" : tbcompany.CityID.ToString());
        //    ViewBag.CountryID = new SelectList(db.tbCountries, "CountryID", "CountryName", tbcompany.CountryID);


        //}
        public JsonResult GetStates(string id)
        {
            Int32 CountryID = (id == "" ? -1 : Convert.ToInt32(id));
            var states = _StateService.GetStates().Where(x => x.CountryID == CountryID).ToList();


            List<SelectListItem> StateList = new List<SelectListItem>();
            StateList.Add(new SelectListItem { Text = "<--Select State-->", Value = "" });
            foreach (var State in states)
            {
                StateList.Add(new SelectListItem { Text = State.StateName, Value = State.StateID.ToString() });
            }
            return Json(new SelectList(StateList, "Value", "Text"));
        }

        public JsonResult GetCity(string id)
        {
            Int32 StateID = (id == "" ? -1 : Convert.ToInt32(id));

            var cities = _CityService.GetCities().Where(x => x.StateID == StateID).ToList();

            List<SelectListItem> CityList = new List<SelectListItem>();
            CityList.Add(new SelectListItem { Text = "<--Select City-->", Value = "" });
            foreach (var City in cities)
            {
                CityList.Add(new SelectListItem { Text = City.CityName, Value = City.CityID.ToString() });
            }
            return Json(new SelectList(CityList, "Value", "Text"));
        }
        //Details

        public ActionResult Details(int id)
        {
            UserPermissionAction("company", RoleAction.detail.ToString());

            CheckPermission();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }

            Company company = _CompanyService.GetCompany(id); //db.tbCompanies.Find(id);
            Mapper.CreateMap<CommunicationApp.Entity.Company, CommunicationApp.Models.CompanyModel>();
            CommunicationApp.Models.CompanyModel compnymodel = Mapper.Map<CommunicationApp.Entity.Company, CommunicationApp.Models.CompanyModel>(company);
            if (company == null)
            {
                return HttpNotFound();

            }
            return View(compnymodel);
        }

        //Delete


        public ActionResult Delete(int id)
        {
            UserPermissionAction("company", RoleAction.delete.ToString());
            CheckPermission();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company company = _CompanyService.GetCompany(id); //db.tbCompanies.Find(id);
            Mapper.CreateMap<CommunicationApp.Entity.Company, CommunicationApp.Models.CompanyModel>();
            CommunicationApp.Models.CompanyModel compnymodel = Mapper.Map<CommunicationApp.Entity.Company, CommunicationApp.Models.CompanyModel>(company);
            if (company == null)
            {
                return HttpNotFound();
            }
            return View(compnymodel);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UserPermissionAction("company", RoleAction.delete.ToString());
            CheckPermission();
            string CompanyName = "";
            string LogoPath = "";
            try
            {
                //Delete from Main Table
                Company company = _CompanyService.GetCompany(id);
                CompanyName = company.CompanyName;

                if (Convert.ToInt32(Session["CompanyID"].ToString()) != id)
                {

                    LogoPath = company.LogoPath;
                    _CompanyService.DeleteCompany(company);
                    //db.tbCompanies.Remove(tbcompany);
                    //db.SaveChanges();

                    if (LogoPath != "")
                    {
                        //Delete Logo from folder
                        string pathDel = Server.MapPath(LogoPath);
                        FileInfo objfile = new FileInfo(pathDel);
                        if (objfile.Exists) //check file exsit or not
                        {
                            objfile.Delete();
                        }
                        //End :Delete Logo from folder
                    }

                    TempData["ShowMessage"] = "success";
                    TempData["MessageBody"] = CompanyName + " is deleted successfully.";
                    //End : Delete from Main Table
                }
                else
                {
                    TempData["ShowMessage"] = "error";
                    TempData["MessageBody"] = "Please switch to another company to delete " + CompanyName + " company.";
                }
            }
            catch (Exception ex)
            {
                if (CommonCls.ErrorLog(ex.InnerException.InnerException.Message.ToString()) == "fk")
                {
                    TempData["ShowMessage"] = "error";
                    TempData["MessageBody"] = "This   " + CompanyName + " is used in another pages.";
                }
                else
                {
                    TempData["ShowMessage"] = "error";
                    TempData["MessageBody"] = "Some problem occured while proccessing delete operation on " + CompanyName + " Company.";
                }
            }

            return RedirectToAction("Index");
        }

        public ActionResult UploadAction()
        {

            return View();
        }

    }
}