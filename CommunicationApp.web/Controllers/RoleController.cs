using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CommunicationApp.Models;
using CommunicationApp.Infrastructure;
using System.Data.Entity;
using CommunicationApp.Services;
using CommunicationApp.Entity;
using AutoMapper;

namespace CommunicationApp.Controllers
{
    public class RoleController : BaseController
    {
        public ICompanyService _CompanyService { get; set; }
        

        public RoleController(ICustomerService CustomerService, IUserService _UserService,ICompanyService CompanyService, IUserService UserService, IRoleService RoleService, IFormService FormService, IRoleDetailService RoleDetailService, IUserRoleService UserRoleService)
            : base(CustomerService, UserService, RoleService, FormService, RoleDetailService, UserRoleService)
        {
            this._UserService = UserService;
            this._CompanyService = CompanyService;
            this._CustomerService = CustomerService;
            this._RoleService = RoleService;
         }
        private void CheckPermission()
        {
            RoleDetailModel roleDetail = UserPermission("role");
            TempData["View"] = roleDetail.IsView;
            TempData["Create"] = roleDetail.IsCreate;
            TempData["Edit"] = roleDetail.IsEdit;
            TempData["Delete"] = roleDetail.IsDelete;
            TempData["Detail"] = roleDetail.IsDetail;
        }
        
        // GET: /Role/

       // UsersContext _usersContext = new UsersContext();
        public ActionResult Index(string operation, string ShowMessage, string MessageBody)
        {
            UserPermissionAction("role", RoleAction.view.ToString(), operation, ShowMessage, MessageBody);
            //Check User Permission
            CheckPermission();

            var lsts = _RoleService.GetRole();
            var models = new List<RoleModel>();
            Mapper.CreateMap<CommunicationApp.Entity.Role, CommunicationApp.Models.RoleModel>();
            foreach (var lst in lsts)
            {
                models.Add(Mapper.Map<CommunicationApp.Entity.Role, CommunicationApp.Models.RoleModel>(lst));
            }
            return View(models);
        }

        public ActionResult Create()
        {
            UserPermissionAction("role", RoleAction.create.ToString());
            //Check User Permission
            CheckPermission();
           // ViewBag.VehicleTypeId = new SelectList(_VehicleTypeService.GetVehicleTypes(), "VehicleTypeId", "VehicleTypeName");
            ViewBag.RoleType = new SelectList(_RoleService.GetRoles(), "RoleId", "RoleType"); 
            return View();
        }

        [HttpPost]
        public ActionResult Create(RoleModel role)
        {
            UserPermissionAction("role", RoleAction.create.ToString());
            //Check User Permission
            CheckPermission();

            ViewBag.RoleType = new SelectList(_RoleService.GetRoles(), "RoleTypeId", "RoleType");
            try
            {
                TempData["ShowMessage"] = "error";
                TempData["MessageBody"] = "Please fill the required field with valid data";
                if (ModelState.IsValid)
                {
                    
                    var rolemodel = _RoleService.GetRoles().Where(x => x.RoleName.ToLower() == role.RoleName.ToLower()).FirstOrDefault();
                    if (rolemodel == null)
                    {
                        //Mapper.CreateMap<CommunicationApp.Models.RoleModel, CommunicationApp.Entity.Role>();
                        //CommunicationApp.Entity.Role roles = Mapper.Map<CommunicationApp.Models.RoleModel, CommunicationApp.Entity.Role>(rolemodel);
                        //_RoleService.InsertRole(roles);
                        TempData["ShowMessage"] = "success";
                        TempData["MessageBody"] = role.RoleName + " is saved successfully.";
                        return RedirectToAction("Index", "Role");
                    }
                    else
                    {
                        TempData["ShowMessage"] = "error";
                        if (rolemodel.RoleName.ToLower() == role.RoleName.ToLower()) //Check Role Name
                        {
                            TempData["MessageBody"] = role.RoleName + " is already exist.";
                        }
                        else
                        {
                            TempData["MessageBody"] = "Some unknown problem occured while proccessing save operation on " + role.RoleName + " role.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.Message.ToString();
                ErrorLogging.LogError(ex);
                TempData["ShowMessage"] = "error";
                TempData["MessageBody"] = "Some unknown problem occured while proccessing save operation on " + role.RoleName + " role.";
            }
            return View(role);
            
        }
        public ActionResult Edit(int id)
        {
            UserPermissionAction("role", RoleAction.edit.ToString());
            //Check User Permission
            CheckPermission();

            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Role role = _RoleService.GetRole(id);
           
            Mapper.CreateMap<CommunicationApp.Entity.Role, CommunicationApp.Models.RoleModel>();
            CommunicationApp.Models.RoleModel rolemodel = Mapper.Map<CommunicationApp.Entity.Role, CommunicationApp.Models.RoleModel>(role);
            ViewBag.RoleType = new SelectList(_RoleService.GetRoles(), "RoleId", "RoleType", role.RoleId);
            ViewBag.Rolelist = new SelectList(_RoleService.GetRoles(), "RoleId", "RoleType", role.RoleId);
            //ViewBag.Citylist = new SelectList(_CityService.GetCities(), "CityID", "CityName", company.CityID);
            return View(rolemodel);
        }
        [HttpPost]
        public ActionResult Edit(RoleModel role, int Id)
        {
            UserPermissionAction("role", RoleAction.edit.ToString());
            //Check User Permission
            CheckPermission();

            ViewBag.RoleType = new SelectList(_RoleService.GetRoles(), "RoleTypeId", "RoleType", role.RoleType); 
            try
            {
                TempData["ShowMessage"] = "error";
                TempData["MessageBody"] = "Please fill the required field with valid data";

                var rolemodel = _RoleService.GetRoles().Where(x => x.RoleName.ToLower() == role.RoleName.ToLower()).FirstOrDefault();
                if (rolemodel == null)
                {
                    //_RoleService.UpdateRole(role);

                    TempData["ShowMessage"] = "success";
                    TempData["MessageBody"] = role.RoleName + " is update successfully.";

                    return RedirectToAction("Index", "Role");
                }
                else
                {
                    TempData["ShowMessage"] = "error";
                    if (rolemodel.RoleName.ToLower() == role.RoleName.ToLower()) //Check Role Name
                    {
                        TempData["MessageBody"] = role.RoleName + " is already exist.";
                    }
                    else
                    {
                        TempData["MessageBody"] = "Some unknown problem occured while proccessing update operation on " + role.RoleName + " role.";
                    }
                }
            }
            catch (Exception ex) 
            {
                string ErrorMsg = ex.Message.ToString();
                ErrorLogging.LogError(ex);
                TempData["ShowMessage"] = "error";
                TempData["MessageBody"] = "Some unknown problem occured while proccessing update operation on " + role.RoleName + " role.";
            }
            return View(role);
        }

        public ActionResult Details(int Id)
        {
            UserPermissionAction("role", RoleAction.detail.ToString());
            //Check User Permission
            CheckPermission();

            Role role = _RoleService.GetRole(Id);

            Mapper.CreateMap<CommunicationApp.Entity.Role, CommunicationApp.Models.RoleModel>();
            CommunicationApp.Models.RoleModel rolemodel = Mapper.Map<CommunicationApp.Entity.Role, CommunicationApp.Models.RoleModel>(role);
            return View(rolemodel);
        }
        public ActionResult Delete(int? id)
        {
            UserPermissionAction("role", RoleAction.delete.ToString());
            //Check User Permission
            CheckPermission();

            string RoleName = "";
            try
            {
               // RoleModel role = _usersContext.Roles.Where(z => z.RoleId == id).FirstOrDefault();
                Role role = _RoleService.GetRoles().Where(c => c.RoleId == id).FirstOrDefault();

             
               
                RoleName = role.RoleName;

                _RoleService.DeleteRole(role);

                TempData["ShowMessage"] = "success";
                TempData["MessageBody"] = RoleName + " is deleted successfully.";
            }
            catch (Exception ex)
            {
                if (CommonCls.ErrorLog(ex.InnerException.InnerException.Message.ToString()) == "fk")
                {
                    TempData["ShowMessage"] = "error";
                    TempData["MessageBody"] = "This role " + RoleName + " is used in another pages.";
                    TempData["MessageBody"] = "" + RoleName + " cannot be deleted." + RoleName + " is used in other pages.";
                }
                else
                {
                    TempData["ShowMessage"] = "error";
                    TempData["MessageBody"] = "Some problem occured while proccessing delete operation on " + RoleName + " role.";
                }
            }
            return RedirectToAction("Index", "Role");
        }
        public ActionResult RevokePermission(int RoleId)
        {
            UserPermissionAction("role", RoleAction.delete.ToString());
            //Check User Permission
            CheckPermission();

            try
            {
                List<RoleDetail> roleList = _RoleDetailService.GetRoleDetails().Where(z => z.RoleId == RoleId).ToList();
                foreach (RoleDetail role in roleList)
                {
                    _RoleDetailService.DeleteRoleDetail(role);
                }
                TempData["ShowMessage"] = "success";
                TempData["MessageBody"] = "Permission revoked successfully.";
            }
            catch (Exception ex)
            {
                if (CommonCls.ErrorLog(ex.InnerException.InnerException.Message.ToString()) == "fk")
                {
                    TempData["ShowMessage"] = "error";
                    TempData["MessageBody"] = "This is used in another pages.";
                }
                else
                {
                    TempData["ShowMessage"] = "error";
                    TempData["MessageBody"] = "Some problem occured while proccessing on revoke operation.";
                }
            }
            return RedirectToAction("Index", "Role");
        }
        public ActionResult AssignPermission(int RoleId)
        {
           
            ViewBag.RoleId = RoleId;
           // ViewData["RoleName"] = db.Roles.Where(z => z.RoleId == RoleId).Select(z => z.RoleName).FirstOrDefault().ToString();
            ViewData["RoleName"] = _RoleService.GetRoles().Where(c => c.RoleId == RoleId).Select(c => c.RoleName).FirstOrDefault().ToString();
            List<CommunicationApp.Entity.Form> lstControllerName =_FormService.GetForms();
            ViewBag.lstControllerName = lstControllerName;

            List<CommunicationApp.Entity.RoleDetail> lstRoleDetail = _RoleDetailService.GetRoleDetails();
            ViewBag.lstRoleDetail = lstRoleDetail;

            var model = new RoleDetailViewModel();
            foreach (var roleDetail in lstControllerName)
            {
                var _lst = lstRoleDetail.Where(z => z.RoleId == RoleId && z.FormId == roleDetail.FormId).FirstOrDefault();

                bool _IsCreate = false;
                bool _IsEdit = false;
                bool _IsDelete = false;
                bool _IsView = false;
                bool _IsDownload = false;
                bool _IsDetail = false;

                try
                {
                    _IsCreate = _lst.IsCreate;
                    _IsDelete = _lst.IsDelete;
                    _IsDownload = _lst.IsDownload;
                    _IsEdit = _lst.IsEdit;
                    _IsView = _lst.IsView;
                    _IsDetail = _lst.IsDetail;
                }
                catch (Exception ex) {
                    string ErrorMsg = ex.Message.ToString();
                    ErrorLogging.LogError(ex);
                }

                var editorViewModel = new RoleDetailEditorViewModel()
                {
                    RoleId = RoleId,
                    FormId = roleDetail.FormId,
                    FormName = roleDetail.FormName,
                    IsCreate = _IsCreate,
                    IsEdit = _IsEdit,
                    IsDelete = _IsDelete,
                    IsView = _IsView,
                    IsDetail = _IsDetail,
                    IsDownload = _IsDownload
                };
                model.RoleDetail.Add(editorViewModel);
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult AssignPermission(RoleDetailViewModel model)
        {
            if (model != null)
            {
                int RoleId = model.RoleDetail.Select(x => x.RoleId).FirstOrDefault();
                //Delete All the Roles
                //List<RoleDetailModel> _lst = _usersContext.RoleDetails.Where(z => z.RoleId == RoleId).ToList();
                List<RoleDetail> _lst = _RoleDetailService.GetRoleDetails().Where(c => c.RoleId == RoleId).ToList();
                foreach (RoleDetail roled in _lst)
                {
                    _RoleDetailService.DeleteRoleDetail(roled);
                }
                //Insert All New Roles
                foreach(var modelObj in model.RoleDetail)
                {
                    RoleDetailModel Roles = new RoleDetailModel();
                    Roles.FormId = modelObj.FormId;
                    Roles.RoleId = modelObj.RoleId;
                    Roles.IsCreate = modelObj.IsCreate;
                    Roles.IsDelete = modelObj.IsDelete;
                    Roles.IsDownload = modelObj.IsDownload;
                    Roles.IsView = modelObj.IsView;
                    Roles.IsEdit = modelObj.IsEdit;
                    Roles.IsDetail = modelObj.IsDetail;
                    Roles.CreateDate = DateTime.Now;
                   // _RoleDetailService.InsertRoleDetail(Roles);
                }

            }
            return RedirectToAction("Index", "Role");
        }
    }
}