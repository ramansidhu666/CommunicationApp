using AutoMapper;
using CommunicationApp.Controllers;
using CommunicationApp.Core.UtilityManager;
using CommunicationApp.Infrastructure;
using CommunicationApp.Models;
using CommunicationApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CommunicationApp.Web.Controllers
{
    public class OfficeLocationController : BaseController
    {

        public IOfficeLocationService _OfficeLocationService { get; set; }
        public ICityService _CityService { get; set; }
        public IStateService _StateService { get; set; }
        public ICountryService _CountryService { get; set; }
        public ICompanyService _CompanyService { get; set; }
        public IAgentService _AgentService { get; set; }
        public IEventCustomerService _EventCustomerService { get; set; }
        public IEventService _EventService { get; set; }
        public ITipService _TipService { get; set; }
        public IPropertyImageService _PropertyImageService { get; set; }
        public IPropertyService _PropertyService { get; set; }


        public OfficeLocationController(IAgentService AgentService, IEventCustomerService EventCustomerService, IEventService EventService, ITipService TipService, IPropertyService PropertyService, IPropertyImageService PropertyImageService, IFeedBackService FeedBackService, ICompanyService CompanyService, ICountryService CountryService, IStateService StateService, ICityService CityService, IOfficeLocationService OfficeLocationService, ICustomerService CustomerService, IUserService UserService, IFormService FormService, IRoleDetailService RoleDetailService, IRoleService RoleService, IUserRoleService UserRoleService)
            : base(CustomerService, UserService, RoleService, FormService, RoleDetailService, UserRoleService)
        {
            this._CustomerService = CustomerService;
            this._UserService = UserService;
            this._UserRoleService = UserRoleService;
            this._OfficeLocationService = OfficeLocationService;
            this._CompanyService = CompanyService;
            this._CountryService = CountryService;
            this._StateService = StateService;
            this._CityService = CityService;

            this._AgentService = AgentService;
            this._EventCustomerService = EventCustomerService;
            this._EventService = EventService;
            this._CountryService = CountryService;
            this._TipService = TipService;
            this._PropertyService = PropertyService;
            this._PropertyImageService = PropertyImageService;


        }
        private void CheckPermission()
        {
            RoleDetailModel roleDetail = UserPermission("customer");
            TempData["Create"] = roleDetail.IsCreate;
            TempData["Delete"] = roleDetail.IsDelete;
            TempData["Detail"] = roleDetail.IsDetail;
            TempData["Edit"] = roleDetail.IsEdit;
            TempData["View"] = roleDetail.IsView;
        }
        //
        // GET: /OfficeLocation/
        public ActionResult Index(string Address, string PhoneNo, string Email)
        {

            UserPermissionAction("OfficeLocation", RoleAction.view.ToString());
            CheckPermission();
            List<OfficeLocationModel> OfficeLocationModelList = new List<OfficeLocationModel>();
            try
            {
                CustomerModel CustomerModel;
                var CompanyId = Convert.ToInt32(Session["CompanyID"]) == null ? 0 : Convert.ToInt32(Session["CompanyID"]);
                Mapper.CreateMap<CommunicationApp.Entity.OfficeLocation, CommunicationApp.Models.OfficeLocationModel>();
                var OfficeLocations = _OfficeLocationService.GetOfficeLocations().Where(c => c.CompanyId == CompanyId);
                //For Address
                if (!string.IsNullOrEmpty(Address))
                {
                    OfficeLocations = OfficeLocations.Where(c => c.OfficeAddress.ToLower().Contains(Address.ToLower()));
                }

                //Phone No
                if (!string.IsNullOrEmpty(PhoneNo))
                {
                    OfficeLocations = OfficeLocations.Where(c => c.TelephoneNo.ToLower().Contains(PhoneNo.ToLower()));
                }
                //For Email
                if (!string.IsNullOrEmpty(Email))
                {
                    OfficeLocations = OfficeLocations.Where(c => c.Email.ToLower().Contains(Email.ToLower()));
                }

                foreach (var OfficeLocation in OfficeLocations)
                {
                    CommunicationApp.Models.OfficeLocationModel OfficeLocationModel = Mapper.Map<CommunicationApp.Entity.OfficeLocation, CommunicationApp.Models.OfficeLocationModel>(OfficeLocation);
                    OfficeLocationModelList.Add(OfficeLocationModel);
                }
                return View(OfficeLocationModelList);
            }
            catch (Exception ex)
            {
                TempData["ShowMessage"] = "error";
                TempData["MessageBody"] = "";
                ErrorLogging.LogError(ex);
                return View();

            }
            return View(OfficeLocationModelList);

        }

        public ActionResult Create()
        {
            UserPermissionAction("OfficeLocation", RoleAction.create.ToString());
            CheckPermission();
            OfficeLocationModel officelocation = new CommunicationApp.Models.OfficeLocationModel();
            officelocation.CompanyId = Convert.ToInt32(Session["CompanyID"]);
            return View(officelocation);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Include = "OfficeLocationId,OfficeAddress,City,TelePhoneNo,Fax,Email,CompanyId")] OfficeLocationModel officeLocationModel)
        {
            UserPermissionAction("OfficeLocation", RoleAction.view.ToString());
            CheckPermission();
            TempData["ShowMessage"] = "";
            TempData["MessageBody"] = "";
            try
            {
                if (ModelState.IsValid)
                {
                    Mapper.CreateMap<CommunicationApp.Models.OfficeLocationModel, CommunicationApp.Entity.OfficeLocation>();
                    CommunicationApp.Entity.OfficeLocation OfficeLocation = Mapper.Map<CommunicationApp.Models.OfficeLocationModel, CommunicationApp.Entity.OfficeLocation>(officeLocationModel);
                    if (OfficeLocation != null)
                    {
                        if (OfficeLocation.CompanyId != 0)
                        {
                            OfficeLocation.CompanyId = Convert.ToInt32(Session["CompanyID"]) == null ? 0 : Convert.ToInt32(Session["CompanyID"]);
                            List<double> ListLatLong = GoogleOperation.GetLatLong(OfficeLocation.City);
                            OfficeLocation.Latitude = (decimal)(ListLatLong.Count > 0 ? ListLatLong[0] : 0);
                            OfficeLocation.Longitude = (decimal)(ListLatLong.Count > 0 ? ListLatLong[1] : 0);
                            _OfficeLocationService.InsertOfficeLocation(OfficeLocation);
                            TempData["ShowMessage"] = "success";
                            TempData["MessageBody"] = "Office Location successfully inserted";
                        }
                        else
                        {
                            TempData["ShowMessage"] = "error";
                            TempData["MessageBody"] = "Lacation not saved successfully.";
                        }

                        return RedirectToAction("index");


                    }

                }
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Key, x.Value.Errors }).ToArray();
                var modelStateErrors = this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors);
                return View(officeLocationModel);
            }
            catch (Exception ex)
            {
                TempData["ShowMessage"] = "error";
                TempData["MessageBody"] = ex.Message.ToString();
            }
            return View();
        }

        // GET: /Customer/Edit/5
        public ActionResult EditOfficeLocation(int id)
        {
            try
            {
                var OfficeLocation = _OfficeLocationService.GetOfficeLocation(id);
                Mapper.CreateMap<CommunicationApp.Entity.OfficeLocation, CommunicationApp.Models.OfficeLocationModel>();
                CommunicationApp.Models.OfficeLocationModel OfficeLocationModel = Mapper.Map<CommunicationApp.Entity.OfficeLocation, CommunicationApp.Models.OfficeLocationModel>(OfficeLocation);
                return View(OfficeLocationModel);
            }
            catch
            {

            }

            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditOfficeLocation([Bind(Include = "OfficeLocationId,OfficeAddress,City,TelePhoneNo,Fax,Email,Latitude,Longitude")] OfficeLocationModel officeLocationModel)
        {
            UserPermissionAction("customer", RoleAction.view.ToString());
            CheckPermission();
            TempData["ShowMessage"] = "";
            TempData["MessageBody"] = "";
            try
            {

                if (ModelState.IsValid)
                {

                    var OfficeLocationFound = _OfficeLocationService.GetOfficeLocations().Where(c => (c.OfficeAddress.Trim() == officeLocationModel.OfficeAddress.Trim()) && c.OfficeLocationId != officeLocationModel.OfficeLocationId).FirstOrDefault();
                    if (OfficeLocationFound == null)
                    {
                        var officeLocation = _OfficeLocationService.GetOfficeLocation(officeLocationModel.OfficeLocationId);
                        if (officeLocation != null)
                        {
                            officeLocation.OfficeAddress = officeLocationModel.OfficeAddress;
                            officeLocation.TelephoneNo = officeLocationModel.TelephoneNo;
                            officeLocation.Fax = officeLocationModel.Fax;
                            officeLocation.Email = officeLocationModel.Email;
                            officeLocation.Latitude = officeLocationModel.Latitude;
                            officeLocation.Longitude = officeLocationModel.Longitude;
                            officeLocation.City = officeLocationModel.City;
                            _OfficeLocationService.UpdateOfficeLocation(officeLocation);
                            TempData["ShowMessage"] = "success";
                            TempData["MessageBody"] = "Office Location successfully updated";
                            return RedirectToAction("index");
                        }
                    }
                    else
                    {
                        TempData["ShowMessage"] = "error";

                        if (OfficeLocationFound.Email.Trim() == officeLocationModel.Email.Trim())
                        {
                            TempData["MessageBody"] = officeLocationModel.Email + " is already exists.";
                        }
                        else if (OfficeLocationFound.Fax.Trim() == officeLocationModel.Fax.Trim())
                        {
                            TempData["MessageBody"] = officeLocationModel.Fax + " is already exists.";
                        }
                        else if (OfficeLocationFound.TelephoneNo.Trim() == officeLocationModel.TelephoneNo.Trim())
                        {
                            TempData["MessageBody"] = "This" + " " + officeLocationModel.TelephoneNo + " is already exists.";
                        }
                        else
                        {
                            TempData["MessageBody"] = "Please fill the required field with valid data";
                        }
                        return View(officeLocationModel);
                    }

                }

                return View(officeLocationModel);
            }
            catch (Exception ex)
            {
                CommonCls.ErrorLog(ex.ToString());
                TempData["ShowMessage"] = "error";
                TempData["MessageBody"] = ex.Message.ToString();

            }
            //var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Key, x.Value.Errors }).ToArray();
            //var modelStateErrors = this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors);


            return View();
        }


        // GET: /Customer/Edit/5
        public ActionResult DeleteOfficeLocation(int id)
        {
            try
            {
                var OfficeLocation = _OfficeLocationService.GetOfficeLocation(id);
                if (OfficeLocation != null)
                {
                    _OfficeLocationService.DeleteOfficeLocation(OfficeLocation);
                }
                return Redirect("index");
            }
            catch
            {

            }

            return View();
        }

    }
}