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

namespace EveryWhereCars.Web.Controllers
{
    public class RoleDetailController : BaseController
    {
        public RoleDetailController(IUserService UserService, IRoleService RoleService, IUserRoleService UserRoleService, IFormService FormService, IRoleDetailService RoleDetailService)
            : base(UserService, RoleService, FormService, RoleDetailService, UserRoleService)
        {
            
            this._FormService = FormService;
        }

        private void CheckPermission()
        {
            RoleDetailModel userRole = UserPermission("roledetail");
            TempData["View"] = userRole.IsView;
            TempData["Create"] = userRole.IsCreate;
            TempData["Edit"] = userRole.IsEdit;
            TempData["Delete"] = userRole.IsDelete;
            TempData["Detail"] = userRole.IsDetail;
        }
        // GET: /RoleDetail/
        public ActionResult Index(string operation, string ShowMessage, string MessageBody)
        {
            UserPermissionAction("RoleDetail", RoleAction.view.ToString(), operation, ShowMessage, MessageBody);
            CheckPermission();
            var roledetails = _RoleDetailService.GetRoleDetails();
            var models = new List<RoleDetailModel>();
            Mapper.CreateMap<EveryWhereCars.Entity.Form, EveryWhereCars.Models.FormModel>();
            foreach (var form in roledetails)
            {

                var _roledetail = Mapper.Map<EveryWhereCars.Entity.RoleDetail, EveryWhereCars.Models.RoleDetailModel>(form);
                RoleModel rolemodel = new RoleModel();
                rolemodel.RoleName = _RoleService.GetRole(form.RoleId).RoleName;
                _roledetail.Role = rolemodel;
                FormModel formmodel = new FormModel();
                formmodel.FormName = _FormService.GetForm(form.FormId).FormName;
                _roledetail.form = formmodel;
               models.Add(_roledetail);
            }
            return View(models);
        }

       
        // GET: /RoleDetail/Details/5
        public ActionResult Details(int id)
        {
            UserPermissionAction("RoleDetail", RoleAction.detail.ToString());
            CheckPermission();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoleDetail roeldetails = _RoleDetailService.GetRoleDetail(id);
           Mapper.CreateMap<EveryWhereCars.Entity.RoleDetail, EveryWhereCars.Models.RoleDetailModel>();
         
            EveryWhereCars.Models.RoleDetailModel roledetailmodel = Mapper.Map<EveryWhereCars.Entity.RoleDetail, EveryWhereCars.Models.RoleDetailModel>(roeldetails);
            RoleModel rolemodel = new RoleModel();
            rolemodel.RoleName = _RoleService.GetRole(roledetailmodel.RoleId).RoleName;
            roledetailmodel.Role = rolemodel;
            FormModel formmodel = new FormModel();
            formmodel.FormName = _FormService.GetForm(roledetailmodel.FormId).FormName;
            roledetailmodel.form = formmodel;
            if (roledetailmodel == null)
            {
                return HttpNotFound();
            }
            return View(roledetailmodel);
        }

        // GET: /RoleDetail/Create
        public ActionResult Create()
        {
            UserPermissionAction("RoleDetail", RoleAction.create.ToString());
            CheckPermission();
            ViewBag.FormId = new SelectList(_FormService.GetForms(), "FormId", "FormName");
            ViewBag.RoleId = new SelectList(_RoleService.GetRoles(), "RoleId", "RoleName");
            return View();
        }

        // POST: /RoleDetail/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="RoleDetailID,IsCreate,IsEdit,IsView,IsDelete,IsDetail,IsDownload,CreateDate,FormId,RoleId")] RoleDetailModel roledetailmodel)
        {
            UserPermissionAction("RoleDetail", RoleAction.create.ToString());
            CheckPermission();
            if (ModelState.IsValid)
            {
                Mapper.CreateMap<EveryWhereCars.Models.RoleDetailModel, EveryWhereCars.Entity.RoleDetail>();
                EveryWhereCars.Entity.RoleDetail roledetail = Mapper.Map<EveryWhereCars.Models.RoleDetailModel, EveryWhereCars.Entity.RoleDetail>(roledetailmodel);
                _RoleDetailService.InsertRoleDetail(roledetail);
                return RedirectToAction("Index");
            }

            ViewBag.FormId = new SelectList(_FormService.GetForms(), "FormId", "FormName", roledetailmodel.FormId);
            ViewBag.RoleId = new SelectList(_RoleService.GetRoles(), "RoleId", "RoleName", roledetailmodel.RoleId);
            return View(roledetailmodel);
        }

        // GET: /RoleDetail/Edit/5
        public ActionResult Edit(int id)
        {
            UserPermissionAction("RoleDetail", RoleAction.edit.ToString());
            CheckPermission();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoleDetail roeldetails = _RoleDetailService.GetRoleDetail(id);
            Mapper.CreateMap<EveryWhereCars.Entity.RoleDetail, EveryWhereCars.Models.RoleDetailModel>();
            EveryWhereCars.Models.RoleDetailModel roledetailmodel = Mapper.Map<EveryWhereCars.Entity.RoleDetail, EveryWhereCars.Models.RoleDetailModel>(roeldetails);
            if (roledetailmodel == null)
            {
                return HttpNotFound();
            }

            ViewBag.FormId = new SelectList(_FormService.GetForms(), "FormId", "FormName", roledetailmodel.FormId);
            ViewBag.RoleId = new SelectList(_RoleService.GetRoles(), "RoleId", "RoleName", roledetailmodel.RoleId);
            return View(roledetailmodel);
        }

        // POST: /RoleDetail/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="RoleDetailID,IsCreate,IsEdit,IsView,IsDelete,IsDetail,IsDownload,CreateDate,FormId,RoleId")] RoleDetailModel  roledetailmodel)
        {
            UserPermissionAction("RoleDetail", RoleAction.edit.ToString());
            CheckPermission();
            if (ModelState.IsValid)
            {
                
                Mapper.CreateMap<EveryWhereCars.Models.RoleDetailModel, EveryWhereCars.Entity.RoleDetail>();
                EveryWhereCars.Entity.RoleDetail roledetail = Mapper.Map<EveryWhereCars.Models.RoleDetailModel, EveryWhereCars.Entity.RoleDetail>(roledetailmodel);
                _RoleDetailService.UpdateRoleDetail(roledetail);
                return RedirectToAction("Index");
            }
            ViewBag.FormId = new SelectList(_FormService.GetForms(), "FormId", "FormName", roledetailmodel.FormId);
            ViewBag.RoleId = new SelectList(_RoleService.GetRoles(), "RoleId", "RoleName", roledetailmodel.RoleId);
            return View(roledetailmodel);
        }

        // GET: /RoleDetail/Delete/5
        public ActionResult Delete(int id)
        {
            UserPermissionAction("RoleDetail", RoleAction.delete.ToString());
            CheckPermission();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoleDetail roeldetails = _RoleDetailService.GetRoleDetail(id);
            Mapper.CreateMap<EveryWhereCars.Entity.RoleDetail, EveryWhereCars.Models.RoleDetailModel>();
            EveryWhereCars.Models.RoleDetailModel roledetailmodel = Mapper.Map<EveryWhereCars.Entity.RoleDetail, EveryWhereCars.Models.RoleDetailModel>(roeldetails);
            RoleModel rolemodel = new RoleModel();
            rolemodel.RoleName = _RoleService.GetRole(roledetailmodel.RoleId).RoleName;
            roledetailmodel.Role = rolemodel;
            FormModel formmodel = new FormModel();
            formmodel.FormName = _FormService.GetForm(roledetailmodel.FormId).FormName;
            roledetailmodel.form = formmodel;
            if (roledetailmodel == null)
            {
                return HttpNotFound();
            }
            return View(roledetailmodel);
        }

        // POST: /RoleDetail/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UserPermissionAction("RoleDetail", RoleAction.delete.ToString());
            CheckPermission();

            RoleDetail roeldetails = _RoleDetailService.GetRoleDetail(id);
            _RoleDetailService.DeleteRoleDetail(roeldetails);
            TempData["ShowMessage"] = "success";
            TempData["MessageBody"] = "RoleDetail is deleted successfully.";
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
