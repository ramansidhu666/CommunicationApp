using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CommunicationApp.Entity;
using CommunicationApp.Data;
using CommunicationApp.Services;
using AutoMapper;
using CommunicationApp.Controllers;
using CommunicationApp.Infrastructure;
using CommunicationApp.Models;
namespace CommunicationApp.Web.Controllers
{
    public class CityController : BaseController
    {
        //private DataContext db = new DataContext();
        public ICityService _CityService { get; set; }
        public IStateService _StateService { get; set; }
        public ICountryService _CountryService { get; set; }
        public CityController(ICustomerService CustomerService,ICityService CityService, IStateService StateService, ICountryService CountryService, IUserService UserService, IRoleService RoleService, IFormService FormService, IRoleDetailService RoleDetailService, IUserRoleService UserRoleService)
            : base(CustomerService,UserService, RoleService, FormService, RoleDetailService, UserRoleService)
        {
            this._CityService = CityService;
            this._StateService = StateService;
            this._CityService = CityService;
        }
        private void CheckPermission()
        {
            RoleDetailModel roledetail = UserPermission("City");
            TempData["View"] = roledetail.IsView;
            TempData["Create"] = roledetail.IsCreate;
            TempData["Edit"] = roledetail.IsEdit;
            TempData["Delete"] = roledetail.IsDelete;
            TempData["Detail"] = roledetail.IsDetail;
        }
        // GET: /City/
        public ActionResult Index(string operation, string ShowMessage, string MessageBody)
        {
            UserPermissionAction("City", RoleAction.view.ToString(), operation, ShowMessage, MessageBody);
            CheckPermission();
            var cities = _CityService.GetCities();
          
            return View(cities.ToList());
        }

       
        // GET: /City/Details/5
        public ActionResult Details(int id)
        {
            UserPermissionAction("City", RoleAction.detail.ToString());
            CheckPermission();
            if (id <=0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var city = _CityService.GetCity(id);
            Mapper.CreateMap<CommunicationApp.Entity.City, CommunicationApp.Models.CityModel>();
            CommunicationApp.Models.CityModel citymodel = Mapper.Map<CommunicationApp.Entity.City, CommunicationApp.Models.CityModel>(city);
          
            if (city == null)
            {
                return HttpNotFound();
            }
            return View(city);
        }

        // GET: /City/Create
        public ActionResult Create()
        {
            UserPermissionAction("City", RoleAction.create.ToString());
            CheckPermission();
            ViewBag.StateID = new SelectList(_StateService.GetStates(), "StateID", "StateName");
            return View();
        }

        // POST: /City/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="CityID,CityName,StateID,CreatedOn,IsActive")] City ctymodel)
        {
            UserPermissionAction("City", RoleAction.create.ToString());
            CheckPermission();
            if (ModelState.IsValid)
            {
                
                Mapper.CreateMap<CommunicationApp.Models.CityModel, CommunicationApp.Entity.City>();
                //CommunicationApp.Entity.City city = Mapper.Map<CommunicationApp.Models.CityModel, CommunicationApp.Entity.City>(ctymodel);
                _CityService.InsertCity(ctymodel); 
               
                return RedirectToAction("Index");
            }

            ViewBag.StateID = new SelectList(_StateService.GetStates(), "StateID", "StateName", ctymodel.StateID);
            return View(ctymodel);
        }

        // GET: /City/Edit/5
        public ActionResult Edit(int id)
        {
            UserPermissionAction("City", RoleAction.edit.ToString());
            CheckPermission();
            if (id <=0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            City city = _CityService.GetCity(id);
            Mapper.CreateMap<CommunicationApp.Entity.City, CommunicationApp.Models.CityModel>();
            CommunicationApp.Models.CityModel citymodel = Mapper.Map<CommunicationApp.Entity.City, CommunicationApp.Models.CityModel>(city);
            if (citymodel == null)
            {
                return HttpNotFound();
            }
            ViewBag.StateID = new SelectList(_CityService.GetCities(), "StateID", "StateName", city.StateID);
            return View(citymodel);
        }

        // POST: /City/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="CityID,CityName,StateID,CreatedOn,IsActive")] City city)
        {
            UserPermissionAction("City", RoleAction.edit.ToString());
            CheckPermission();
            if (ModelState.IsValid)
            {
                _CityService.UpdateCity(city);
               return RedirectToAction("Index");
            }
            ViewBag.StateID = new SelectList(_StateService.GetStates(), "StateID", "StateName", city.StateID);
            return View(city);
        }

        // GET: /City/Delete/5
        public ActionResult Delete(int id)
        {
            UserPermissionAction("City", RoleAction.delete.ToString());
            CheckPermission();
            if (id <=0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            City city = _CityService.GetCity(id);
            Mapper.CreateMap<CommunicationApp.Entity.City, CommunicationApp.Models.CityModel>();
            CommunicationApp.Models.CityModel citymodel = Mapper.Map<CommunicationApp.Entity.City, CommunicationApp.Models.CityModel>(city);
            if (citymodel == null)
            {
                return HttpNotFound();
            }
            return View(citymodel);
        }

        // POST: /City/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UserPermissionAction("City", RoleAction.delete.ToString());
            CheckPermission();
            City city = _CityService.GetCity(id);
            
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
