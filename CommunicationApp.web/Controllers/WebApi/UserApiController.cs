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
    [RoutePrefix("User")]
    public class UserApiController : ApiController
    {
        public IUserService _UserService { get; set; }
        public ICustomerService _CustomerService { get; set; }
        public UserApiController(IUserService UserService, ICustomerService CustomerServer)
        {
            this._UserService = UserService;
            this._CustomerService = CustomerServer;
        }

        [Route("GetUsers")]
        [HttpGet]
        public IHttpActionResult GetAllUsers()
        {
            var users = _UserService.GetUsers();
            var models = new List<UserModel>();
            Mapper.CreateMap<CommunicationApp.Entity.User, CommunicationApp.Models.UserModel>();
            foreach (var user in users)
            {
                models.Add(Mapper.Map<CommunicationApp.Entity.User, CommunicationApp.Models.UserModel>(user));
            }

            return Json(models);
        }

        
        
       
    }
}