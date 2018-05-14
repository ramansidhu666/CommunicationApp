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
     [RoutePrefix("Country")]
    public class CountryApiController : ApiController
    {
 public ICountryService _countryservice { get; set; }

 public CountryApiController(ICountryService countryservice)
        {
            this._countryservice = countryservice;
        }

        //Get api/Country/Country
        [Route("GetAllCountry")]
        [HttpGet]
        public IHttpActionResult GetAllCountry()
        {
            var Country = _countryservice.GetCountries();
             var models = new List<CountryModel>();
             Mapper.CreateMap<CommunicationApp.Entity.Country, CommunicationApp.Models.CountryModel>();
             foreach (var country in Country)
            {
                models.Add(Mapper.Map<CommunicationApp.Entity.Country, CommunicationApp.Models.CountryModel>(country));
               
            }

            return Json(models);
        }

    
        // GET api/City/GetstateById
        //http://swapstff.com/CustomerId/GetCustomerByID/1
        [Route("GetCountryByID/{CountryId}")]
        [HttpGet]
        public IHttpActionResult GetStateByID(int CountryId)
        {
            var country = _countryservice.GetCountry(CountryId);
            Mapper.CreateMap<CommunicationApp.Entity.Country, CommunicationApp.Models.CountryModel>();
            CommunicationApp.Models.CountryModel countrymodel = Mapper.Map<CommunicationApp.Entity.Country, CommunicationApp.Models.CountryModel>(country);
            return Json(countrymodel);
        }

        [Route("SaveCountry")]
        [HttpPost]
        public HttpResponseMessage SaveCountry([FromBody]CountryModel countrymodel)
        {
            string CountryID = "-1";
        
            try
            {
                Mapper.CreateMap<CommunicationApp.Models.CountryModel, CommunicationApp.Entity.Country>();
                CommunicationApp.Entity.Country country = Mapper.Map<CommunicationApp.Models.CountryModel, CommunicationApp.Entity.Country>(countrymodel);

                if (country.CountryID <= 0) //new 
                {

                        //Insert the Country
                    _countryservice.InsertCountry(country); //Save Operation
                        //End : Insert the Customer
                                   }
                else
                {
                        _countryservice.UpdateCountry(country); //Update Operation
                    
                }
                CountryID = country.CountryID.ToString();

                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("success", CountryID), Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.Message.ToString();
                ErrorLogging.LogError(ex);
                return Request.CreateResponse(HttpStatusCode.NotImplemented, CommonCls.CreateMessage("error", CountryID), Configuration.Formatters.JsonFormatter);
            }
        }
        // DELETE api/State/5
        //http://swapstff.com/Items/DeleteItem/1
        [Route("DeleteCountry/{CountryId}")]
        [HttpGet]
        public HttpResponseMessage DeleteState(int CountryId)
        {
            try
            {
                var country= _countryservice.GetCountry(CountryId);
                _countryservice.DeleteCountry(country);
                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("success", CountryId), Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.Message.ToString();
                ErrorLogging.LogError(ex);
                return Request.CreateResponse(HttpStatusCode.NotImplemented, CommonCls.CreateMessage("error", CountryId), Configuration.Formatters.JsonFormatter);
            }
        }
    }
}
