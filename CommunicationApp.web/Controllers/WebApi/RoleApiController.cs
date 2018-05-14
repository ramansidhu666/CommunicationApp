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
     [RoutePrefix("Roles")]
    public class RoleApiController : ApiController
    {
        public IRoleService _RoleService { get; set; }
        public RoleApiController(  IRoleService RoleService)
        {
           this._RoleService = RoleService;
        }

        // GET api/Driver/GetDrivers
        [Route("GetAllRoles")]
        [HttpGet]
        public IHttpActionResult GetAllRoles()
        {
            var Roles = _RoleService.GetRoles();
            var models = new List<RoleModel>();

            Mapper.CreateMap<CommunicationApp.Entity.Role, CommunicationApp.Models.RoleModel>();
            foreach (var role in Roles)
            {
                models.Add(Mapper.Map<CommunicationApp.Entity.Role, CommunicationApp.Models.RoleModel>(role));
            }


            return Json(models);


        }

        // GET api/Roles
        //http://swapstff.com/CustomerId/GetCustomerByID/1
        [Route("GetRoleByID/{RoleId}")]
        [HttpGet]
        public IHttpActionResult GetRoleByID(int RoleId)
        {
            var role = _RoleService.GetRole(RoleId);
            Mapper.CreateMap<CommunicationApp.Entity.Role, CommunicationApp.Models.RoleModel>();
            CommunicationApp.Models.RoleModel rolemodel = Mapper.Map<CommunicationApp.Entity.Role, CommunicationApp.Models.RoleModel>(role);
            return Json(rolemodel);
        }

        [Route("SaveRole")]
        [HttpPost]
        public HttpResponseMessage SaveRole([FromBody]RoleModel Rolemodel)
        {
            string RoleID = "-1";

            try
            {
                Mapper.CreateMap<CommunicationApp.Models.RoleModel, CommunicationApp.Entity.Role>();
                CommunicationApp.Entity.Role role = Mapper.Map<CommunicationApp.Models.RoleModel, CommunicationApp.Entity.Role>(Rolemodel);

                if (role.RoleId <= 0) //new 
                {


                    //Insert the city
                    _RoleService.InsertRole(role); //Save Operation
                    //End : Insert the Customer
                }
                else
                {
                    _RoleService.UpdateRole(role); //Update Operation

                }
                RoleID = role.RoleId.ToString();

                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("success", RoleID), Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.Message.ToString();
                ErrorLogging.LogError(ex);
                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", RoleID), Configuration.Formatters.JsonFormatter);
            }

        }

        // DELETE api/State/5
        //http://swapstff.com/Items/DeleteItem/1
        [Route("DeleteRole/{RoleId}")]
        [HttpGet]
        public HttpResponseMessage DeleteRole(int RoleId)
        {
            try
            {
                var role = _RoleService.GetRole(RoleId);
                _RoleService.DeleteRole(role);
                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("success", RoleId), Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.Message.ToString();
                ErrorLogging.LogError(ex);
                return Request.CreateResponse(HttpStatusCode.NotImplemented, CommonCls.CreateMessage("error", RoleId), Configuration.Formatters.JsonFormatter);
            }
        }
    }

}
