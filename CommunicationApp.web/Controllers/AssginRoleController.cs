using CommunicationApp.Models;
using CommunicationApp.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommunicationApp.Services;
using CommunicationApp.Entity;
using AutoMapper;

namespace CommunicationApp.Controllers
{
    public class AssginRoleController : BaseController
    {
        public AssginRoleController(ICustomerService CustomerService, IUserRoleService UserRoleService, IUserService UserService, IRoleService RoleService, IUserService _UserService, ICompanyService CompanyService, IFormService FormService, IRoleDetailService RoleDetailService)
            : base(CustomerService,UserService, RoleService, FormService, RoleDetailService, UserRoleService)
        {
            this._UserService = UserService;
            this._RoleService = RoleService;
            this._UserRoleService = UserRoleService;
        }
        private void CheckPermission()
        {
            RoleDetailModel roleDetail = UserPermission("assginrole");
            TempData["View"] = roleDetail.IsView;
            TempData["Create"] = roleDetail.IsCreate;
            TempData["Edit"] = roleDetail.IsEdit;
            TempData["Delete"] = roleDetail.IsDelete;
            TempData["Detail"] = roleDetail.IsDetail;
        }

        //
        // GET: /AssginRole/
        public ActionResult Index(string operation, string ShowMessage, string MessageBody, string SearchUsername, string SearchEmailid)
        {
            UserPermissionAction("assginrole", RoleAction.view.ToString(), operation, ShowMessage, MessageBody);
            CheckPermission();

            int CompanyID = Convert.ToInt32(Session["CompanyID"]);
            int UserId = Convert.ToInt32(Session["UserId"]);

            ViewBag.RoleId = new SelectList(_RoleService.GetRole(true), "RoleId", "RoleName");

            //var userlist = _UserService.GetUsers().Where(z => z.UserName.ToLower() != "administrator" && z.UserId != UserId && z.CompanyID == CompanyID).OrderBy(z => z.UserName).ToList();
            var models = new List<UserRoleModel>();
            var userrollist = _UserRoleService.GetUserRoles();
           
            Mapper.CreateMap<CommunicationApp.Entity.UserRole, CommunicationApp.Models.UserRoleModel>();
          
            foreach (var userrol in userrollist)
            {

               var _userrol= Mapper.Map<CommunicationApp.Entity.UserRole, CommunicationApp.Models.UserRoleModel>(userrol);
                UserModel usermodel = new UserModel();
                usermodel.UserName = _UserService.GetUserById(userrol.UserId).UserName;
                usermodel.UserEmailAddress = _UserService.GetUserById(userrol.UserId).UserEmailAddress;
                _userrol.User = usermodel;
                models.Add(_userrol);
            }

           
            ViewBag.UserRoleList = models;

            ViewBag.UserList = _UserService.GetUsers().Where(z => z.UserName.ToLower() != "administrator" && z.UserId != UserId && z.CompanyID == CompanyID).OrderBy(z => z.UserName).ToList();
            //Search Users via username or email.
            if (!string.IsNullOrEmpty(SearchUsername) )
            {
                ViewBag.UserList = _UserService.GetUsers().Where(u => u.UserName.ToLower().Contains(SearchUsername.ToLower()) 
                   
                    && u.UserName.ToLower() != "administrator"
                    && u.UserId != UserId
                    && u.CompanyID == CompanyID).OrderBy(u => u.UserEmailAddress);
            }

            if(!string.IsNullOrEmpty(SearchEmailid))
            {

                ViewBag.UserList = _UserService.GetUsers().Where(u=>
                       u.UserEmailAddress.ToLower().Contains(SearchEmailid.ToLower())
                      && u.UserName.ToLower() != "administrator"
                      && u.UserId != UserId
                      && u.CompanyID == CompanyID).OrderBy(u => u.UserEmailAddress);
            }
           

            


            return View();
        }
        public ActionResult Create()
        {
            UserPermissionAction("assginrole", RoleAction.create.ToString());
            CheckPermission();

            ViewBag.UserList = _UserService.GetUsers().OrderBy(z => z.UserName).ToList();
            return View();
        }
        [HttpPut]
        public ActionResult Create(FormCollection frm)
        {
            UserPermissionAction("assginrole", RoleAction.create.ToString());
            CheckPermission();

            ViewBag.UserList = _UserService.GetUsers().OrderBy(z => z.UserName).ToList();
            return View();
        }
        public JsonResult UpdateRole(int UserId, int RoleId, int Id)
        {
            UserPermissionAction("assginrole", RoleAction.edit.ToString());
            CheckPermission();

            UserRole userrole = _UserRoleService.GetUserRoles().Where(z => z.UserId == UserId).FirstOrDefault();


            if (userrole != null)
            {
                userrole.UserId = UserId;
                userrole.RoleId = RoleId;
            }
            else
            {
                UserRole userroleentity = new UserRole();
                userroleentity.UserId = UserId;
                userroleentity.RoleId = RoleId;
                _UserRoleService.InsertUserRole(userroleentity);
            }
            _UserRoleService.UpdateUserRole(userrole);

            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}