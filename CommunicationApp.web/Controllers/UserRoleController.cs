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
using CommunicationApp.Controllers;
using CommunicationApp.Models;
using CommunicationApp.Infrastructure;

namespace CommunicationApp.Web.Controllers
{
    public class UserRoleController :BaseController
    {
        public ICityService _CityService { get; set; }
        public IStateService _StateService { get; set; }
        public ICountryService _CountryService { get; set; }
        public ICompanyService _CompanyService { get; set; }
        public UserRoleController(ICustomerService CustomerService, IUserService UserService, ICompanyService CompanyService, IRoleService RoleService, IFormService FormService, IRoleDetailService RoleDetailService, IUserRoleService UserRoleService)
            : base(CustomerService,UserService, RoleService, FormService, RoleDetailService, UserRoleService)
        {
            this._UserRoleService = UserRoleService;
        }
        private void CheckPermission()
        {
            RoleDetailModel userRole = UserPermission("UserRole");
            TempData["View"] = userRole.IsView;
            TempData["Create"] = userRole.IsCreate;
            TempData["Edit"] = userRole.IsEdit;
            TempData["Delete"] = userRole.IsDelete;
            TempData["Detail"] = userRole.IsDetail;
        }
        // GET: /UserRole/
        public ActionResult Index(string operation, string ShowMessage, string MessageBody)
        {
           UserPermissionAction("case", RoleAction.view.ToString(), operation, ShowMessage, MessageBody);
            CheckPermission();
            
            var userroles = _UserRoleService.GetUserRoles();
            return View(userroles.ToList());
        }

        
        // GET: /UserRole/Details/5
        public ActionResult Details(int id)
        {
            UserPermissionAction("UserRole", RoleAction.detail.ToString());
            CheckPermission();
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserRole userrole = _UserRoleService.GetUserRole(id);
            if (userrole == null)
            {
                return HttpNotFound();
            }
            return View(userrole);
        }

        // GET: /UserRole/Create
        public ActionResult Create()
        {
            UserPermissionAction("UserRole", RoleAction.create.ToString());
            CheckPermission();
            ViewBag.RoleId = new SelectList(_UserRoleService.GetUserRoles(), "RoleId", "RoleName");
            ViewBag.UserId = new SelectList(_UserService.GetUsers(), "UserId", "FirstName");
            return View();
        }

        // POST: /UserRole/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="UserRoleId,UserId,RoleId")] UserRole userrole)
        {
            UserPermissionAction("UserRole", RoleAction.create.ToString());
            CheckPermission();
            if (ModelState.IsValid)
            {
                _UserRoleService.InsertUserRole(userrole);
                return RedirectToAction("Index");
            }

            ViewBag.RoleId = new SelectList(_UserRoleService.GetUserRoles(), "RoleId", "RoleName", userrole.RoleId);
            ViewBag.UserId = new SelectList(_UserService.GetUsers(), "UserId", "FirstName", userrole.UserId);
            return View(userrole);
        }

        // GET: /UserRole/Edit/5
        public ActionResult Edit(int id)
        {
            UserPermissionAction("UserRole", RoleAction.edit.ToString());
            CheckPermission();
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserRole userrole = _UserRoleService.GetUserRole(id);
            if (userrole == null)
            {
                return HttpNotFound();
            }
            ViewBag.RoleId = new SelectList(_UserRoleService.GetUserRoles(), "RoleId", "RoleName", userrole.RoleId);
            ViewBag.UserId = new SelectList(_UserService.GetUsers(), "UserId", "FirstName", userrole.UserId);
            return View(userrole);
        }

        // POST: /UserRole/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="UserRoleId,UserId,RoleId")] UserRole userrole)
        {
            UserPermissionAction("UserRole", RoleAction.edit.ToString());
            CheckPermission();
            if (ModelState.IsValid)
            {
                _UserRoleService.UpdateUserRole(userrole);
                return RedirectToAction("Index");
            }
            ViewBag.RoleId = new SelectList(_UserRoleService.GetUserRoles(), "RoleId", "RoleName", userrole.RoleId);
            ViewBag.UserId = new SelectList(_UserService.GetUsers(), "UserId", "FirstName", userrole.UserId);
            return View(userrole);
        }

        // GET: /UserRole/Delete/5
        public ActionResult Delete(int id)
        {
            UserPermissionAction("UserRole", RoleAction.delete.ToString());
            CheckPermission();
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserRole userrole = _UserRoleService.GetUserRole(id);
            if (userrole == null)
            {
                return HttpNotFound();
            }
            return View(userrole);
        }

        // POST: /UserRole/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UserRole userrole = _UserRoleService.GetUserRole(id);
            _UserRoleService.DeleteUserRole(userrole);
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
