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
using EveryWhereCars.Controllers;
using EveryWhereCars.Infrastructure;
using EveryWhereCars.Models;
using EveryWhereCars.Services;
using AutoMapper;

namespace EveryWhereCars.Web.Controllers
{
    public class FormsController :BaseController
    {
        
      //  public IFormService _FormService { get; set; }

        public FormsController(IUserService UserService, IRoleService RoleService, IUserRoleService UserRoleService, IFormService FormService, IRoleDetailService RoleDetailService)
            : base(UserService, RoleService, FormService, RoleDetailService, UserRoleService)
        {
            
            this._FormService = FormService;
        }

        private void CheckPermission()
        {   
            RoleDetailModel userRole = UserPermission("forms");
            TempData["View"] = userRole.IsView;
            TempData["Create"] = userRole.IsCreate;
            TempData["Edit"] = userRole.IsEdit;
            TempData["Delete"] = userRole.IsDelete;
            TempData["Detail"] = userRole.IsDetail;
        }
        // GET: /Forms/
        public ActionResult Index(string operation, string ShowMessage, string MessageBody)
        {
            UserPermissionAction("forms", RoleAction.view.ToString(), operation, ShowMessage, MessageBody);
            CheckPermission();
            var forms = _FormService.GetForms();
            var models = new List<FormModel>();
            Mapper.CreateMap<EveryWhereCars.Entity.Form, EveryWhereCars.Models.FormModel>();
            foreach (var form in forms)
            {

                models.Add(Mapper.Map<EveryWhereCars.Entity.Form, EveryWhereCars.Models.FormModel>(form));

            }
            return View(models);
        }

       
        // GET: /Forms/Details/5
        public ActionResult Details(int id)
        {
            UserPermissionAction("forms", RoleAction.detail.ToString());
            CheckPermission();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Form form = _FormService.GetForm(id);
            Mapper.CreateMap<EveryWhereCars.Entity.Form, EveryWhereCars.Models.FormModel>();
            EveryWhereCars.Models.FormModel formmodel = Mapper.Map<EveryWhereCars.Entity.Form, EveryWhereCars.Models.FormModel>(form);
            if (formmodel == null)
            {
                return HttpNotFound();
            }
            return View(formmodel);
            
        }

        // GET: /Forms/Create
        public ActionResult Create()
        {
            UserPermissionAction("forms", RoleAction.create.ToString());
            CheckPermission();
            return View();
        }

        // POST: /Forms/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="FormId,FormName,ControllerName")] FormModel formmodel)
        {
            UserPermissionAction("forms", RoleAction.create.ToString());
            CheckPermission();
            var forms = _FormService.GetForms().Where(c => c.ControllerName == formmodel.ControllerName);
            if (ModelState.IsValid)
            {
                if (forms==null)
                {
                    Mapper.CreateMap<EveryWhereCars.Models.FormModel, EveryWhereCars.Entity.Form>();
                    EveryWhereCars.Entity.Form form = Mapper.Map<EveryWhereCars.Models.FormModel, EveryWhereCars.Entity.Form>(formmodel);
                    _FormService.InsertForm(form);
                    TempData["ShowMessage"] = "success";
                    TempData["MessageBody"] = "form is saved successfully.";


                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ShowMessage"] = "Failiur";
                    TempData["MessageBody"] = "controllername is already exist.";
                }
               
            }

            return View(formmodel);
        }

        // GET: /Forms/Edit/5
        public ActionResult Edit(int id)
        {
            UserPermissionAction("forms", RoleAction.edit.ToString());
            CheckPermission();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Form form = _FormService.GetForm(id);
            Mapper.CreateMap<EveryWhereCars.Entity.Form, EveryWhereCars.Models.FormModel>();
            EveryWhereCars.Models.FormModel formmodel = Mapper.Map<EveryWhereCars.Entity.Form, EveryWhereCars.Models.FormModel>(form);
            if (formmodel == null)
            {
                return HttpNotFound();
            }
            return View(formmodel);
            
        }

        // POST: /Forms/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="FormId,FormName,ControllerName")] FormModel formmodel)
        {
            UserPermissionAction("forms", RoleAction.edit.ToString());
            CheckPermission();
            var forms = _FormService.GetForms().Where(c => c.ControllerName == formmodel.ControllerName && c.FormId!=formmodel.FormId);
            if (ModelState.IsValid)
            {
                if (forms == null)
                {
                    Mapper.CreateMap<EveryWhereCars.Models.FormModel, EveryWhereCars.Entity.Form>();
                    EveryWhereCars.Entity.Form form = Mapper.Map<EveryWhereCars.Models.FormModel, EveryWhereCars.Entity.Form>(formmodel);
                    _FormService.UpdateForm(form);
                    TempData["ShowMessage"] = "success";
                    TempData["MessageBody"] = "form is update successfully.";
                    return RedirectToAction("Index");
                }

                else
                {
                    TempData["ShowMessage"] = "Failiur";
                    TempData["MessageBody"] = "form is not update successfully.";
                }
                
            }
            return View(formmodel);
        }

        // GET: /Forms/Delete/5
        public ActionResult Delete(int id)
        {
            UserPermissionAction("forms", RoleAction.delete.ToString());
            CheckPermission();
            Form form = _FormService.GetForm(id);
            Mapper.CreateMap<EveryWhereCars.Entity.Form, EveryWhereCars.Models.FormModel>();
            EveryWhereCars.Models.FormModel formmodel = Mapper.Map<EveryWhereCars.Entity.Form, EveryWhereCars.Models.FormModel>(form);
            if (formmodel == null)
            {
                return HttpNotFound();
            }
            return View(formmodel);
        }

        // POST: /Forms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UserPermissionAction("forms", RoleAction.delete.ToString());
            CheckPermission();
            Form form = _FormService.GetForm(id);
            _FormService.DeleteForm(form);
            TempData["ShowMessage"] = "success";
            TempData["MessageBody"] = "from is deleted successfully.";
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
