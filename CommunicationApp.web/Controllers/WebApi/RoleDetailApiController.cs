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
    [RoutePrefix("RoleDetails")]
    public class roledetailApiController : ApiController
    {
        public IRoleDetailService _roledetailservice { get; set; }

        public roledetailApiController(IRoleDetailService roledetailservice)
        {
            this._roledetailservice = roledetailservice;
        }

        //Get api/Form/forms
        [Route("GetAllRoleDetail")]
        [HttpGet]
        public IHttpActionResult GetAllRoleDetail()
        {
            var Roledetail = _roledetailservice.GetRoleDetails();
             var models = new List<RoleDetailModel>();
             Mapper.CreateMap<CommunicationApp.Entity.RoleDetail, CommunicationApp.Models.RoleDetailModel>();
             foreach (var roledetail in Roledetail)
            {
                models.Add(Mapper.Map<CommunicationApp.Entity.RoleDetail, CommunicationApp.Models.RoleDetailModel>(roledetail));
               
            }

            return Json(models);
        }

    
        // GET api/Form/GetFormById
        //http://swapstff.com/CustomerId/GetCustomerByID/1
        [Route("GetRoleDetailByID/{roleDetailId}")]
        [HttpGet]
        public IHttpActionResult GetRoleDetailByID(int RoleDetailId)
        {
            var roledetail = _roledetailservice.GetRoleDetail(RoleDetailId);
            Mapper.CreateMap<CommunicationApp.Entity.RoleDetail, CommunicationApp.Models.RoleDetailModel>();
            CommunicationApp.Models.RoleDetailModel roledetailmodel = Mapper.Map<CommunicationApp.Entity.RoleDetail, CommunicationApp.Models.RoleDetailModel>(roledetail);
            return Json(roledetailmodel);
        }

        [Route("SaveroleDetail")]
        [HttpPost]
        public HttpResponseMessage SaveroleDetail([FromBody]RoleDetailModel roledetailmodel)
        {
            string RoleDetailID = "-1";
        
            try
            {
                Mapper.CreateMap<CommunicationApp.Models.RoleDetailModel, CommunicationApp.Entity.RoleDetail>();
                CommunicationApp.Entity.RoleDetail roledetail = Mapper.Map<CommunicationApp.Models.RoleDetailModel, CommunicationApp.Entity.RoleDetail>(roledetailmodel);

                if (roledetail.RoleDetailID <= 0) //new 
                {

                        //Insert the Country
                    _roledetailservice.InsertRoleDetail(roledetail); //Save Operation
                        //End : Insert the Customer
                                   }
                else
                {
                        _roledetailservice.UpdateRoleDetail(roledetail); //Update Operation
                    
                }
               RoleDetailID = roledetail.FormId.ToString();

               return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("success", RoleDetailID), Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.Message.ToString();
                ErrorLogging.LogError(ex);
                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", RoleDetailID), Configuration.Formatters.JsonFormatter);
            }
        }
        // DELETE api/RoleDetail/5
        //http://swapstff.com/Items/DeleteItem/1
        [Route("DeleteroleDetail/{RoleDetailId}")]
        [HttpGet]
        public HttpResponseMessage DeleteRoleDetail(int RoleDetailId)
        {
            try
            {
                var roledetail = _roledetailservice.GetRoleDetail(RoleDetailId);
                _roledetailservice.DeleteRoleDetail(roledetail);
                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("success", RoleDetailId), Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.Message.ToString();
                ErrorLogging.LogError(ex);
                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", RoleDetailId), Configuration.Formatters.JsonFormatter);
            }
        }
    }
}
