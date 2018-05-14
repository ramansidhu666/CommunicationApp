using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Http;
using System.Web.SessionState;

using CommunicationApp.Models;
using CommunicationApp.Infrastructure;
using CommunicationApp.Services;
using AutoMapper;

namespace CommunicationApp.Controllers
{
    public class BaseController : Controller
    {
        public IUserService _UserService { get; set; }
        public IRoleService _RoleService { get; set; }
        public IUserRoleService _UserRoleService { get; set; }
        public IFormService _FormService { get; set; }
        public IRoleDetailService _RoleDetailService { get; set; }
        public ICustomerService _CustomerService { get; set; }

        public BaseController(ICustomerService CustomerService, IUserService UserService, IRoleService RoleService, IFormService FormService, IRoleDetailService RoleDetailService, IUserRoleService UserRoleService)
        {
            this._UserService = UserService;
            this._UserRoleService = UserRoleService;
            this._RoleService = RoleService;
            this._RoleDetailService = RoleDetailService;
            this._FormService = FormService;
            this._CustomerService = CustomerService;
        }
        public PartialViewResult DisplayMessage(string ShowMessage, string MessageBody)
        {
            ViewBag.ShowMessage = ShowMessage;
            ViewBag.MessageBody = MessageBody;
            return PartialView("DisplayMessage");
        }
        public PartialViewResult Paging()
        {
            return PartialView("Paging");
        }
        public PartialViewResult ShowProgressBar()
        {
            return PartialView("ShowProgressBar");
        }
        
        public RoleDetailModel UserPermission(string ControllerName)
        {
            RoleDetailModel roleDetail = new RoleDetailModel();
            try
            {
                if (ExcludePublicController().Contains(ControllerName.ToLower()))
                {
                    //Set True for Each Operation
                    roleDetail.IsView = true;
                    roleDetail.IsCreate = true;
                    roleDetail.IsEdit = true;
                    roleDetail.IsDelete = true;
                    roleDetail.IsDetail = true;
                    roleDetail.IsDownload = true;
                }
                else
                {
                    roleDetail = (Session["UserPermission"] as List<CommunicationApp.Models.RoleDetailModel>).Where(z => z.form.ControllerName.ToLower().Trim() == ControllerName.ToLower().Trim()).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                //Set False for Each Operation
                roleDetail.IsView = false;
                roleDetail.IsCreate = false;
                roleDetail.IsEdit = false;
                roleDetail.IsDelete = false;
                roleDetail.IsDetail = false;
                roleDetail.IsDownload = false;
                Response.Redirect("/Account/LogOn");
                string ErrorMsg = ex.Message.ToString();
                ErrorLogging.LogError(ex);
               
            }
            return roleDetail;
        }
        public List<string> ExcludePublicController()
        {
            List<string> lstController = new List<string>();
            //lstController.Add("home");
            lstController.Add("career");
            lstController.Add("chooseus");
            lstController.Add("contactus");
            lstController.Add("login");
            lstController.Add("authenticationservice");
            lstController.Add("error");

            return lstController;
        }

        public void UserPermissionAction(string ControllerName,string ActionName,string PreviousActionName="",string ShowMessage="", string MessageBody="")
        {
            RoleDetailModel roleDetail = UserPermission(ControllerName.ToLower());
            if ((ActionName.ToLower() == RoleAction.view.ToString()) && (!roleDetail.IsView)) //View Operation
            {
                if (PreviousActionName != "" && ShowMessage!="" && MessageBody!="") //Redirect
                {
                    Response.Redirect("/AuthenticationService/" + ShowMessage + "?ShowMessage=" + ShowMessage + "&&MessageBody=" + MessageBody);
                }
                else
                {
                    //Response.Redirect("/AuthenticationService");
                   Response.Redirect("/Account/LogOn");
                    //RedirectToAction("LogOn", "Account");
                }
            }
            else if ((ActionName.ToLower() == RoleAction.create.ToString()) && (!roleDetail.IsCreate)) //Create Operation
            {
                Response.Redirect("/AuthenticationService");
            }
            else if ((ActionName.ToLower() == RoleAction.edit.ToString()) && (!roleDetail.IsEdit)) //Edit Operation
            {
                Response.Redirect("/AuthenticationService");
            }
            else if ((ActionName.ToLower() == RoleAction.delete.ToString()) && (!roleDetail.IsDelete)) //Delete Operation
            {
                Response.Redirect("/AuthenticationService");
            }
            else if ((ActionName.ToLower() == RoleAction.detail.ToString()) && (!roleDetail.IsDetail)) //Detail Operation
            {
                Response.Redirect("/AuthenticationService");
            }
            else if ((ActionName.ToLower() == RoleAction.download.ToString()) && (!roleDetail.IsDownload)) //Download Operation
            {
                Response.Redirect("/AuthenticationService");
            }
        }
        public void SetSessionVariables(string UserName)
        {
            //UserModel user =_UserService.GetUserByName(UserName);

            var user = _UserService.GetUserByName(UserName);

            if (user != null) //By Email Id
            {

                var Role = _UserRoleService.Role().Where(x => x.UserId == user.UserId).FirstOrDefault();
                int RoleId = Role.RoleId; //Get RoleId

                var RoleDetails = _RoleDetailService.GetRoleDetails(RoleId);
                var models = new List<RoleDetailModel>();

                Mapper.CreateMap<CommunicationApp.Entity.RoleDetail, CommunicationApp.Models.RoleDetailModel>();
                foreach (var roledetail in RoleDetails)
                {
                    var _roleDetail= Mapper.Map<CommunicationApp.Entity.RoleDetail, CommunicationApp.Models.RoleDetailModel>(roledetail);
                    
                    FormModel formModal=new FormModel();
                    formModal.FormId=_roleDetail.FormId;
                    formModal.FormName = _FormService.GetForm(roledetail.FormId).FormName;
                    formModal.ControllerName = _FormService.GetForm(roledetail.FormId).ControllerName;
                    _roleDetail.form = formModal;
                    //_roleDetail.FormName = _FormService.GetForm(roledetail.FormId).ControllerName;
                    //_roleDetail.ControllerName =_FormService.GetForm(roledetail.FormId).ControllerName;
                    models.Add(_roleDetail);
                }
                var lstRoleDetail = models; //Get Permission


                Session["RoleType"] = _RoleService.GetRoles().Where(x => x.RoleId == RoleId).Select(x => x.RoleType).FirstOrDefault();
                Session["UserPermission"] = lstRoleDetail;
                
                Session["UserId"] = user.UserId; //Set User Id
                var Customer= _CustomerService.GetCustomers().Where(c => c.UserId == user.UserId).FirstOrDefault();
                if (Customer!=null)
                {
                    Session["CustomerID"] = Customer.CustomerId;
                    Session["Designation"] = Customer.Designation;
                    Session["AdminPhoto"] = Customer.PhotoPath;
                }                
                
                Session["CompanyID"] = user.CompanyID; //Set Company Id

                Session["CompanyName"] = user.Companies.CompanyName;//Set Company Name
                Session["LogoPath"] = user.Companies.LogoPath;
                
                Session["UserName"] = user.UserName;
                Session["FullUserName"] = (user.FirstName + " " + user.LastName).Trim();
                //Set Logo
            }
            
        }
	}
}