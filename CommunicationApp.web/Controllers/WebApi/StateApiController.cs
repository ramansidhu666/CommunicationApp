using System;
using System.Web;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using CommunicationApp.Entity;
using CommunicationApp.Services;
using Newtonsoft.Json;
using CommunicationApp.Models;
using AutoMapper;
using CommunicationApp.Infrastructure;
using CommunicationApp.Core.UtilityManager;

namespace CommunicationApp.Controllers.WebApi
{
    [RoutePrefix("State")]
    public class StateApiController : ApiController
    {
        public IStateService _stateservice { get; set; }

        public StateApiController(IStateService stateservice)
        {
            this._stateservice = stateservice;
        }

        //Get api/State/States
        [Route("GetAllStates")]
        [HttpGet]
        public IHttpActionResult GetAllStates()
        {
            var states = _stateservice.GetStates();
            var models = new List<StateModel>();
            Mapper.CreateMap<CommunicationApp.Entity.State, CommunicationApp.Models.StateModel>();
            foreach (var state in states)
            {
                models.Add(Mapper.Map<CommunicationApp.Entity.State, CommunicationApp.Models.StateModel>(state));

            }

            return Json(models);
        }

        //Get api/State/States
        [Route("GetAddressFromLatLong")]
        [HttpPost]
        public HttpResponseMessage GetAddressFromLatLong()
        {
            var address= GoogleOperation.GetAddressFromLatLong(30.71343770, 76.69907860);
            return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("success", address), Configuration.Formatters.JsonFormatter);
        }
        //GetAddressFromLatLong


        // GET api/City/GetstateById
        //http://swapstff.com/CustomerId/GetCustomerByID/1
        [Route("GetStateByID/{StateId}")]
        [HttpGet]
        public IHttpActionResult GetStateByID(int StateId)
        {
            var state = _stateservice.GetState(StateId);
            Mapper.CreateMap<CommunicationApp.Entity.State, CommunicationApp.Models.StateModel>();
            CommunicationApp.Models.StateModel statemodel = Mapper.Map<CommunicationApp.Entity.State, CommunicationApp.Models.StateModel>(state);
            return Json(statemodel);
        }

        [Route("SaveState")]
        [HttpPost]
        public HttpResponseMessage SaveState([FromBody]StateModel statemodel)
        {
            string StateID = "-1";

            try
            {
                Mapper.CreateMap<CommunicationApp.Models.StateModel, CommunicationApp.Entity.State>();
                CommunicationApp.Entity.State state = Mapper.Map<CommunicationApp.Models.StateModel, CommunicationApp.Entity.State>(statemodel);

                if (state.StateID <= 0) //new 
                {


                    //Insert the city
                    _stateservice.InsertState(state); //Save Operation
                    //End : Insert the Customer
                }
                else
                {
                    _stateservice.UpdateState(state); //Update Operation

                }
                StateID = state.StateID.ToString();

                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("success", StateID), Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.Message.ToString();
                ErrorLogging.LogError(ex);
                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", StateID), Configuration.Formatters.JsonFormatter);
            }
        }
        // DELETE api/State/5
        //http://swapstff.com/Items/DeleteItem/1
        [Route("DeleteState/{StateId}")]
        [HttpGet]
        public HttpResponseMessage DeleteState(int StateId)
        {
            try
            {
                var state = _stateservice.GetState(StateId);
                _stateservice.DeleteState(state);
                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("success", StateId), Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.Message.ToString();
                ErrorLogging.LogError(ex);
                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", StateId), Configuration.Formatters.JsonFormatter);
            }
        }



    }
}