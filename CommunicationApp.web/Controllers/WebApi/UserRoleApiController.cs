using AutoMapper;
using CommunicationApp.Infrastructure;
using CommunicationApp.Models;
using CommunicationApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CommunicationApp.Web.Controllers.WebApi
{
    [RoutePrefix("UserRoles")]
    public class UserRoleApiController : ApiController
    {
         public IUserRoleService _UserRoleservice { get; set; }

         public UserRoleApiController(IUserRoleService UserRoleservic)
        {
            this._UserRoleservice = UserRoleservic;
        }

        //Get api/Userroles/Userrole
        [Route("GetAllUserRole")]
        [HttpGet]
         public IHttpActionResult GetAllUserRole()
        {
            var Userrole = _UserRoleservice.GetUserRoles();
             var models = new List<UserRoleModel>();
             Mapper.CreateMap<CommunicationApp.Entity.UserRole, CommunicationApp.Models.UserRoleModel>();
             foreach (var userrole in Userrole)
            {
                models.Add(Mapper.Map<CommunicationApp.Entity.UserRole, CommunicationApp.Models.UserRoleModel>(userrole));
               
            }

            return Json(models);
        }

    
        // GET api/Form/GetFormById
        //http://swapstff.com/CustomerId/GetCustomerByID/1
        [Route("GetUserRoleByID/{UserRoleId}")]
        [HttpGet]
        public IHttpActionResult GetUserRoleByID(int UserRoleId)
        {
            var Userrole = _UserRoleservice.GetUserRole(UserRoleId);
            Mapper.CreateMap<CommunicationApp.Entity.UserRole, CommunicationApp.Models.UserRoleModel>();
            CommunicationApp.Models.UserRoleModel userrolemodel = Mapper.Map<CommunicationApp.Entity.UserRole, CommunicationApp.Models.UserRoleModel>(Userrole);
            return Json(userrolemodel);
        }

        [Route("SaveUserRole")]
        [HttpPost]
        public HttpResponseMessage SaveUserRole([FromBody]UserRoleModel userrolemodel)
        {
            string UserRoleID = "-1";
        
            try
            {
                Mapper.CreateMap<CommunicationApp.Models.UserRoleModel, CommunicationApp.Entity.UserRole>();
                CommunicationApp.Entity.UserRole Userrole = Mapper.Map<CommunicationApp.Models.UserRoleModel, CommunicationApp.Entity.UserRole>(userrolemodel);

                if (Userrole.UserRoleId <= 0) //new 
                {

                    //Insert the Country
                    _UserRoleservice.InsertUserRole(Userrole); //Save Operation
                    //End : Insert the Customer
                }
                else
                {
                    _UserRoleservice.UpdateUserRole(Userrole); //Update Operation
                    
                }
                UserRoleID = Userrole.UserRoleId.ToString();

                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("success", UserRoleID), Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.Message.ToString();
                ErrorLogging.LogError(ex);
                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", UserRoleID), Configuration.Formatters.JsonFormatter);
            }
        }
        // DELETE api/RoleDetail/5
        //http://swapstff.com/Items/DeleteItem/1
        [Route("DeleteUserRole/{UserRoleId}")]
        [HttpGet]
        public HttpResponseMessage DeleteUserRole(int UserRoleId)
        {
            try
            {
                var Userrole = _UserRoleservice.GetUserRole(UserRoleId);
                _UserRoleservice.DeleteUserRole(Userrole);
                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("success", UserRoleId), Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.Message.ToString();
                ErrorLogging.LogError(ex);
                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", UserRoleId), Configuration.Formatters.JsonFormatter);
            }
        }
    }
}
