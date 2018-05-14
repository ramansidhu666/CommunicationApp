using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EveryWhereCars.Entity;
using EveryWhereCars.Data;
using EveryWhereCars.Services;
using EveryWhereCars.Controllers;
using EveryWhereCars.Infrastructure;
using EveryWhereCars.Models;
using AutoMapper;
using System.IO;

namespace EveryWhereCars.Web.Controllers
{
    public class CountryController : BaseController
    {
        public ICountryService _CountryService { get; set; }
        public CountryController( IUserService UserService,  ICountryService CountryService, IStateService StateService,  IRoleService RoleService, IFormService FormService, IRoleDetailService RoleDetailService, IUserRoleService UserRoleService)
           : base(UserService, RoleService, FormService, RoleDetailService, UserRoleService)
        
        {
            this._CountryService = CountryService;
          
       
    }
       
        // GET: /Country/
        public ActionResult Index(string operation, string ShowMessage, string MessageBody)
        {
            UserPermissionAction("Country", RoleAction.view.ToString(), operation, ShowMessage, MessageBody);
            CheckPermission();
            var countries = _CountryService.GetCountries();
            var models = new List<CountryModel>();
            Mapper.CreateMap<EveryWhereCars.Entity.Country, EveryWhereCars.Models.CountryModel>();
            foreach (var country in countries)
            {

                models.Add(Mapper.Map<EveryWhereCars.Entity.Country, EveryWhereCars.Models.CountryModel>(country));

            }
            return View(models);
        }
        private void CheckPermission()
        {
            RoleDetailModel userRole = UserPermission("Country");
            TempData["View"] = userRole.IsView;
            TempData["Create"] = userRole.IsCreate;
            TempData["Edit"] = userRole.IsEdit;
            TempData["Delete"] = userRole.IsDelete;
            TempData["Detail"] = userRole.IsDetail;
        }
        // GET: /Country/Details/5
        public ActionResult Details(int id)
        {
            UserPermissionAction("Country", RoleAction.detail.ToString());
            CheckPermission();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            Country country = _CountryService.GetCountry(id);
            Mapper.CreateMap<EveryWhereCars.Entity.Country, EveryWhereCars.Models.CountryModel>();
            EveryWhereCars.Models.CountryModel countrymodel = Mapper.Map<EveryWhereCars.Entity.Country, EveryWhereCars.Models.CountryModel>(country);
            if (countrymodel == null)
            {
                return HttpNotFound();
            }
            return View(countrymodel);
        }

        // GET: /Country/Create
        public ActionResult Create()
        {
            UserPermissionAction("Country", RoleAction.create.ToString());
            CheckPermission();
            return View();
        }

        // POST: /Country/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CurrencyName,CountryID,CountryName,CreatedOn,IsActive,ISOAlpha2,ISOAlpha3,ISONumeric,CurrencyCode,CurrrencySymbol,CountryFlag,SubUnitName,SubUnitValue")] CountryModel countrymodel, HttpPostedFileBase file)
        {
            try
            {
                TempData["ShowMessage"] = "error";
                TempData["MessageBody"] = "Please fill the required field with valid data";
                UserPermissionAction("Country", RoleAction.create.ToString());
                CheckPermission();
                var checkcountry = _CountryService.GetCountries().Where(c => c.CountryName == countrymodel.CountryName).FirstOrDefault();
                if (ModelState.IsValid)
                {
                    if (checkcountry == null)
                    {
                        Mapper.CreateMap<EveryWhereCars.Models.CountryModel, EveryWhereCars.Entity.Country>();
                        EveryWhereCars.Entity.Country countrytype = Mapper.Map<EveryWhereCars.Models.CountryModel, EveryWhereCars.Entity.Country>(countrymodel);
                        // countrytype.CompanyID = 1;
                        var fileExt = Path.GetExtension(file.FileName);
                        string fileName = Guid.NewGuid() + fileExt;
                        var subPath = Server.MapPath("~/CountryFlags");

                        //Check SubPath Exist or Not
                        if (!Directory.Exists(subPath))
                        {
                            Directory.CreateDirectory(subPath);
                        }
                        //End : Check SubPath Exist or Not

                        var path = Path.Combine(subPath, fileName);
                        file.SaveAs(path);
                        string URL =  "/CountryFlags/" + fileName;
                        countrytype.CountryFlag = URL;
                        _CountryService.InsertCountry(countrytype);
                        TempData["ShowMessage"] = "success";
                        TempData["MessageBody"] = "country is saved successfully.";
                        return RedirectToAction("Index");


                    }
                    else
                    {
                        TempData["ShowMessage"] = "error";
                        if (checkcountry.CountryName == countrymodel.CountryName.Trim())
                        {
                            TempData["MessageBody"] = countrymodel.CountryName + " is already exists.";
                        }

                        else
                        {
                            TempData["MessageBody"] = "Please fill the required field with valid data";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogging.LogError(ex);
                TempData["ShowMessage"] = "error";
                TempData["MessageBody"] = "Some unknown problem occured while proccessing save operation on " + countrymodel.CountryName;
            }



            return View(countrymodel);
        }
        
        // GET: /Country/Edit/5
        public ActionResult Edit(int id)
        {
            UserPermissionAction("Country", RoleAction.edit.ToString());
           CheckPermission();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Country country = _CountryService.GetCountry(id);
            Mapper.CreateMap<EveryWhereCars.Entity.Country, EveryWhereCars.Models.CountryModel>();
            EveryWhereCars.Models.CountryModel countrymodel = Mapper.Map<EveryWhereCars.Entity.Country, EveryWhereCars.Models.CountryModel>(country);
            if (country == null)
            {
                return HttpNotFound();
            }
            return View(countrymodel);
        }

        // POST: /Country/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CountryID,CountryName,CreatedOn,IsActive,ISOAlpha2,ISOAlpha3,ISONumeric,CurrencyName,CurrencyCode,CurrrencySymbol,CountryFlag,SubUnitName,SubUnitValue")] CountryModel countrymodel, HttpPostedFileBase file)
        {
            try
            {
                TempData["ShowMessage"] = "error";
                TempData["MessageBody"] = "Please fill the required field with valid data";
                UserPermissionAction("Country", RoleAction.edit.ToString());
                CheckPermission();
                var checkcountry = _CountryService.GetCountries().Where(c => c.CountryName == countrymodel.CountryName && c.CountryID != countrymodel.CountryID).FirstOrDefault();
                if (ModelState.IsValid)
                {
                    if (checkcountry == null)
                    {
                        Mapper.CreateMap<EveryWhereCars.Models.CountryModel, EveryWhereCars.Entity.Country>();
                        EveryWhereCars.Entity.Country country = Mapper.Map<EveryWhereCars.Models.CountryModel, EveryWhereCars.Entity.Country>(countrymodel);
                        if (file != null)
                        {
                            var fileExt = Path.GetExtension(file.FileName);
                            string fileName = Guid.NewGuid() + fileExt;
                            var subPath = Server.MapPath("~/CountryFlags");

                            //Check SubPath Exist or Not
                            if (!Directory.Exists(subPath))
                            {
                                Directory.CreateDirectory(subPath);
                            }
                            //End : Check SubPath Exist or Not

                            var path = Path.Combine(subPath, fileName);
                            file.SaveAs(path);
                            string URL = "/CountryFlags/" + fileName;
                            country.CountryFlag = URL;
                        }
                        else
                        {
                            country.CountryFlag = countrymodel.CountryFlag;
                        }

                        
                        _CountryService.UpdateCountry(country);
                        TempData["ShowMessage"] = "success";
                        TempData["MessageBody"] = "country is update successfully.";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["ShowMessage"] = "error";
                        if (checkcountry.CountryName.Trim() == countrymodel.CountryName.Trim())
                        {
                            TempData["MessageBody"] = checkcountry.CountryName + " is already exists.";
                        }

                        else
                        {
                            TempData["MessageBody"] = "Please fill the required field with valid data";
                        }
                    }


                }

            }
            catch (Exception ex)
            {

                ErrorLogging.LogError(ex);
                TempData["ShowMessage"] = "error";
                TempData["MessageBody"] = "Some unknown problem occured while proccessing save operation on " + countrymodel.CountryName;


            }
            return View(countrymodel);
        }

        // GET: /Country/Delete/5
        public ActionResult Delete(int id)
        {
            UserPermissionAction("Country", RoleAction.delete.ToString());
            CheckPermission();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Country country = _CountryService.GetCountry(id);
            Mapper.CreateMap<EveryWhereCars.Entity.Country, EveryWhereCars.Models.CountryModel>();
            EveryWhereCars.Models.CountryModel countrymodel = Mapper.Map<EveryWhereCars.Entity.Country, EveryWhereCars.Models.CountryModel>(country);
            if (country == null)
            {
                return HttpNotFound();
            }
            return View(countrymodel);
        }

        // POST: /Country/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UserPermissionAction("Country", RoleAction.delete.ToString());
            CheckPermission();
            Country country = _CountryService.GetCountry(id);
            _CountryService.DeleteCountry(country);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
