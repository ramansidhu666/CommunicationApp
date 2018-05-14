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
    [RoutePrefix("Forms")]
    public class FormApiController : ApiController
    {
        public IFormService _formservice { get; set; }

        public FormApiController(IFormService formservice)
        {
            this._formservice = formservice;
        }

        //Get api/Form/forms
        [Route("GetAllForm")]
        [HttpGet]
        public IHttpActionResult GetAllForm()
        {
            var Form = _formservice.GetForms();
            var models = new List<FormModel>();
            Mapper.CreateMap<CommunicationApp.Entity.Form, CommunicationApp.Models.FormModel>();
            foreach (var form in Form)
            {
                models.Add(Mapper.Map<CommunicationApp.Entity.Form, CommunicationApp.Models.FormModel>(form));

            }

            return Json(models);
        }


        // GET api/Form/GetFormById
        //http://swapstff.com/CustomerId/GetCustomerByID/1
        [Route("GetFormByID/{FormId}")]
        [HttpGet]
        public IHttpActionResult GetStateByID(int FormId)
        {
            var form = _formservice.GetForm(FormId);
            Mapper.CreateMap<CommunicationApp.Entity.Country, CommunicationApp.Models.CountryModel>();
            CommunicationApp.Models.FormModel formmodel = Mapper.Map<CommunicationApp.Entity.Form, CommunicationApp.Models.FormModel>(form);
            return Json(formmodel);
        }

        [Route("SaveForm")]
        [HttpPost]
        public HttpResponseMessage SaveForm([FromBody]FormModel formmodel)
        {
            string FormID = "-1";

            try
            {
                Mapper.CreateMap<CommunicationApp.Models.FormModel, CommunicationApp.Entity.Form>();
                CommunicationApp.Entity.Form form = Mapper.Map<CommunicationApp.Models.FormModel, CommunicationApp.Entity.Form>(formmodel);

                if (form.FormId <= 0) //new 
                {

                    //Insert the Country
                    _formservice.InsertForm(form); //Save Operation
                    //End : Insert the Customer
                }
                else
                {
                    _formservice.UpdateForm(form); //Update Operation

                }
                FormID = form.FormId.ToString();

                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("success", FormID), Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.Message.ToString();
                ErrorLogging.LogError(ex);
                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", FormID), Configuration.Formatters.JsonFormatter);
            }
        }
        // DELETE api/Form/5
        //http://swapstff.com/Items/DeleteItem/1
        [Route("DeleteForm/{FormId}")]
        [HttpGet]
        public HttpResponseMessage DeleteForm(int FormId)
        {
            try
            {
                var form = _formservice.GetForm(FormId);
                _formservice.DeleteForm(form);
                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("success", FormId), Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.Message.ToString();
                ErrorLogging.LogError(ex);
                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", FormId), Configuration.Formatters.JsonFormatter);
            }
        }

    }
}
