using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CommunicationApp.Models;
using CommunicationApp.Infrastructure;
using CommunicationApp.Services;
using CommunicationApp.Entity;

namespace CommunicationApp.Controllers
{
    public class StatesController : BaseController
    {
        public ICityService _CityService { get; set; }
        public IStateService _StateService { get; set; }
        public ICountryService _CountryService { get; set; }
        public StatesController(ICustomerService CustomerService, ICityService CityService, IStateService StateService, IFormService FormService, ICountryService CountryService, IUserService UserService, IRoleService RoleService, IRoleDetailService RoleDetailService, IUserRoleService UserRoleService)
            : base(CustomerService, UserService, RoleService, FormService, RoleDetailService, UserRoleService)
        {
            this._CityService = CityService;
            this._StateService = StateService;
            this._CityService = CityService;
        }
        private void CheckPermission()
        {
            RoleDetailModel roleDetail = UserPermission("states");
            TempData["Create"] = roleDetail.IsCreate;
            TempData["Delete"] = roleDetail.IsDelete;
            TempData["Detail"] = roleDetail.IsDetail;
            TempData["Edit"] = roleDetail.IsEdit;
            TempData["View"] = roleDetail.IsView;
        }

        // GET: States
        public ActionResult Index(string operation, string ShowMessage, string MessageBody)
        {
            UserPermissionAction("states", RoleAction.view.ToString(), operation, ShowMessage, MessageBody);
            CheckPermission();

            var tbStates = _StateService.GetStates();
            return View(tbStates.ToList());
        }

        // GET: States/Details/5
        public ActionResult Details(int id)
        {
            UserPermissionAction("states", RoleAction.detail.ToString());
            CheckPermission();

            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            State tbState = _StateService.GetState(id);
            if (tbState == null)
            {
                return HttpNotFound();
            }
            return View(tbState);
        }

        // GET: States/Create
        public ActionResult Create()
        {
            UserPermissionAction("states", RoleAction.create.ToString());
            CheckPermission();

            ViewBag.CountryID = new SelectList(_StateService.GetStates(), "CountryID", "CountryName");
            return View();
        }

        // POST: States/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "StateID,StateName,CountryID,CreatedOn,IsActive")] StateModel tbState)
        {
            UserPermissionAction("states", RoleAction.create.ToString());
            CheckPermission();

            try
            {
                
                TempData["ShowMessage"] = "error";
                TempData["MessageBody"] = "Please fill the required field with valid data";

                if (ModelState.IsValid)
                {
                   
                    State checkdupilcatestatename = _StateService.GetStates().Where(c => c.StateName.ToLower() == tbState.StateName.ToLower()).FirstOrDefault();
                    if (checkdupilcatestatename == null)
                    {
                        //_StateService.InsertState(tbState);

                        TempData["ShowMessage"] = "success";
                        TempData["MessageBody"] = tbState.StateName + " is created successfully.";
                        return RedirectToAction("Index", new { operation = "create", ShowMessage = TempData["ShowMessage"], MessageBody = TempData["MessageBody"] });

                    }
                    else
                    {
                        TempData["ShowMessage"] = "error";
                        TempData["MessageBody"] = tbState.StateName + " already exists.";
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.Message.ToString();
                ErrorLogging.LogError(ex);
                TempData["ShowMessage"] = "error";
                TempData["MessageBody"] = "Some unknown problem occured while proccessing save operation on " + tbState.StateID + " record";
            }

            ViewBag.CountryID = new SelectList(_StateService.GetStates(), "CountryID", "CountryName", tbState.CountryID);

            return View(tbState);
        }

        // GET: States/Edit/5
        public ActionResult Edit(int id)
        {
            UserPermissionAction("states", RoleAction.edit.ToString());
            CheckPermission();

            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            State tbState = _StateService.GetState(id);
            if (tbState == null)
            {
                return HttpNotFound();
            }

            ViewBag.CountryID = new SelectList(_StateService.GetStates(), "CountryID", "CountryName", tbState.CountryID);

            return View(tbState);
        }

        // POST: States/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "StateID,StateName,CountryID,CreatedOn,IsActive")] StateModel tbState)
        {
            UserPermissionAction("states", RoleAction.edit.ToString());
            CheckPermission();

            try
            {
                TempData["ShowMessage"] = "error";
                TempData["MessageBody"] = "Please fill the required field with valid data";

                if (ModelState.IsValid)
                {
                    State checkdupilcatestatename = _StateService.GetStates().Where(c => c.StateName.ToLower() == tbState.StateName.ToLower()).FirstOrDefault();
                    if (checkdupilcatestatename == null)
                    {
                       // _StateService.UpdateState(tbState);

                        TempData["ShowMessage"] = "success";
                        TempData["MessageBody"] = tbState.StateName + " is update successfully.";
                        return RedirectToAction("Index", new { operation = "edit", ShowMessage = TempData["ShowMessage"], MessageBody = TempData["MessageBody"] });

                    }
                    else
                    {
                        TempData["ShowMessage"] = "error";
                        TempData["MessageBody"] = tbState.StateName + " already exists.";
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.Message.ToString();
                ErrorLogging.LogError(ex);
                TempData["ShowMessage"] = "error";
                TempData["MessageBody"] = "Some unknown problem occured while proccessing save operation on " + tbState.StateID + " record";
            }

            ViewBag.CountryID = new SelectList(_CountryService.GetCountries(), "CountryID", "CountryName", tbState.CountryID);
            return View(tbState);
        }

        // GET: States/Delete/5
        public ActionResult Delete(int id)
        {
            UserPermissionAction("states", RoleAction.delete.ToString());
            CheckPermission();

            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            State tbState = _StateService.GetState(id);
            if (tbState == null)
            {
                return HttpNotFound();
            }
            return View(tbState);
        }

        // POST: States/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UserPermissionAction("states", RoleAction.delete.ToString());
            CheckPermission();

            string StateName = "";

            try
            {
                State tbState = _StateService.GetState(id);
                StateName = tbState.StateName;
                _StateService.DeleteState(tbState);

                TempData["ShowMessage"] = "success";
                TempData["MessageBody"] = tbState.StateName + " is deleted successfully.";
            }
            catch (Exception ex)
            {
                if (CommonCls.ErrorLog(ex.InnerException.InnerException.Message.ToString()) == "fk")
                {
                    TempData["ShowMessage"] = "error";
                    TempData["MessageBody"] = "Alert! cannot delete " + StateName + ". " + StateName + " is used in other forms as well.";
                }
                else
                {
                    TempData["ShowMessage"] = "error";
                    TempData["MessageBody"] = "Some issue occured while proccessing delete operation.";
                }
            }

            return RedirectToAction("Index", new { operation = "delete", ShowMessage = TempData["ShowMessage"], MessageBody = TempData["MessageBody"] });
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
