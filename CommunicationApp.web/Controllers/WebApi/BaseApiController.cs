using EveryWhereCars.Contexts;
using EveryWhereCars.Entities;
using EveryWhereCars.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace EveryWhereCars.Web.Controllers.WebApi
{
    public class BaseApiController : ApiController
    {

        UsersContext db = new UsersContext();

        public BaseApiController()
        {
        }

        public RoleDetail UserPermission(string ControllerName)
        {
            RoleDetail roleDetail = new RoleDetail();
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
                    roleDetail = (HttpContext.Current.Session["UserPermission"] as List<EveryWhereCars.Entities.RoleDetail>).Where(z => z.form.ControllerName.ToLower().Trim() == ControllerName.ToLower().Trim()).FirstOrDefault();
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
            }
            return roleDetail;
        }
        public List<string> ExcludePublicController()
        {
            List<string> lstController = new List<string>();
            lstController.Add("home");
            lstController.Add("career");
            lstController.Add("chooseus");
            lstController.Add("contactus");
            lstController.Add("login");
            lstController.Add("authenticationservice");
            lstController.Add("error");

            return lstController;
        }

        public void UserPermissionAction(string ControllerName, string ActionName, string PreviousActionName = "", string ShowMessage = "", string MessageBody = "")
        {
            RoleDetail roleDetail = UserPermission(ControllerName.ToLower());
            if ((ActionName.ToLower() == RoleAction.view.ToString()) && (!roleDetail.IsView)) //View Operation
            {
                if (PreviousActionName != "" && ShowMessage != "" && MessageBody != "") //Redirect
                {
                    HttpContext.Current.Response.Redirect("/AuthenticationService/" + ShowMessage + "?ShowMessage=" + ShowMessage + "&&MessageBody=" + MessageBody);
                }
                else
                {
                    HttpContext.Current.Response.Redirect("/AuthenticationService");
                }
            }
            else if ((ActionName.ToLower() == RoleAction.create.ToString()) && (!roleDetail.IsCreate)) //Create Operation
            {
                HttpContext.Current.Response.Redirect("/AuthenticationService");
            }
            else if ((ActionName.ToLower() == RoleAction.edit.ToString()) && (!roleDetail.IsEdit)) //Edit Operation
            {
                HttpContext.Current.Response.Redirect("/AuthenticationService");
            }
            else if ((ActionName.ToLower() == RoleAction.delete.ToString()) && (!roleDetail.IsDelete)) //Delete Operation
            {
                HttpContext.Current.Response.Redirect("/AuthenticationService");
            }
            else if ((ActionName.ToLower() == RoleAction.detail.ToString()) && (!roleDetail.IsDetail)) //Detail Operation
            {
                HttpContext.Current.Response.Redirect("/AuthenticationService");
            }
            else if ((ActionName.ToLower() == RoleAction.download.ToString()) && (!roleDetail.IsDownload)) //Download Operation
            {
                HttpContext.Current.Response.Redirect("/AuthenticationService");
            }
        }
        public void SetSessionVariables(string UserName)
        {
            User user = db.GetUserByName(UserName);
            if (user != null) //By Email Id
            {
                int RoleId = Convert.ToInt32(db.GetRoleId(user.UserId)); //Get RoleId
                var lstRoleDetail = db.GetRoleDetails(RoleId); //Get Permission

                HttpContext.Current.Session["RoleType"] = db.Roles.Where(x => x.RoleId == RoleId).Select(x => x.RoleType).FirstOrDefault();
                HttpContext.Current.Session["UserPermission"] = lstRoleDetail;


                HttpContext.Current.Session["UserId"] = user.UserId; //Set User Id
                HttpContext.Current.Session["CompanyID"] = user.CompanyID; //Set Company Id

                HttpContext.Current.Session["CompanyName"] = user.Companys.CompanyName;//Set Company Name
                HttpContext.Current.Session["LogoPath"] = user.Companys.LogoPath;


                HttpContext.Current.Session["UserName"] = user.UserName;
                HttpContext.Current.Session["FullUserName"] = (user.FirstName + " " + user.LastName).Trim();
                //Set Logo
            }

        }
    }
}
