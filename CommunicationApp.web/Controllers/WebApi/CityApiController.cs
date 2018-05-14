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

namespace CommunicationApp.Controllers.WebApi
{
    [RoutePrefix("City")]
    public class CityApiController : ApiController
    {
        public ICityService _cityservice { get; set; }

        public CityApiController(ICityService cityservice)
        {
            this._cityservice = cityservice;
        }

        //Get api/City/Cities
        [Route("GetAllCities")]
        [HttpGet]
        public IHttpActionResult GetAllCities()
        {
            var cities = _cityservice.GetCities();
            var models = new List<CityModel>();
            Mapper.CreateMap<CommunicationApp.Entity.City, CommunicationApp.Models.CityModel>();
            foreach (var city in cities)
            {
                models.Add(Mapper.Map<CommunicationApp.Entity.City, CommunicationApp.Models.CityModel>(city));
            }

            return Json(models);
        }

    
        // GET api/City/GetCityById
        //http://swapstff.com/CustomerId/GetCustomerByID/1
        [Route("GetCityByID/{CityId}")]
        [HttpGet]
        public IHttpActionResult GetCityByID(int CityId)
        {
            var city = _cityservice.GetCity(CityId);
            Mapper.CreateMap<CommunicationApp.Entity.City, CommunicationApp.Models.CityModel>();
            CommunicationApp.Models.CityModel citymodel = Mapper.Map<CommunicationApp.Entity.City, CommunicationApp.Models.CityModel>(city);
            return Json(citymodel);
        }

        [Route("SaveCity")]
        [HttpPost]
        public HttpResponseMessage SaveCity([FromBody]CityModel citymodel)
        {
            string CityID = "-1";
        
            try
            {
                Mapper.CreateMap<CommunicationApp.Models.CityModel, CommunicationApp.Entity.City>();
                CommunicationApp.Entity.City city = Mapper.Map<CommunicationApp.Models.CityModel, CommunicationApp.Entity.City>(citymodel);

                if (citymodel.CityID<= 0) //new 
                {
                    
                        //Insert the city
                        _cityservice.InsertCity(city); //Save Operation
                        //End : Insert the Customer
                                   }
                else
                {
                        _cityservice.UpdateCity(city); //Update Operation
                    
                }
                CityID = city.CityID.ToString();

                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("success", CityID), Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.Message.ToString();
                ErrorLogging.LogError(ex);
                return Request.CreateResponse(HttpStatusCode.NotImplemented, CommonCls.CreateMessage("error", CityID), Configuration.Formatters.JsonFormatter);
            }
        }
        // DELETE api/City/5
        //http://swapstff.com/Items/DeleteItem/1
        [Route("DeleteCity/{CityId}")]
        [HttpGet]
        public HttpResponseMessage DeleteCity(int CityId)
        {
            try
            {
                var city = _cityservice.GetCity(CityId);
                _cityservice.DeleteCity(city);
                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("success", CityId), Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.Message.ToString();
                ErrorLogging.LogError(ex);
                return Request.CreateResponse(HttpStatusCode.NotImplemented, CommonCls.CreateMessage("error", CityId), Configuration.Formatters.JsonFormatter);
            }
        }


        
    }
}