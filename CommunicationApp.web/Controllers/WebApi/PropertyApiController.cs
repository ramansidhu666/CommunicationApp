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
using AutoMapper;
using CommunicationApp.Models;
using CommunicationApp.Infrastructure;
using CommunicationApp.Core.Infrastructure;
using System.IO;
using System.Drawing;
using System.Configuration;
//using System.Data.Spatial;
using System.Spatial;
using CommunicationApp.Core.UtilityManager;
using System.Net.Mail;
using System.Web.Configuration;
using CommunicationApp.Web.Models;
using System.Text;
using CommunicationApp.Web.Infrastructure.PushNotificationFile;
using CommunicationApp.Web.Infrastructure.AsyncTask;

namespace CommunicationApp.Controllers.WebApi
{
    [RoutePrefix("Property")]
    public class PropertyApiController : ApiController
    {
        public IUserService _UserService { get; set; }
        public IUserRoleService _UserRoleService { get; set; }
        public IPropertyService _PropertyService { get; set; }
        public IPropertyImageService _PropertyImageService { get; set; }
        public IAgentService _AgentService { get; set; }
        public ICustomerService _CustomerService { get; set; }
        public IFeedBackService _FeedBackService { get; set; }
        public IEventService _EventService { get; set; }
        public INotification _Notification { get; set; }
        public ICompanyService _CompanyService { get; set; }
        public IViewsService _ViewsService { get; set; }
        //public i _EventService { get; set; }
        public PropertyApiController(INotification Notification, IEventService EventService, IFeedBackService FeedBackService, ICustomerService CustomerService, IAgentService AgentService, IPropertyImageService PropertyImageService, IPropertyService PropertyService, IUserService UserService, IUserRoleService UserRoleService, ICompanyService CompanyService, IViewsService ViewsService)
        {
            this._PropertyService = PropertyService;
            this._UserService = UserService;
            this._UserRoleService = UserRoleService;
            this._PropertyImageService = PropertyImageService;
            this._AgentService = AgentService;
            this._CustomerService = CustomerService;
            this._FeedBackService = FeedBackService;
            this._EventService = EventService;//Notification
            this._Notification = Notification;
            this._CompanyService = CompanyService;
            this._ViewsService = ViewsService;
        }

        public string ConvertDate(string dateSt)
        {
            string[] DateArr = dateSt.Split('/');
            //From : 0-MM,1-DD,2-YYYY HH:MM:ss
            //From : 0-DD,1-MM,2-YYYY HH:MM:ss
            return DateArr[1] + "/" + DateArr[0] + "/" + DateArr[2];
        }
        //PropertyModel PropertyModel;
        //Property Property;
        [Route("GetAllProperty")]
        [HttpGet]
        public HttpResponseMessage GetAllProperty([FromUri]string PropertyTypeStatus, string ParentId, string BrokerageListing)
        {
            var PropertyCount = 0;
            List<int?> PropertyStatusIdList = new List<int?>();
            try
            {
                if (PropertyTypeStatus == "" || PropertyTypeStatus == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Property Type Status cannot be blank."), Configuration.Formatters.JsonFormatter);
                }
                if (PropertyTypeStatus == EnumValue.PropertySatus.ExclusiveCommercial.ToString())
                {

                    PropertyStatusIdList.Add((int)EnumValue.PropertySatus.ExclusiveCommercial);

                }
                else if (PropertyTypeStatus == EnumValue.PropertySatus.ExclusiveResidential.ToString())
                {

                    PropertyStatusIdList.Add((int)EnumValue.PropertySatus.ExclusiveResidential);
                    PropertyStatusIdList.Add((int)EnumValue.PropertySatus.ExclusiveResidentialCondo);
                }
                else if (PropertyTypeStatus == EnumValue.PropertySatus.NewHotListingCommercial.ToString())
                {
                    PropertyStatusIdList.Add((int)EnumValue.PropertySatus.NewHotListingCommercial);
                }
                else if (PropertyTypeStatus == EnumValue.PropertySatus.NewHotListingResidential.ToString())
                {
                    PropertyStatusIdList.Add((int)EnumValue.PropertySatus.NewHotListingResidential);
                    PropertyStatusIdList.Add((int)EnumValue.PropertySatus.NewHotListingResidentialCondo);
                }
                else if (PropertyTypeStatus == EnumValue.PropertySatus.LookingForCommercial.ToString())
                {
                    PropertyStatusIdList.Add((int)EnumValue.PropertySatus.LookingForCommercial);
                }
                else if (PropertyTypeStatus == EnumValue.PropertySatus.LookingForResidential.ToString())
                {
                    PropertyStatusIdList.Add((int)EnumValue.PropertySatus.LookingForResidential);
                    PropertyStatusIdList.Add((int)EnumValue.PropertySatus.LookingForResidentialCondo);
                }
                else if (PropertyTypeStatus == EnumValue.PropertySatus.LookingForResidentialCondo.ToString())
                {
                    PropertyStatusIdList.Add((int)EnumValue.PropertySatus.LookingForResidentialCondo);
                }
                else if (PropertyTypeStatus == EnumValue.PropertySatus.ExclusiveResidentialCondo.ToString())
                {
                    PropertyStatusIdList.Add((int)EnumValue.PropertySatus.ExclusiveResidentialCondo);
                }
                else if (PropertyTypeStatus == EnumValue.PropertySatus.NewHotListingResidentialCondo.ToString())
                {
                    PropertyStatusIdList.Add((int)EnumValue.PropertySatus.NewHotListingResidentialCondo);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Property Type Status is incorrect."), Configuration.Formatters.JsonFormatter);
                }



                if (PropertyStatusIdList != null)
                {
                    var Parentid = 0;
                    var Properties = _PropertyService.GetPropertys();
                    if (ParentId != null && ParentId != "")
                    {
                        Parentid = Convert.ToInt32(ParentId);
                    }
                    List<int> CustomerIds = _CustomerService.GetCustomers().Where(c => c.ParentId == Parentid).ToList().Select(c => c.CustomerId).ToList();

                    if (BrokerageListing == "My")
                    {
                        Properties = _PropertyService.GetPropertys().Where(c => PropertyStatusIdList.Contains(c.PropertyStatusId) && CustomerIds.Contains(Convert.ToInt32(c.CustomerId))).OrderByDescending(c => c.PropertyId).ToList();

                    }
                    else
                    {
                        Properties = _PropertyService.GetPropertys().Where(c => PropertyStatusIdList.Contains(c.PropertyStatusId) && !CustomerIds.Contains(Convert.ToInt32(c.CustomerId))).OrderByDescending(c => c.PropertyId).ToList();
                    }
                   // PropertyCount = Properties.Count();

                    var models = new List<PropertyModel>();
                    Mapper.CreateMap<CommunicationApp.Entity.Property, CommunicationApp.Web.Models.PropertyModel>();
                    foreach (var Property in Properties)
                    {
                        if (CheckExpireDate(Property))
                        {
                            PropertyCount += 1;
                            PropertyModel PropertyModel = new Web.Models.PropertyModel();
                            var Propertyimages = _PropertyImageService.GetPropertyImages().Where(c => c.PropertyId == Property.PropertyId).OrderByDescending(c => c.PropertyId).ToList();

                            var _Model = Mapper.Map<CommunicationApp.Entity.Property, CommunicationApp.Web.Models.PropertyModel>(Property);
                            var Customer = _CustomerService.GetCustomer(Convert.ToInt32(Property.CustomerId));
                            if (Customer != null)
                            {
                                _Model.CustomerName = Customer.FirstName + " " + Customer.MiddleName + " " + Customer.LastName;
                                if (Customer.IsActive == true && Customer.UpdateStatus == true)
                                {
                                    _Model.CustomerPhoto = Customer.PhotoPath;
                                }
                                else
                                {
                                    _Model.CustomerPhoto = ConfigurationManager.AppSettings["LiveURL"].ToString() + "/images/noImage.jpg";
                                }

                                _Model.CustomerEmail = Customer.EmailId;
                                _Model.CustomerPhoneNo = Customer.MobileNo;
                                _Model.WebsiteUrl = Customer.WebsiteUrl;
                                if (Customer.ParentId != 0)
                                {
                                    var Admin = _CustomerService.GetCustomer(Convert.ToInt32(Customer.ParentId));
                                    if (Admin != null)
                                    {
                                        _Model.AdminName = Admin.FirstName;
                                        _Model.AdminPhoto = Admin.PhotoPath;
                                        _Model.AdminWebSiteUrl = Admin.WebsiteUrl;
                                        _Model.AdminPhoneNo = Admin.MobileNo;
                                        _Model.AdminEmail = Admin.EmailId;
                                        var Company = _CompanyService.GetCompany(Admin.CompanyID);
                                        if (Company != null)
                                        {
                                            _Model.AdminCompanyLogo = Company.LogoPath;
                                            _Model.AdminCompanyAddress = Company.CompanyAddress;
                                            _Model.AdminCompanyName = Company.CompanyName;
                                        }
                                    }
                                }

                            }
                            if (Property.PropertyForStatusId != null)
                            {
                                if (Property.PropertyForStatusId == (int)EnumValue.PropertyForSatus.Lease)
                                {
                                    _Model.PropertyFor = EnumValue.GetEnumDescription(EnumValue.PropertyForSatus.Lease);
                                }
                                else if (Property.PropertyForStatusId == (int)EnumValue.PropertyForSatus.SubLease)
                                {
                                    _Model.PropertyFor = EnumValue.GetEnumDescription(EnumValue.PropertyForSatus.SubLease);
                                }
                                else if (Property.PropertyForStatusId == (int)EnumValue.PropertyForSatus.Sale)
                                {
                                    _Model.PropertyFor = EnumValue.GetEnumDescription(EnumValue.PropertyForSatus.Sale);
                                }
                            }

                            if (Property.SaleOfBusinessId != null)
                            {
                                if (Property.SaleOfBusinessId == (int)EnumValue.SaleOfBusinessSatus.WithProperty)
                                {
                                    _Model.SaleOfBusinessType = EnumValue.GetEnumDescription(EnumValue.SaleOfBusinessSatus.WithProperty);
                                }
                                else if (Property.SaleOfBusinessId == (int)EnumValue.SaleOfBusinessSatus.WithoutProperty)
                                {
                                    _Model.SaleOfBusinessType = EnumValue.GetEnumDescription(EnumValue.SaleOfBusinessSatus.WithoutProperty);
                                }
                                else if (Property.SaleOfBusinessId == (int)EnumValue.SaleOfBusinessSatus.Land)
                                {
                                    _Model.SaleOfBusinessType = EnumValue.GetEnumDescription(EnumValue.SaleOfBusinessSatus.Land);
                                }

                            }
                            // passing 'PropertyStatusId' to return model according to PropertyType.
                            if (Property.PropertyStatusId != null)
                            {
                                if (Property.PropertyStatusId == (int)EnumValue.PropertySatus.ExclusiveCommercial)
                                {
                                    _Model.TypeOfProperty = EnumValue.GetEnumDescription(EnumValue.PropertySatus.ExclusiveCommercial);
                                }
                                else if (Property.PropertyStatusId == (int)EnumValue.PropertySatus.ExclusiveResidential)
                                {
                                    _Model.TypeOfProperty = EnumValue.GetEnumDescription(EnumValue.PropertySatus.ExclusiveResidential);
                                }
                                else if (Property.PropertyStatusId == (int)EnumValue.PropertySatus.ExclusiveResidentialCondo)
                                {
                                    _Model.TypeOfProperty = EnumValue.GetEnumDescription(EnumValue.PropertySatus.ExclusiveResidentialCondo);
                                }
                                else if (Property.PropertyStatusId == (int)EnumValue.PropertySatus.LookingForCommercial)
                                {
                                    _Model.TypeOfProperty = EnumValue.GetEnumDescription(EnumValue.PropertySatus.LookingForCommercial);
                                }
                                else if (Property.PropertyStatusId == (int)EnumValue.PropertySatus.LookingForResidential)
                                {
                                    _Model.TypeOfProperty = EnumValue.GetEnumDescription(EnumValue.PropertySatus.LookingForResidential);
                                }
                                else if (Property.PropertyStatusId == (int)EnumValue.PropertySatus.LookingForResidentialCondo)
                                {
                                    _Model.TypeOfProperty = EnumValue.GetEnumDescription(EnumValue.PropertySatus.LookingForResidentialCondo);
                                }
                                else if (Property.PropertyStatusId == (int)EnumValue.PropertySatus.NewHotListingCommercial)
                                {
                                    _Model.TypeOfProperty = EnumValue.GetEnumDescription(EnumValue.PropertySatus.NewHotListingCommercial);
                                }
                                else if (Property.PropertyStatusId == (int)EnumValue.PropertySatus.NewHotListingResidential)
                                {
                                    _Model.TypeOfProperty = EnumValue.GetEnumDescription(EnumValue.PropertySatus.NewHotListingResidential);
                                }
                                else if (Property.PropertyStatusId == (int)EnumValue.PropertySatus.NewHotListingResidentialCondo)
                                {
                                    _Model.TypeOfProperty = EnumValue.GetEnumDescription(EnumValue.PropertySatus.NewHotListingResidentialCondo);
                                }
                            }

                            List<PropertyImages> PropertyImagesList = new List<PropertyImages>();
                            foreach (var Image in Propertyimages)
                            {
                                //PropertyModel.PropertyImage = Image.ImagePath;
                                PropertyImages PropertyImages = new Web.Models.PropertyImages();
                                if (Property.IsActive == true)
                                {
                                    PropertyImages.imagelist = Image.ImagePath;
                                }
                                else
                                {
                                    PropertyImages.imagelist = ConfigurationManager.AppSettings["LiveURL"].ToString() + "/images/noImage.jpg";
                                    // _Model.Remark = "";
                                }

                                PropertyImagesList.Add(PropertyImages);
                            }

                            _Model.PropertyPhotolist = PropertyImagesList;
                            var PropertyViewCount = _ViewsService.GetViewss().Where(s => s.PropertyId == _Model.PropertyId).Count();
                            if (PropertyViewCount != 0)
                            {
                                _Model.propertyViewsCount = PropertyViewCount;
                            }
                            _Model.TotalPropertyCount = PropertyCount;
                            models.Add(_Model);
                        }

                    }

                    return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("success", models), Configuration.Formatters.JsonFormatter);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "No record found"), Configuration.Formatters.JsonFormatter);

                }

            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Please try later."), Configuration.Formatters.JsonFormatter);

            }

        }

        [Route("GetAllAgents")]
        [HttpGet]
        public HttpResponseMessage GetAllAgents(string PropertyTypeStatus ,int parentId)
        {
            int PropertyStatusId = 0;
            try
            {
                if (PropertyTypeStatus == "" || PropertyTypeStatus == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Agent Type Status cannot be blank."), Configuration.Formatters.JsonFormatter);
                }
                if (PropertyTypeStatus == EnumValue.AgentSatus.AgentAvailable.ToString())
                {
                    PropertyStatusId = (int)EnumValue.AgentSatus.AgentAvailable;
                }
                else if (PropertyTypeStatus == EnumValue.AgentSatus.AgentRequired.ToString())
                {
                    PropertyStatusId = (int)EnumValue.AgentSatus.AgentRequired;
                }
                var Properties = _AgentService.GetAgents().Where(c => c.AgentStatusId == PropertyStatusId && c.ParentId==parentId).OrderByDescending(x => x.AgentId).ToList();
                var models = new List<AgentModel>();
                Mapper.CreateMap<CommunicationApp.Entity.Agent, CommunicationApp.Models.AgentModel>();
                foreach (var OpenHouse in Properties)
                {
                    var Agentimages = _PropertyImageService.GetPropertyImages().Where(c => c.AgentId == OpenHouse.AgentId).OrderByDescending(c => c.AgentId);

                    var _Model = Mapper.Map<CommunicationApp.Entity.Agent, CommunicationApp.Models.AgentModel>(OpenHouse);
                    var customerDetail = _CustomerService.GetCustomer(Convert.ToInt32(OpenHouse.CustomerId));
                    if (customerDetail != null)
                    {
                        _Model.CustomerName = customerDetail.FirstName + " " + customerDetail.MiddleName + " " + customerDetail.LastName;
                        if (customerDetail.IsActive == true && customerDetail.UpdateStatus == true)
                        {
                            _Model.CustomerPhoto = customerDetail.PhotoPath;
                        }
                        else
                        {
                            _Model.CustomerPhoto = ConfigurationManager.AppSettings["LiveURL"].ToString() + "/images/noImage.jpg";
                        }
                        
                        _Model.CustomerEmail = customerDetail.EmailId;
                        _Model.CustomerMobileNo = customerDetail.MobileNo;
                        _Model.WebsiteUrl = customerDetail.WebsiteUrl;
                        _Model.AgentTypeStatus = PropertyTypeStatus;
                        if (customerDetail.ParentId != 0)
                        {
                            var Admin = _CustomerService.GetCustomer(Convert.ToInt32(customerDetail.ParentId));
                            if (Admin != null)
                            {
                                _Model.AdminName = Admin.FirstName;
                                _Model.AdminPhoto = Admin.PhotoPath;
                                _Model.AdminWebSiteUrl = Admin.WebsiteUrl;
                                _Model.AdminPhoneNo = Admin.MobileNo;
                            }
                        }
                        List<PropertyImages> PropertyImagesList = new List<PropertyImages>();
                        foreach (var Image in Agentimages)
                        {
                            PropertyImages PropertyImages = new Web.Models.PropertyImages();
                            //PropertyModel.PropertyImage = Image.ImagePath;
                            PropertyImages.imagelist = Image.ImagePath;

                            if (OpenHouse.IsActive == true)
                            {
                                PropertyImages.imagelist = Image.ImagePath;

                            }
                            else
                            {
                                if (PropertyTypeStatus == EnumValue.AgentSatus.AgentAvailable.ToString())
                                {
                                    PropertyImages.imagelist = Image.ImagePath;
                                    _Model.Comments = "";
                                }
                                else if (PropertyTypeStatus == EnumValue.AgentSatus.AgentRequired.ToString())
                                {
                                    PropertyImages.imagelist = ConfigurationManager.AppSettings["LiveURL"].ToString() + "/images/noImage.jpg"; ;
                                    _Model.Comments = "";
                                }

                            }
                            PropertyImagesList.Add(PropertyImages);
                        }

                        if (OpenHouse.IsActive == true)
                        {
                        }
                        else
                        {
                            if (PropertyTypeStatus == EnumValue.AgentSatus.AgentAvailable.ToString())
                            {

                                // _Model.Comments = "";
                            }
                            else if (PropertyTypeStatus == EnumValue.AgentSatus.AgentRequired.ToString())
                            {
                                //_Model.CustomerPhoto = ConfigurationManager.AppSettings["LiveURL"].ToString() + "images/noImage.jpg"; ;
                                //_Model.Comments = "";
                            }

                        }

                        _Model.PropertyPhotolist = PropertyImagesList;
                        models.Add(_Model);
                    }
                }
                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("success", models), Configuration.Formatters.JsonFormatter);
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Please try later."), Configuration.Formatters.JsonFormatter);
            }
        }

        [Route("GetPropertyByID")]
        [HttpGet]
        public HttpResponseMessage GetPropertyByID([FromUri] int PropertyId)
        {
            try
            {
                if (PropertyId != 0)
                {
                    var Property = _PropertyService.GetProperty(PropertyId);
                    Mapper.CreateMap<CommunicationApp.Entity.Property, CommunicationApp.Web.Models.PropertyModel>();
                    CommunicationApp.Web.Models.PropertyModel PropertyModel = Mapper.Map<CommunicationApp.Entity.Property, CommunicationApp.Web.Models.PropertyModel>(Property);
                    var PropertyImages = _PropertyImageService.GetPropertyImages().Where(c => c.PropertyId == PropertyId);
                    List<PropertyImages> PropertyImageList = new List<PropertyImages>();
                    foreach (var propertyImage in PropertyImages)
                    {
                        PropertyImages PropertyImage = new PropertyImages();
                        PropertyImage.imagelist = propertyImage.ImagePath;
                        PropertyImageList.Add(PropertyImage);
                    }

                    PropertyModel.PropertyPhotolist = PropertyImageList;
                    var Customer = _CustomerService.GetCustomer(Convert.ToInt32(Property.CustomerId));
                    if (Customer != null)
                    {
                        PropertyModel.CustomerName = Customer.FirstName + " " + Customer.MiddleName + " " + Customer.LastName;
                        PropertyModel.CustomerPhoto = Customer.PhotoPath;
                        PropertyModel.CustomerEmail = Customer.EmailId;
                        PropertyModel.CustomerPhoneNo = Customer.MobileNo;
                        if (Customer.ParentId != 0)
                        {
                            var Admin = _CustomerService.GetCustomer(Convert.ToInt32(Customer.ParentId));
                            if (Admin != null)
                            {
                                PropertyModel.AdminName = Admin.FirstName;
                                PropertyModel.AdminPhoto = Admin.PhotoPath;
                                PropertyModel.AdminWebSiteUrl = Admin.WebsiteUrl;
                                PropertyModel.AdminPhoneNo = Admin.MobileNo;
                            }
                        }
                    }


                    return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("success", PropertyModel), Configuration.Formatters.JsonFormatter);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Property id null."), Configuration.Formatters.JsonFormatter);

                }

            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.Message.ToString();
                ErrorLogging.LogError(ex);
                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Property not found."), Configuration.Formatters.JsonFormatter);
            }
        }

        [Route("SavePropertyViews")]
        [HttpGet]
        public HttpResponseMessage SavePropertyViews([FromUri] int PropertyId, int CustomerId)
        {
            try
            {
                if (PropertyId != 0 && CustomerId != 0)
                {
                    var IsSelfProperty = _PropertyService.GetPropertys().Where(c => c.PropertyId == PropertyId && c.CustomerId == CustomerId).ToList();
                    if (IsSelfProperty.Count == 0)
                    {
                        var IsAlreadyView = _ViewsService.GetViewss().Where(s => s.PropertyId == PropertyId && s.CustomerId == CustomerId).ToList();
                        if (IsAlreadyView.Count == 0)
                        {
                            Views Views = new Views();
                            Views.PropertyId = PropertyId;
                            Views.CustomerId = CustomerId;
                            Views.Status = true;
                            _ViewsService.InsertViews(Views);
                            return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("success", "View successfully saved."), Configuration.Formatters.JsonFormatter);
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("unsuccess", "Already view this property."), Configuration.Formatters.JsonFormatter);
                        }

                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("unsuccess", "Its your property."), Configuration.Formatters.JsonFormatter);
                    }
                }

            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.Message.ToString();
                ErrorLogging.LogError(ex);
                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Property not found."), Configuration.Formatters.JsonFormatter);
            }

            return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("errors", ""), Configuration.Formatters.JsonFormatter);


        }

        [Route("SaveProperty")]
        [HttpPost]
        public HttpResponseMessage SaveProperty([FromBody]PropertyModel PropertyModel)
        {

            try
            {
                int PropertyId = 0;

                int Flag = 1;
                string Message = "";
                string PropertyType = "";

                //for set model default value.
                PropertyModel.PropertyId = PropertyModel.PropertyId == null ? 0 : PropertyModel.PropertyId;
                PropertyModel.CustomerId = PropertyModel.CustomerId == null ? 0 : PropertyModel.CustomerId;
                PropertyModel.CustomerName = PropertyModel.CustomerName == null ? "" : PropertyModel.CustomerName;
                PropertyModel.CustomerTrebId = PropertyModel.CustomerTrebId == null ? "" : PropertyModel.CustomerTrebId;
                PropertyModel.CustomerPhoto = PropertyModel.CustomerPhoto == null ? "" : PropertyModel.CustomerPhoto;
                PropertyModel.CustomerEmail = PropertyModel.CustomerEmail == null ? "" : PropertyModel.CustomerEmail;
                PropertyModel.CustomerPhoneNo = PropertyModel.CustomerPhoneNo == null ? "" : PropertyModel.CustomerPhoneNo;
                PropertyModel.Kitchen = PropertyModel.Kitchen == null ? "" : PropertyModel.Kitchen;
                PropertyModel.SideDoorEntrance = PropertyModel.SideDoorEntrance == null ? "" : PropertyModel.SideDoorEntrance;
                PropertyModel.PropertyStatusId = PropertyModel.PropertyStatusId == null ? 0 : PropertyModel.PropertyStatusId;
                PropertyModel.MLS = PropertyModel.MLS == null ? "" : PropertyModel.MLS;
                PropertyModel.Price = PropertyModel.Price == null ? "" : PropertyModel.Price;
                PropertyModel.MininumPrice = PropertyModel.MininumPrice == null ? "" : PropertyModel.MininumPrice;
                PropertyModel.MaximumPrice = PropertyModel.MaximumPrice == null ? "" : PropertyModel.MaximumPrice;
                PropertyModel.LocationPrefered = PropertyModel.LocationPrefered == null ? "" : PropertyModel.LocationPrefered;
                PropertyModel.Style = PropertyModel.Style == null ? "" : PropertyModel.Style;
                PropertyModel.Age = PropertyModel.Age == null ? "" : PropertyModel.Age;
                PropertyModel.Garage = PropertyModel.Garage == null ? "" : PropertyModel.Garage;
                PropertyModel.Bedrooms = PropertyModel.Bedrooms == null ? "" : PropertyModel.Bedrooms;
                PropertyModel.Bathrooms = PropertyModel.Bathrooms == null ? "" : PropertyModel.Bathrooms;
                PropertyModel.PropertyType = PropertyModel.PropertyType == null ? "" : PropertyModel.PropertyType;
                PropertyModel.Basement = PropertyModel.Basement == null ? "" : PropertyModel.Basement;
                PropertyModel.BasementValue = PropertyModel.BasementValue == null ? "" : PropertyModel.BasementValue;
                PropertyModel.Size = PropertyModel.Size == null ? "" : PropertyModel.Size;
                PropertyModel.Remark = PropertyModel.Remark == null ? "" : PropertyModel.Remark;
                PropertyModel.WebsiteUrl = PropertyModel.WebsiteUrl == null ? "" : PropertyModel.WebsiteUrl;
                PropertyModel.Type = PropertyModel.Type == null ? "" : PropertyModel.Type;
                PropertyModel.Balcony = PropertyModel.Balcony == null ? "" : PropertyModel.Balcony;
                PropertyModel.Alivator = PropertyModel.Alivator == null ? "" : PropertyModel.Alivator;
                PropertyModel.ParkingSpace = PropertyModel.ParkingSpace == null ? "" : PropertyModel.ParkingSpace;
                PropertyModel.GarageType = PropertyModel.GarageType == null ? "" : PropertyModel.GarageType;
                PropertyModel.PropertyTypeStatus = PropertyModel.PropertyTypeStatus == null ? "" : PropertyModel.PropertyTypeStatus;
                PropertyModel.TypeOfProperty = PropertyModel.TypeOfProperty == null ? "" : PropertyModel.TypeOfProperty;
                PropertyModel.PropertyFor = PropertyModel.PropertyFor == null ? "" : PropertyModel.PropertyFor;
                PropertyModel.SaleOfBusinessType = PropertyModel.SaleOfBusinessType == null ? "" : PropertyModel.SaleOfBusinessType;
                PropertyModel.PropertyImage = PropertyModel.PropertyImage == null ? "" : PropertyModel.PropertyImage;
                PropertyModel.Loundry = PropertyModel.Loundry == null ? "" : PropertyModel.Loundry;
                PropertyModel.Level = PropertyModel.Level == null ? "" : PropertyModel.Level;
                PropertyModel.ListPriceCode = PropertyModel.ListPriceCode == null ? "" : PropertyModel.ListPriceCode;
                PropertyModel.TypeTaxes = PropertyModel.TypeTaxes == null ? "" : PropertyModel.TypeTaxes;
                PropertyModel.TypeCommercial = PropertyModel.TypeCommercial == null ? "" : PropertyModel.TypeCommercial;
                PropertyModel.CategoryCommercial = PropertyModel.CategoryCommercial == null ? "" : PropertyModel.CategoryCommercial;
                PropertyModel.Use = PropertyModel.Use == null ? "" : PropertyModel.Use;
                PropertyModel.Zoning = PropertyModel.Zoning == null ? "" : PropertyModel.Zoning;
                PropertyModel.IsActive = PropertyModel.IsActive == null ? false : PropertyModel.IsActive;
                PropertyModel.TypeOfProperty = PropertyModel.TypeOfProperty == null ? "" : PropertyModel.TypeOfProperty;
                PropertyModel.Community = PropertyModel.Community == null ? "" : PropertyModel.Community;
                PropertyModel.CreatedOn = DateTime.Now;

                //End

                Mapper.CreateMap<CommunicationApp.Web.Models.PropertyModel, CommunicationApp.Entity.Property>();
                CommunicationApp.Entity.Property Property = Mapper.Map<CommunicationApp.Web.Models.PropertyModel, CommunicationApp.Entity.Property>(PropertyModel);
                if (PropertyModel.PropertyTypeStatus == "" || PropertyModel.PropertyTypeStatus == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Property Type Status cannot be blank."), Configuration.Formatters.JsonFormatter);
                }
                if (PropertyModel.PropertyTypeStatus == EnumValue.PropertySatus.ExclusiveCommercial.ToString())
                {
                    Property.PropertyStatusId = (int)EnumValue.PropertySatus.ExclusiveCommercial;
                    Flag = (int)EnumValue.PropertySatus.ExclusiveCommercial;
                    PropertyType = "Exclusive Listing Commercial";
                }
                else if (PropertyModel.PropertyTypeStatus == EnumValue.PropertySatus.ExclusiveResidential.ToString())
                {
                    Property.PropertyStatusId = (int)EnumValue.PropertySatus.ExclusiveResidential;
                    Flag = (int)EnumValue.PropertySatus.ExclusiveResidential;
                    PropertyType = "Exclusive Listing Residential";
                }
                else if (PropertyModel.PropertyTypeStatus == EnumValue.PropertySatus.NewHotListingCommercial.ToString())
                {
                    Property.PropertyStatusId = (int)EnumValue.PropertySatus.NewHotListingCommercial;
                    Flag = (int)EnumValue.PropertySatus.NewHotListingCommercial;
                    PropertyType = "New Hot Listing Commercial";
                }
                else if (PropertyModel.PropertyTypeStatus == EnumValue.PropertySatus.NewHotListingResidential.ToString())
                {
                    Property.PropertyStatusId = (int)EnumValue.PropertySatus.NewHotListingResidential;
                    Flag = (int)EnumValue.PropertySatus.NewHotListingResidential;
                    PropertyType = "New Hot Listing Residential";
                }
                else if (PropertyModel.PropertyTypeStatus == EnumValue.PropertySatus.LookingForCommercial.ToString())
                {
                    Property.PropertyStatusId = (int)EnumValue.PropertySatus.LookingForCommercial;
                    Flag = (int)EnumValue.PropertySatus.LookingForCommercial;
                    PropertyType = "Looking For Commercial";
                }
                else if (PropertyModel.PropertyTypeStatus == EnumValue.PropertySatus.LookingForResidential.ToString())
                {
                    Property.PropertyStatusId = (int)EnumValue.PropertySatus.LookingForResidential;
                    Flag = (int)EnumValue.PropertySatus.LookingForResidential;
                    PropertyType = "Looking For Residential";
                }
                else if (PropertyModel.PropertyTypeStatus == EnumValue.PropertySatus.ExclusiveResidentialCondo.ToString())
                {
                    Property.PropertyStatusId = (int)EnumValue.PropertySatus.ExclusiveResidentialCondo;
                    Flag = (int)EnumValue.PropertySatus.ExclusiveResidential;
                    PropertyType = "Exclusive Listing Residential Condo";
                }
                else if (PropertyModel.PropertyTypeStatus == EnumValue.PropertySatus.NewHotListingResidentialCondo.ToString())
                {
                    Property.PropertyStatusId = (int)EnumValue.PropertySatus.NewHotListingResidentialCondo;
                    Flag = (int)EnumValue.PropertySatus.NewHotListingResidential;
                    PropertyType = "New Hot Listing Residential Condo";
                }
                else if (PropertyModel.PropertyTypeStatus == EnumValue.PropertySatus.LookingForResidentialCondo.ToString())
                {
                    Property.PropertyStatusId = (int)EnumValue.PropertySatus.LookingForResidentialCondo;
                    Flag = (int)EnumValue.PropertySatus.LookingForResidential;
                    PropertyType = "Looking For Residential Condo";
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Property Type Status is incorrect."), Configuration.Formatters.JsonFormatter);
                    Flag = 0;
                    Message = "Property Type Status is incorrect.";
                }

                Message += "New property of " + PropertyType + " is available.";

                //Save PropertyFor Status Data
                if (PropertyModel.PropertyFor == EnumValue.GetEnumDescription(EnumValue.PropertyForSatus.Sale))
                {
                    Property.PropertyForStatusId = (int)EnumValue.PropertyForSatus.Sale;
                }
                else if (PropertyModel.PropertyFor == EnumValue.GetEnumDescription(EnumValue.PropertyForSatus.Lease))
                {
                    Property.PropertyForStatusId = (int)EnumValue.PropertyForSatus.Lease;
                }
                else if (PropertyModel.PropertyFor == EnumValue.GetEnumDescription(EnumValue.PropertyForSatus.SubLease))
                {
                    Property.PropertyForStatusId = (int)EnumValue.PropertyForSatus.SubLease;
                }


                //Save SaleOfBusiness Status Data
                if (PropertyModel.SaleOfBusinessType == EnumValue.GetEnumDescription(EnumValue.SaleOfBusinessSatus.WithProperty))
                {
                    Property.SaleOfBusinessId = (int)EnumValue.SaleOfBusinessSatus.WithProperty;
                }
                else if (PropertyModel.SaleOfBusinessType == EnumValue.GetEnumDescription(EnumValue.SaleOfBusinessSatus.WithoutProperty))
                {
                    Property.SaleOfBusinessId = (int)EnumValue.SaleOfBusinessSatus.WithoutProperty;
                }
                else if (PropertyModel.SaleOfBusinessType == EnumValue.GetEnumDescription(EnumValue.SaleOfBusinessSatus.Land))
                {
                    Property.SaleOfBusinessId = (int)EnumValue.SaleOfBusinessSatus.Land;
                }

                Property.IsActive = false;
                if (Property.SaleOfBusinessId == null)
                {
                    Property.SaleOfBusinessId = 0;
                }
                if (Property.PropertyForStatusId == null)
                {
                    Property.PropertyForStatusId = 0;
                }
                if (Property.PropertyStatusId == null)
                {
                    Property.PropertyStatusId = 0;
                }
                Property.IsPropertyUpdated = false;
                _PropertyService.InsertProperty(Property);
                
                //update customer table
                var customer = _CustomerService.GetCustomer(Convert.ToInt32(Property.CustomerId));
                if (customer!=null)
                {
                    customer.LastUpdatedOn = DateTime.Now;
                    customer.IsAvailable = true;
                    _CustomerService.UpdateCustomer(customer);
                }
                //
                PropertyId = Property.PropertyId;

                PropertyModel.Style = Property.Style;
                PropertyModel.PropertyTypeStatus = PropertyType;
                PropertyModel.PropertyType = Property.PropertyType;
                PropertyModel.LocationPrefered = Property.LocationPrefered;
                string body = "New property has registered by user and waiting for your approval.";
                ////Send Email to  admin for new property.
                string Subject = "New property registered.";

                // JobScheduler for email and notification.
                JobScheduler.SendEmailProperty(Property.PropertyId.ToString(), Subject, body, PropertyModel.PropertyTypeStatus);
                JobScheduler.SendNotificationProperty(Property.PropertyId.ToString(), Flag.ToString(), Message);
                ////End


                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("success", PropertyId), Configuration.Formatters.JsonFormatter);

            }
            catch (Exception ex)
            {

                string ErrorMsg = ex.Message.ToString();
                ErrorLogging.LogError(ex);
                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Please try later."), Configuration.Formatters.JsonFormatter);
            }
        }
        [Route("SearchProperty")]
        [HttpPost]
        public HttpResponseMessage SearchProperty([FromBody]PropertyModel PropertyModel)
        {

            try
            {
                Mapper.CreateMap<CommunicationApp.Web.Models.PropertyModel, CommunicationApp.Entity.Property>();
                CommunicationApp.Entity.Property Property = Mapper.Map<CommunicationApp.Web.Models.PropertyModel, CommunicationApp.Entity.Property>(PropertyModel);
                List<PropertyModel> models = new List<PropertyModel>();

                List<int?> PropertyStatusIdList = new List<int?>();
                if (PropertyModel.PropertyTypeStatus == EnumValue.PropertySatus.ExclusiveCommercial.ToString())
                {
                    PropertyStatusIdList.Add((int)EnumValue.PropertySatus.ExclusiveCommercial);
                }
                else if (PropertyModel.PropertyTypeStatus == EnumValue.PropertySatus.ExclusiveResidential.ToString())
                {
                    PropertyStatusIdList.Add((int)EnumValue.PropertySatus.ExclusiveResidential);
                }
                else if (PropertyModel.PropertyTypeStatus == EnumValue.PropertySatus.NewHotListingCommercial.ToString())
                {
                    PropertyStatusIdList.Add((int)EnumValue.PropertySatus.NewHotListingCommercial);
                }
                else if (PropertyModel.PropertyTypeStatus == EnumValue.PropertySatus.NewHotListingResidential.ToString())
                {
                    PropertyStatusIdList.Add((int)EnumValue.PropertySatus.NewHotListingResidential);
                }
                else if (PropertyModel.PropertyTypeStatus == EnumValue.PropertySatus.LookingForCommercial.ToString())
                {
                    PropertyStatusIdList.Add((int)EnumValue.PropertySatus.LookingForCommercial);
                }
                else if (PropertyModel.PropertyTypeStatus == EnumValue.PropertySatus.LookingForResidential.ToString())
                {
                    PropertyStatusIdList.Add((int)EnumValue.PropertySatus.LookingForResidential);
                }
                else if (PropertyModel.PropertyTypeStatus == EnumValue.PropertySatus.ExclusiveResidentialCondo.ToString())
                {
                    PropertyStatusIdList.Add((int)EnumValue.PropertySatus.ExclusiveResidentialCondo);
                }
                else if (PropertyModel.PropertyTypeStatus == EnumValue.PropertySatus.LookingForResidentialCondo.ToString())
                {
                    PropertyStatusIdList.Add((int)EnumValue.PropertySatus.LookingForResidentialCondo);
                }
                else if (PropertyModel.PropertyTypeStatus == EnumValue.PropertySatus.NewHotListingResidentialCondo.ToString())
                {
                    PropertyStatusIdList.Add((int)EnumValue.PropertySatus.NewHotListingResidentialCondo);
                }
                else if (PropertyModel.PropertyTypeStatus.ToLower().Trim() == "commercial")
                {
                    PropertyStatusIdList.Add((int)EnumValue.PropertySatus.ExclusiveCommercial);
                    PropertyStatusIdList.Add((int)EnumValue.PropertySatus.NewHotListingCommercial);
                }
                else if (PropertyModel.PropertyTypeStatus.ToLower().Trim() == "residential")
                {
                    PropertyStatusIdList.Add((int)EnumValue.PropertySatus.ExclusiveResidential);
                    PropertyStatusIdList.Add((int)EnumValue.PropertySatus.NewHotListingResidential);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "No record found."), Configuration.Formatters.JsonFormatter);
                }


                List<int> CustomerIds = _CustomerService.GetCustomers().Where(c => c.ParentId == PropertyModel.ParentId).ToList().Select(c => c.CustomerId).ToList();   

                var Propertylist = _PropertyService.GetPropertys().Where(x => PropertyStatusIdList.Contains(x.PropertyStatusId) && !CustomerIds.Contains(Convert.ToInt32(x.CustomerId))).OrderByDescending(c => c.PropertyId).ToList(); 
                //Search property by age.
                if (PropertyModel.Age != "" && PropertyModel.Age != null)
                {
                    Propertylist = Propertylist.Where(c => c.Age == PropertyModel.Age).ToList();
                }
                //Search propertyFor .
                if (PropertyModel.PropertyFor != "" && PropertyModel.PropertyFor != null)
                {
                    var StatusId = 0;

                    if (PropertyModel.PropertyFor == EnumValue.GetEnumDescription(EnumValue.PropertyForSatus.Sale))
                    {
                        StatusId = (int)EnumValue.PropertyForSatus.Sale;
                    }
                    else if (PropertyModel.PropertyFor == EnumValue.GetEnumDescription(EnumValue.PropertyForSatus.Lease))
                    {
                        StatusId = (int)EnumValue.PropertyForSatus.Lease;
                    }
                    else if (PropertyModel.PropertyFor == EnumValue.GetEnumDescription(EnumValue.PropertyForSatus.SubLease))
                    {
                        StatusId = (int)EnumValue.PropertyForSatus.SubLease;
                    }
                    Propertylist = Propertylist.Where(c => c.PropertyForStatusId == StatusId).ToList();
                }

                //Search saleOfBusiness .
                if (PropertyModel.SaleOfBusinessType != "" && PropertyModel.SaleOfBusinessType != null)
                {
                    var StatusId = 0;

                    if (PropertyModel.SaleOfBusinessType == EnumValue.GetEnumDescription(EnumValue.SaleOfBusinessSatus.WithProperty))
                    {
                        StatusId = (int)EnumValue.SaleOfBusinessSatus.WithProperty;
                    }
                    else if (PropertyModel.SaleOfBusinessType == EnumValue.GetEnumDescription(EnumValue.SaleOfBusinessSatus.WithoutProperty))
                    {
                        StatusId = (int)EnumValue.SaleOfBusinessSatus.WithoutProperty;
                    }
                    else if (PropertyModel.SaleOfBusinessType == EnumValue.GetEnumDescription(EnumValue.SaleOfBusinessSatus.Land))
                    {
                        StatusId = (int)EnumValue.SaleOfBusinessSatus.Land;
                    }

                    Propertylist = Propertylist.Where(c => c.SaleOfBusinessId == StatusId).ToList();
                }

                //Search property by Basement.
                if (PropertyModel.Basement != "" && PropertyModel.Basement != null)
                {
                    Propertylist = Propertylist.Where(c => c.Basement == PropertyModel.Basement.Trim()).ToList();
                }
                //Search property by Community.
                if (PropertyModel.Community != "" && PropertyModel.Community != null)
                {
                    Propertylist = Propertylist.Where(c => c.Community == PropertyModel.Community.Trim()).ToList();
                }
                //Search property by Bathrooms.
                if (PropertyModel.Bathrooms != "" && PropertyModel.Bathrooms != null)
                {
                    Propertylist = Propertylist.Where(c => c.Bathrooms == PropertyModel.Bathrooms.Trim()).ToList();
                }
                //Search property by Bedrooms.
                if (PropertyModel.Bedrooms != "" && PropertyModel.Bedrooms != null)
                {
                    Propertylist = Propertylist.Where(c => c.Bedrooms == PropertyModel.Bedrooms.Trim()).ToList();
                }
                //Search property by Kitchen.
                if (PropertyModel.Kitchen != "" && PropertyModel.Kitchen != null)
                {
                    Propertylist = Propertylist.Where(c => c.Kitchen == PropertyModel.Kitchen.Trim()).ToList();
                }


                //For LookingForCommercail range search
                int LookingForCommercailStatus = 0;
                if (PropertyModel.PropertyTypeStatus == EnumValue.PropertySatus.LookingForCommercial.ToString())
                {
                    LookingForCommercailStatus = (int)EnumValue.PropertySatus.LookingForCommercial;
                }
                else if (PropertyModel.PropertyTypeStatus == EnumValue.PropertySatus.LookingForResidential.ToString())
                {
                    LookingForCommercailStatus = (int)EnumValue.PropertySatus.LookingForResidential;
                }
                else if (PropertyModel.PropertyTypeStatus == EnumValue.PropertySatus.LookingForResidentialCondo.ToString())
                {
                    LookingForCommercailStatus = (int)EnumValue.PropertySatus.LookingForResidentialCondo;
                }

                if (LookingForCommercailStatus != 0)
                {
                    //Search property by Max-Min Price.
                    if (PropertyModel.MininumPrice != "" && PropertyModel.MininumPrice != null && PropertyModel.MaximumPrice != "" && PropertyModel.MaximumPrice != null)
                    {
                        Propertylist = Propertylist.Where(c => Convert.ToInt32(c.MininumPrice) >= Convert.ToInt32(PropertyModel.MininumPrice) && Convert.ToInt32(c.MaximumPrice) <= Convert.ToInt32(PropertyModel.MaximumPrice)).ToList();
                    }
                }
                else
                {
                    //Search property by  Price.
                    if (PropertyModel.MininumPrice != "" && PropertyModel.MininumPrice != null && PropertyModel.MaximumPrice != "" && PropertyModel.MaximumPrice != null)
                    {
                        Propertylist = Propertylist.Where(c => Convert.ToInt32(c.Price) >= Convert.ToInt32(PropertyModel.MininumPrice) && Convert.ToInt32(c.Price) <= Convert.ToInt32(PropertyModel.MaximumPrice)).ToList();
                    }
                }
                //End looking for commercial range search.



                //Search property by LocationPrefered.
                if (PropertyModel.LocationPrefered != "" && PropertyModel.LocationPrefered != null)
                {
                    Propertylist = Propertylist.Where(c => c.LocationPrefered == PropertyModel.LocationPrefered.Trim()).ToList();
                }
                //Search property by MLS.
                if (PropertyModel.MLS != "" && PropertyModel.MLS != null)
                {
                    Propertylist = Propertylist.Where(c => c.MLS == PropertyModel.MLS.Trim()).ToList();
                }
                //Search property by PropertyType.
                if (PropertyModel.PropertyType != "" && PropertyModel.PropertyType != null)
                {
                    Propertylist = Propertylist.Where(c => c.PropertyType == PropertyModel.PropertyType.Trim()).ToList();
                }

                //Search property by SideDoorEntrance.
                if (PropertyModel.SideDoorEntrance != "" && PropertyModel.SideDoorEntrance != null)
                {
                    Propertylist = Propertylist.Where(c => c.SideDoorEntrance == PropertyModel.SideDoorEntrance.Trim()).ToList();
                }
                //Search property by Size.
                if (PropertyModel.Size != "" && PropertyModel.Size != null)
                {
                    Propertylist = Propertylist.Where(c => c.Size == PropertyModel.Size.Trim()).ToList();
                }



                //Search property by Style.
                if (PropertyModel.Style != "" && PropertyModel.Style != null)
                {
                    Propertylist = Propertylist.Where(c => c.Style == PropertyModel.Style.Trim()).ToList();
                }

                //Search property by Garage.
                if (PropertyModel.Garage != "" && PropertyModel.Garage != null)
                {
                    Propertylist = Propertylist.Where(c => c.Garage == PropertyModel.Garage.Trim()).ToList();
                }
                //Search property by Type.
                if (PropertyModel.Type != "" && PropertyModel.Type != null)
                {
                    Propertylist = Propertylist.Where(c => c.Type == PropertyModel.Type.Trim()).ToList();
                }
                //Search property by Alivator.
                if (PropertyModel.Alivator != "" && PropertyModel.Alivator != null)
                {
                    Propertylist = Propertylist.Where(c => c.Alivator == PropertyModel.Alivator.Trim()).ToList();
                }
                //Search property by Balcony.
                if (PropertyModel.Balcony != "" && PropertyModel.Balcony != null)
                {
                    Propertylist = Propertylist.Where(c => c.Balcony == PropertyModel.Balcony.Trim()).ToList();
                }
                //Search property by ParkingSpace.
                if (PropertyModel.ParkingSpace != "" && PropertyModel.ParkingSpace != null)
                {
                    Propertylist = Propertylist.Where(c => c.ParkingSpace == PropertyModel.ParkingSpace.Trim()).ToList();
                }
                //Search property by GarageType.
                if (PropertyModel.GarageType != "" && PropertyModel.GarageType != null)
                {
                    Propertylist = Propertylist.Where(c => c.GarageType == PropertyModel.GarageType.Trim()).ToList();
                }
                //Search property by Laundry.
                if (PropertyModel.Loundry != "" && PropertyModel.Loundry != null)
                {
                    Propertylist = Propertylist.Where(c => c.Loundry == PropertyModel.Loundry.Trim()).ToList();
                }
                //Search property by Level.
                if (PropertyModel.Level != "" && PropertyModel.Level != null)
                {
                    Propertylist = Propertylist.Where(c => c.Level == PropertyModel.Level.Trim()).ToList();
                }

                //Search property by Level.
                if (PropertyModel.TypeCommercial != "" && PropertyModel.TypeCommercial != null)
                {
                    Propertylist = Propertylist.Where(c => c.TypeCommercial == PropertyModel.TypeCommercial.Trim()).ToList();
                }
                //Search property by Level.
                if (PropertyModel.TypeTaxes != "" && PropertyModel.TypeTaxes != null)
                {
                    Propertylist = Propertylist.Where(c => c.TypeTaxes == PropertyModel.TypeTaxes.Trim()).ToList();
                }
                //Search property by Level.
                if (PropertyModel.Use != "" && PropertyModel.Use != null)
                {
                    Propertylist = Propertylist.Where(c => c.Use == PropertyModel.Use.Trim()).ToList();
                }
                //Search property by Level.
                if (PropertyModel.Zoning != "" && PropertyModel.Zoning != null)
                {
                    Propertylist = Propertylist.Where(c => c.Zoning == PropertyModel.Zoning.Trim()).ToList();
                }
                //Search property by Level.
                if (PropertyModel.ListPriceCode != "" && PropertyModel.ListPriceCode != null)
                {
                    Propertylist = Propertylist.Where(c => c.ListPriceCode == PropertyModel.ListPriceCode.Trim()).ToList();
                }
                //Search property by Level.
                if (PropertyModel.CategoryCommercial != "" && PropertyModel.CategoryCommercial != null)
                {
                    Propertylist = Propertylist.Where(c => c.CategoryCommercial == PropertyModel.CategoryCommercial.Trim()).ToList();
                }
                var PropertyCount = Propertylist.Count();

                if (Propertylist.Count() > 0)
                {
                    Mapper.CreateMap<CommunicationApp.Entity.Property, CommunicationApp.Web.Models.PropertyModel>();
                    foreach (var property in Propertylist)
                    {

                        CommunicationApp.Web.Models.PropertyModel Propertymodel = Mapper.Map<CommunicationApp.Entity.Property, CommunicationApp.Web.Models.PropertyModel>(property);
                        Propertymodel.PropertyPhotosAndCustomerModelList = GetPropertyDetail(property);

                        // passing 'PropertyFor' to return model according to PropertyForStatusId.
                        if (property.PropertyForStatusId != null)
                        {
                            if (property.PropertyForStatusId == (int)EnumValue.PropertyForSatus.Lease)
                            {
                                Propertymodel.PropertyFor = EnumValue.GetEnumDescription(EnumValue.PropertyForSatus.Lease);
                            }
                            else if (property.PropertyForStatusId == (int)EnumValue.PropertyForSatus.SubLease)
                            {
                                Propertymodel.PropertyFor = EnumValue.GetEnumDescription(EnumValue.PropertyForSatus.SubLease);
                            }
                            else if (property.PropertyForStatusId == (int)EnumValue.PropertyForSatus.Sale)
                            {
                                Propertymodel.PropertyFor = EnumValue.GetEnumDescription(EnumValue.PropertyForSatus.Sale);
                            }
                        }

                        // passing 'SaleOfBusinessType' to return model according to SaleOfBusinessId.
                        if (property.SaleOfBusinessId != null)
                        {
                            if (property.SaleOfBusinessId == (int)EnumValue.SaleOfBusinessSatus.WithProperty)
                            {
                                Propertymodel.SaleOfBusinessType = EnumValue.GetEnumDescription(EnumValue.SaleOfBusinessSatus.WithProperty);
                            }
                            else if (property.SaleOfBusinessId == (int)EnumValue.SaleOfBusinessSatus.WithoutProperty)
                            {
                                Propertymodel.SaleOfBusinessType = EnumValue.GetEnumDescription(EnumValue.SaleOfBusinessSatus.WithoutProperty);
                            }
                            else if (property.SaleOfBusinessId == (int)EnumValue.SaleOfBusinessSatus.Land)
                            {
                                Propertymodel.SaleOfBusinessType = EnumValue.GetEnumDescription(EnumValue.SaleOfBusinessSatus.Land);
                            }
                        }

                        // passing 'PropertyStatusId' to return model according to PropertyType.
                        if (property.PropertyStatusId != null)
                        {
                            if (property.PropertyStatusId == (int)EnumValue.PropertySatus.ExclusiveCommercial)
                            {
                                Propertymodel.TypeOfProperty = EnumValue.GetEnumDescription(EnumValue.PropertySatus.ExclusiveCommercial);
                            }
                            else if (property.PropertyStatusId == (int)EnumValue.PropertySatus.ExclusiveResidential)
                            {
                                Propertymodel.TypeOfProperty = EnumValue.GetEnumDescription(EnumValue.PropertySatus.ExclusiveResidential);
                            }
                            else if (property.PropertyStatusId == (int)EnumValue.PropertySatus.ExclusiveResidentialCondo)
                            {
                                Propertymodel.TypeOfProperty = EnumValue.GetEnumDescription(EnumValue.PropertySatus.ExclusiveResidentialCondo);
                            }
                            else if (property.PropertyStatusId == (int)EnumValue.PropertySatus.LookingForCommercial)
                            {
                                Propertymodel.TypeOfProperty = EnumValue.GetEnumDescription(EnumValue.PropertySatus.LookingForCommercial);
                            }
                            else if (property.PropertyStatusId == (int)EnumValue.PropertySatus.LookingForResidential)
                            {
                                Propertymodel.TypeOfProperty = EnumValue.GetEnumDescription(EnumValue.PropertySatus.LookingForResidential);
                            }
                            else if (property.PropertyStatusId == (int)EnumValue.PropertySatus.LookingForResidentialCondo)
                            {
                                Propertymodel.TypeOfProperty = EnumValue.GetEnumDescription(EnumValue.PropertySatus.LookingForResidentialCondo);
                            }
                            else if (property.PropertyStatusId == (int)EnumValue.PropertySatus.NewHotListingCommercial)
                            {
                                Propertymodel.TypeOfProperty = EnumValue.GetEnumDescription(EnumValue.PropertySatus.NewHotListingCommercial);
                            }
                            else if (property.PropertyStatusId == (int)EnumValue.PropertySatus.NewHotListingResidential)
                            {
                                Propertymodel.TypeOfProperty = EnumValue.GetEnumDescription(EnumValue.PropertySatus.NewHotListingResidential);
                            }
                            else if (property.PropertyStatusId == (int)EnumValue.PropertySatus.NewHotListingResidentialCondo)
                            {
                                Propertymodel.TypeOfProperty = EnumValue.GetEnumDescription(EnumValue.PropertySatus.NewHotListingResidentialCondo);
                            }

                        }
                        if (property.IsActive == false)
                        {
                            Propertymodel.Remark = "";
                            Propertymodel.PropertyImage = ConfigurationManager.AppSettings["LiveURL"].ToString() + "/images/noImage.jpg";
                        }
                        var PropertyViewCount = _ViewsService.GetViewss().Where(s => s.PropertyId == Propertymodel.PropertyId).Count();
                        if (PropertyViewCount != 0)
                        {
                            Propertymodel.propertyViewsCount = PropertyViewCount;
                        }

                        Propertymodel.TotalPropertyCount = PropertyCount;
                        if (Propertymodel.CustomerId != 0)
                        {
                            var Customer = _CustomerService.GetCustomer(Convert.ToInt32(Propertymodel.CustomerId));
                            if (Customer != null)
                            {
                                var Admin = _CustomerService.GetCustomer(Convert.ToInt32(Customer.ParentId));
                                if (Admin != null)
                                {
                                    Propertymodel.AdminName = Admin.FirstName;
                                    Propertymodel.AdminPhoto = Admin.PhotoPath;
                                    Propertymodel.AdminWebSiteUrl = Admin.WebsiteUrl;
                                    Propertymodel.AdminPhoneNo = Admin.MobileNo;
                                    Propertymodel.AdminEmail = Admin.EmailId;
                                    var Company = _CompanyService.GetCompany(Admin.CompanyID);
                                    if (Company != null)
                                    {
                                        Propertymodel.AdminCompanyLogo = Company.LogoPath;
                                        Propertymodel.AdminCompanyAddress = Company.CompanyAddress;
                                        Propertymodel.AdminCompanyName = Company.CompanyName;
                                    }
                                }
                            }

                        }
                        models.Add(Propertymodel);
                    }
                    return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("success", models), Configuration.Formatters.JsonFormatter);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "No record found."), Configuration.Formatters.JsonFormatter);
                }
            }
            catch (Exception ex)
            {

                string ErrorMsg = ex.Message.ToString();
                ErrorLogging.LogError(ex);
                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Please try later."), Configuration.Formatters.JsonFormatter);
            }
        }

        [Route("SaveAgents")]
        [HttpPost]
        public HttpResponseMessage SaveAgents([FromBody]AgentModel AgentModel)
        {
            string Flag = "1";
            string Message = "";
            string PropertyType = "";
            try
            {
                Mapper.CreateMap<CommunicationApp.Models.AgentModel, CommunicationApp.Entity.Agent>();
                CommunicationApp.Entity.Agent Agent = Mapper.Map<CommunicationApp.Models.AgentModel, CommunicationApp.Entity.Agent>(AgentModel);
                if (AgentModel.AgentTypeStatus == "" || AgentModel.AgentTypeStatus == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Agent Type Status cannot be blank."), Configuration.Formatters.JsonFormatter);
                }
                if (AgentModel.AgentTypeStatus == EnumValue.AgentSatus.AgentAvailable.ToString())
                {
                    Agent.AgentStatusId = (int)EnumValue.AgentSatus.AgentAvailable;
                    PropertyType = "Agent Available";
                    Flag = "10";//status for AgentAvailable;
                }
                else if (AgentModel.AgentTypeStatus == EnumValue.AgentSatus.AgentRequired.ToString())
                {
                    Agent.AgentStatusId = (int)EnumValue.AgentSatus.AgentRequired;
                    PropertyType = "Agent Required";
                    Flag = "11";//status for AgentRequired;
                }
                Message += "New open house of " + PropertyType + " is available.";
                if (AgentModel.FromTime != "" && AgentModel.FromTime != null)
                {
                    Agent.FromTime = AgentModel.FromTime;
                }
                else
                {
                    Agent.FromTime = "";
                }
                if (AgentModel.ToTime != "" && AgentModel.ToTime != null)
                {
                    Agent.ToTime = AgentModel.ToTime;
                }
                else
                {
                    Agent.ToTime = "";
                }
                if (AgentModel.Date != "" && AgentModel.Date != null)
                {
                    Agent.Date = AgentModel.Date;
                }
                else
                {
                    Agent.Date = "";
                }
                if (AgentModel.FromTime2 != "" && AgentModel.FromTime2 != null)
                {
                    Agent.FromTime2 = AgentModel.FromTime2;
                }
                else
                {
                    Agent.FromTime2 = "";
                }
                if (AgentModel.ToTime2 != "" && AgentModel.ToTime2 != null)
                {
                    Agent.ToTime2 = AgentModel.ToTime2;
                }
                else
                {
                    Agent.ToTime2 = "";
                }
                if (AgentModel.Date2 != "" && AgentModel.Date2 != null)
                {
                    Agent.Date2 = AgentModel.Date2;
                }
                else
                {
                    Agent.Date2 = "";
                }
                Agent.CompanyId = 1;
                Agent.ParentId = AgentModel.ParentId;
                Agent.IsActive = false;
                Agent.CreatedOn = DateTime.Now;
                _AgentService.InsertAgent(Agent);

                //update customer table
                var customer = _CustomerService.GetCustomer(Convert.ToInt32(Agent.CustomerId));
                if (customer != null)
                {
                    customer.LastUpdatedOn = DateTime.Now;
                    customer.IsAvailable = true;
                    _CustomerService.UpdateCustomer(customer);
                }
                //

                PropertyModel PropertyModel = new Web.Models.PropertyModel();
                PropertyModel.PropertyType = PropertyType;
                PropertyModel.PropertyTypeStatus = "Open House Property";
                PropertyModel.LocationPrefered = Agent.City;
                AgentModel.AgentId = Agent.AgentId;
                var Body = "";
                //Send Notification
                var PropertyCustomer = _CustomerService.GetCustomer(Convert.ToInt32(Agent.CustomerId));
                string Subject = "New Property registered.";
                if (PropertyCustomer != null)
                {
                    string FirstName = PropertyCustomer.FirstName + " " + PropertyCustomer.MiddleName + " " + PropertyCustomer.LastName;
                    PropertyModel.CustomerTrebId = PropertyCustomer.TrebId;
                    SendMailToUser(FirstName, PropertyCustomer.EmailId, PropertyModel);
                    SendMailToAdmin(FirstName, PropertyCustomer.EmailId, PropertyModel, Subject, Body);
                }
                //End

                //send notification
                JobScheduler.SendNotificationOpenHouse(AgentModel.AgentId.ToString(), Flag.ToString(), Message);

                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("success", Agent.AgentId), Configuration.Formatters.JsonFormatter);

            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.Message.ToString();
                ErrorLogging.LogError(ex);
                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Please try later."), Configuration.Formatters.JsonFormatter);
            }
        }

        [Route("SaveFeedBacks")]
        [HttpPost]
        public HttpResponseMessage SaveFeedBacks([FromBody]FeedBackModel FeedBackModel)
        {

            try
            {
                Mapper.CreateMap<CommunicationApp.Models.FeedBackModel, CommunicationApp.Entity.FeedBack>();
                CommunicationApp.Entity.FeedBack FeedBack = Mapper.Map<CommunicationApp.Models.FeedBackModel, CommunicationApp.Entity.FeedBack>(FeedBackModel);
                FeedBack.CreatedOn = DateTime.Now;
                _FeedBackService.InsertFeedBack(FeedBack);

                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("success", FeedBackModel), Configuration.Formatters.JsonFormatter);

            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.Message.ToString();
                ErrorLogging.LogError(ex);
                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Please try later."), Configuration.Formatters.JsonFormatter);
            }
        }

        [Route("DeleteProperty")]
        [HttpGet]
        public HttpResponseMessage DeleteProperty(int PropertyId)
        {
            try
            {
                //Delete from Property, It will delete from user, user role & Propertys
                var Property = _PropertyService.GetProperty(PropertyId);
                if (Property != null)
                {
                    var PropertyImages = _PropertyImageService.GetPropertyImages().Where(c => c.PropertyId == PropertyId);
                    foreach (var PropertyImage in PropertyImages)
                    {
                        _PropertyImageService.DeletePropertyImage(PropertyImage);
                    }

                    var PropertyViews = _ViewsService.GetViewss().Where(c => c.PropertyId == PropertyId);
                    foreach (var PropertyView in PropertyViews)
                    {
                        _ViewsService.DeleteViews(PropertyView);
                    }
                    _PropertyService.DeleteProperty(Property);
                    return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("success", "Property deleted successfully."), Configuration.Formatters.JsonFormatter);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("success", "No record found."), Configuration.Formatters.JsonFormatter);
                }

            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.Message.ToString();
                ErrorLogging.LogError(ex);
                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Please try later."), Configuration.Formatters.JsonFormatter);
            }
        }

        [Route("DeleteAgent")]
        [HttpGet]
        public HttpResponseMessage DeleteAgent(int AgentId)
        {
            try
            {
                //Delete from Property, It will delete from user, user role & Propertys
                var Agent = _AgentService.GetAgent(AgentId);
                if (Agent != null)
                {
                    var AgentImages = _PropertyImageService.GetPropertyImages().Where(c => c.AgentId == AgentId);
                    foreach (var AgentImage in AgentImages)
                    {
                        _PropertyImageService.DeletePropertyImage(AgentImage);
                    }
                    _AgentService.DeleteAgent(Agent);
                    return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("success", "Agent deleted successfully."), Configuration.Formatters.JsonFormatter);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("success", "No record found."), Configuration.Formatters.JsonFormatter);
                }

            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.Message.ToString();
                ErrorLogging.LogError(ex);
                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Please try later."), Configuration.Formatters.JsonFormatter);
            }
        }

        [Route("UpdateAgent")]
        [HttpPost]
        public HttpResponseMessage UpdateAgent([FromBody]AgentModel AgentModel)
        {
            try
            {

                var Agent = _AgentService.GetAgents().Where(x => x.AgentId == AgentModel.AgentId).FirstOrDefault();


                if (Agent != null)
                {

                    if ((AgentModel.City != null) && (AgentModel.City != ""))
                    {
                        Agent.City = AgentModel.City;
                    }
                    if ((AgentModel.Comments != null) && (AgentModel.Comments != ""))
                    {
                        Agent.Comments = AgentModel.Comments;
                    }
                    if ((AgentModel.FromTime != null) && (AgentModel.FromTime != ""))
                    {
                        Agent.FromTime = AgentModel.FromTime;
                    }
                    if ((AgentModel.ToTime != null) && (AgentModel.ToTime != ""))
                    {
                        Agent.ToTime = AgentModel.ToTime;
                    }
                    if ((AgentModel.Date != null) && (AgentModel.Date != ""))
                    {
                        Agent.Date = AgentModel.Date;
                    }
                    if ((AgentModel.FromTime2 != null) && (AgentModel.FromTime2 != ""))
                    {
                        Agent.FromTime2 = AgentModel.FromTime2;
                    }
                    if ((AgentModel.ToTime2 != null) && (AgentModel.ToTime2 != ""))
                    {
                        Agent.ToTime2 = AgentModel.ToTime2;
                    }
                    if ((AgentModel.Date2 != null) && (AgentModel.Date2 != ""))
                    {
                        Agent.Date2 = AgentModel.Date2;
                    }
                    if ((AgentModel.MLS != null) && (AgentModel.MLS != ""))
                    {
                        Agent.MLS = AgentModel.MLS;
                    }
                    if ((AgentModel.Price != null))
                    {
                        Agent.Price = AgentModel.Price;
                    }

                    var AgentImages = _PropertyImageService.GetPropertyImages().Where(c => c.AgentId == Agent.AgentId);
                    foreach (var Agentimage in AgentImages)
                    {
                        if (Agentimage.ImagePath.Contains('.'))
                        {
                            if ((Agentimage.ImagePath != "") && (Agentimage.ImagePath != null))
                            {
                                DeleteImage(Agentimage.ImagePath);
                                // Agentimage.ImagePath = "";

                                _PropertyImageService.DeletePropertyImage(Agentimage);
                            }

                        }
                    }

                    Agent.IsActive = false;
                    _AgentService.UpdateAgent(Agent);
                    PropertyModel PropertyModel = new Web.Models.PropertyModel();
                    if (Agent.AgentStatusId == (int)EnumValue.AgentSatus.AgentAvailable)
                    {
                        PropertyModel.PropertyType = EnumValue.GetEnumDescription(EnumValue.AgentSatus.AgentAvailable);
                    }
                    else if (Agent.AgentStatusId == (int)EnumValue.AgentSatus.AgentRequired)
                    {
                        PropertyModel.PropertyType = EnumValue.GetEnumDescription(EnumValue.AgentSatus.AgentRequired);
                    }
                    PropertyModel.PropertyTypeStatus = "Open House Property";
                    PropertyModel.LocationPrefered = Agent.City;
                    var Customer = _CustomerService.GetCustomer(Convert.ToInt32(Agent.CustomerId));
                    string Subject = "Property has Updated.";
                    string body = "";
                    if (Customer != null)
                    {
                        string FirstName = Customer.FirstName + " " + Customer.MiddleName + " " + Customer.LastName;
                        PropertyModel.CustomerTrebId = Customer.TrebId;
                        SendMailToUpdateUser(FirstName, Customer.EmailId, PropertyModel);
                        SendMailToAdmin(FirstName, Customer.EmailId, PropertyModel, Subject, body);
                    }
                    return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("success", Agent.AgentId), Configuration.Formatters.JsonFormatter);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("success", "RideId is not found."), Configuration.Formatters.JsonFormatter);
                }
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.Message.ToString();
                ErrorLogging.LogError(ex);
                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Please try later."), Configuration.Formatters.JsonFormatter);
            }
        }
        [Route("UpdateProperty")]
        [HttpPost]
        public HttpResponseMessage UpdateProperty([FromBody]PropertyModel PropertyModel)
        {
            try
            {

                string PropertyType = "";
                //for set model default value.
                PropertyModel.PropertyId = PropertyModel.PropertyId == null ? 0 : PropertyModel.PropertyId;
                PropertyModel.CustomerId = PropertyModel.CustomerId == null ? 0 : PropertyModel.CustomerId;
                PropertyModel.CustomerName = PropertyModel.CustomerName == null ? "" : PropertyModel.CustomerName;
                PropertyModel.CustomerTrebId = PropertyModel.CustomerTrebId == null ? "" : PropertyModel.CustomerTrebId;
                PropertyModel.CustomerPhoto = PropertyModel.CustomerPhoto == null ? "" : PropertyModel.CustomerPhoto;
                PropertyModel.CustomerEmail = PropertyModel.CustomerEmail == null ? "" : PropertyModel.CustomerEmail;
                PropertyModel.CustomerPhoneNo = PropertyModel.CustomerPhoneNo == null ? "" : PropertyModel.CustomerPhoneNo;
                PropertyModel.Kitchen = PropertyModel.Kitchen == null ? "" : PropertyModel.Kitchen;
                PropertyModel.SideDoorEntrance = PropertyModel.SideDoorEntrance == null ? "" : PropertyModel.SideDoorEntrance;
                PropertyModel.PropertyStatusId = PropertyModel.PropertyStatusId == null ? 0 : PropertyModel.PropertyStatusId;
                PropertyModel.MLS = PropertyModel.MLS == null ? "" : PropertyModel.MLS;
                PropertyModel.Price = PropertyModel.Price == null ? "" : PropertyModel.Price;
                PropertyModel.MininumPrice = PropertyModel.MininumPrice == null ? "" : PropertyModel.MininumPrice;
                PropertyModel.MaximumPrice = PropertyModel.MaximumPrice == null ? "" : PropertyModel.MaximumPrice;
                PropertyModel.LocationPrefered = PropertyModel.LocationPrefered == null ? "" : PropertyModel.LocationPrefered;
                PropertyModel.Style = PropertyModel.Style == null ? "" : PropertyModel.Style;
                PropertyModel.Age = PropertyModel.Age == null ? "" : PropertyModel.Age;
                PropertyModel.Garage = PropertyModel.Garage == null ? "" : PropertyModel.Garage;
                PropertyModel.Bedrooms = PropertyModel.Bedrooms == null ? "" : PropertyModel.Bedrooms;
                PropertyModel.Bathrooms = PropertyModel.Bathrooms == null ? "" : PropertyModel.Bathrooms;
                PropertyModel.PropertyType = PropertyModel.PropertyType == null ? "" : PropertyModel.PropertyType;
                PropertyModel.Basement = PropertyModel.Basement == null ? "" : PropertyModel.Basement;
                PropertyModel.BasementValue = PropertyModel.BasementValue == null ? "" : PropertyModel.BasementValue;
                PropertyModel.Size = PropertyModel.Size == null ? "" : PropertyModel.Size;
                PropertyModel.Remark = PropertyModel.Remark == null ? "" : PropertyModel.Remark;
                PropertyModel.WebsiteUrl = PropertyModel.WebsiteUrl == null ? "" : PropertyModel.WebsiteUrl;
                PropertyModel.Type = PropertyModel.Type == null ? "" : PropertyModel.Type;
                PropertyModel.Balcony = PropertyModel.Balcony == null ? "" : PropertyModel.Balcony;
                PropertyModel.Alivator = PropertyModel.Alivator == null ? "" : PropertyModel.Alivator;
                PropertyModel.ParkingSpace = PropertyModel.ParkingSpace == null ? "" : PropertyModel.ParkingSpace;
                PropertyModel.GarageType = PropertyModel.GarageType == null ? "" : PropertyModel.GarageType;
                PropertyModel.PropertyTypeStatus = PropertyModel.PropertyTypeStatus == null ? "" : PropertyModel.PropertyTypeStatus;
                PropertyModel.TypeOfProperty = PropertyModel.TypeOfProperty == null ? "" : PropertyModel.TypeOfProperty;
                PropertyModel.PropertyFor = PropertyModel.PropertyFor == null ? "" : PropertyModel.PropertyFor;
                PropertyModel.SaleOfBusinessType = PropertyModel.SaleOfBusinessType == null ? "" : PropertyModel.SaleOfBusinessType;
                PropertyModel.PropertyImage = PropertyModel.PropertyImage == null ? "" : PropertyModel.PropertyImage;
                PropertyModel.Loundry = PropertyModel.Loundry == null ? "" : PropertyModel.Loundry;
                PropertyModel.Level = PropertyModel.Level == null ? "" : PropertyModel.Level;
                PropertyModel.ListPriceCode = PropertyModel.ListPriceCode == null ? "" : PropertyModel.ListPriceCode;
                PropertyModel.TypeTaxes = PropertyModel.TypeTaxes == null ? "" : PropertyModel.TypeTaxes;
                PropertyModel.TypeCommercial = PropertyModel.TypeCommercial == null ? "" : PropertyModel.TypeCommercial;
                PropertyModel.CategoryCommercial = PropertyModel.CategoryCommercial == null ? "" : PropertyModel.CategoryCommercial;
                PropertyModel.Use = PropertyModel.Use == null ? "" : PropertyModel.Use;
                PropertyModel.Zoning = PropertyModel.Zoning == null ? "" : PropertyModel.Zoning;
                PropertyModel.IsActive = PropertyModel.IsActive == null ? false : PropertyModel.IsActive;
                PropertyModel.TypeOfProperty = PropertyModel.TypeOfProperty == null ? "" : PropertyModel.TypeOfProperty;

                //End


                //Mapper.CreateMap<CommunicationApp.Web.Models.PropertyModel, CommunicationApp.Entity.Property>();
                //CommunicationApp.Entity.Property Property = Mapper.Map<CommunicationApp.Web.Models.PropertyModel, CommunicationApp.Entity.Property>(PropertyModel);
                var Property = _PropertyService.GetPropertys().Where(x => x.PropertyId == PropertyModel.PropertyId).FirstOrDefault();

                if (Property != null)
                {
                    //Update Property Type.
                    if (PropertyModel.PropertyTypeStatus == EnumValue.PropertySatus.ExclusiveCommercial.ToString())
                    {
                        Property.PropertyStatusId = (int)EnumValue.PropertySatus.ExclusiveCommercial;
                        PropertyType = "Exclusive Listing Commercial";
                    }
                    else if (PropertyModel.PropertyTypeStatus == EnumValue.PropertySatus.ExclusiveResidential.ToString())
                    {
                        Property.PropertyStatusId = (int)EnumValue.PropertySatus.ExclusiveResidential;
                        PropertyType = "Exclusive Listing Residential";
                    }
                    else if (PropertyModel.PropertyTypeStatus == EnumValue.PropertySatus.NewHotListingCommercial.ToString())
                    {
                        Property.PropertyStatusId = (int)EnumValue.PropertySatus.NewHotListingCommercial;
                        PropertyType = "New Hot Listing Commercial";
                    }
                    else if (PropertyModel.PropertyTypeStatus == EnumValue.PropertySatus.NewHotListingResidential.ToString())
                    {
                        Property.PropertyStatusId = (int)EnumValue.PropertySatus.NewHotListingResidential;
                        PropertyType = "New Hot Listing Residential";
                    }
                    else if (PropertyModel.PropertyTypeStatus == EnumValue.PropertySatus.LookingForCommercial.ToString())
                    {
                        Property.PropertyStatusId = (int)EnumValue.PropertySatus.LookingForCommercial;
                        PropertyType = "Looking For Commercial";
                    }
                    else if (PropertyModel.PropertyTypeStatus == EnumValue.PropertySatus.LookingForResidential.ToString())
                    {
                        Property.PropertyStatusId = (int)EnumValue.PropertySatus.LookingForResidential;
                        PropertyType = "Looking For Residential";
                    }
                    else if (PropertyModel.PropertyTypeStatus == EnumValue.PropertySatus.ExclusiveResidentialCondo.ToString())
                    {
                        Property.PropertyStatusId = (int)EnumValue.PropertySatus.ExclusiveResidentialCondo;
                        PropertyType = "Exclusive Listing Residential Condo";
                    }
                    else if (PropertyModel.PropertyTypeStatus == EnumValue.PropertySatus.LookingForResidentialCondo.ToString())
                    {
                        Property.PropertyStatusId = (int)EnumValue.PropertySatus.LookingForResidentialCondo;
                        PropertyType = "Looking For Residential Condo";
                    }
                    else if (PropertyModel.PropertyTypeStatus == EnumValue.PropertySatus.NewHotListingResidentialCondo.ToString())
                    {
                        Property.PropertyStatusId = (int)EnumValue.PropertySatus.NewHotListingResidentialCondo;
                        PropertyType = "New Hot Listing Residential Condo";
                    }



                    //Update PropertyFor Status Data.
                    if (PropertyModel.PropertyFor == EnumValue.GetEnumDescription(EnumValue.PropertyForSatus.Sale))
                    {
                        Property.PropertyForStatusId = (int)EnumValue.PropertyForSatus.Sale;
                    }
                    else if (PropertyModel.PropertyFor == EnumValue.GetEnumDescription(EnumValue.PropertyForSatus.Lease))
                    {
                        Property.PropertyForStatusId = (int)EnumValue.PropertyForSatus.Lease;
                    }
                    else if (PropertyModel.PropertyFor == EnumValue.GetEnumDescription(EnumValue.PropertyForSatus.SubLease))
                    {
                        Property.PropertyForStatusId = (int)EnumValue.PropertyForSatus.SubLease;
                    }


                    //Update SaleOfBusiness Status Data.
                    if (PropertyModel.SaleOfBusinessType == EnumValue.GetEnumDescription(EnumValue.SaleOfBusinessSatus.WithProperty))
                    {
                        Property.SaleOfBusinessId = (int)EnumValue.SaleOfBusinessSatus.WithProperty;
                    }
                    else if (PropertyModel.SaleOfBusinessType == EnumValue.GetEnumDescription(EnumValue.SaleOfBusinessSatus.WithoutProperty))
                    {
                        Property.SaleOfBusinessId = (int)EnumValue.SaleOfBusinessSatus.WithoutProperty;
                    }
                    else if (PropertyModel.SaleOfBusinessType == EnumValue.GetEnumDescription(EnumValue.SaleOfBusinessSatus.Land))
                    {
                        Property.SaleOfBusinessId = (int)EnumValue.SaleOfBusinessSatus.Land;
                    }

                    if ((PropertyModel.Age != null) && (PropertyModel.Age != ""))
                    {
                        Property.Age = PropertyModel.Age;
                    }
                    if ((PropertyModel.Basement != null) && (PropertyModel.Basement != ""))
                    {
                        Property.Basement = PropertyModel.Basement;
                    }
                    if ((PropertyModel.BasementValue != null) && (PropertyModel.BasementValue != ""))
                    {
                        Property.BasementValue = PropertyModel.BasementValue;
                    }
                    if ((PropertyModel.Bathrooms != null) && (PropertyModel.Bathrooms != ""))
                    {
                        Property.Bathrooms = PropertyModel.Bathrooms;
                    }
                    if ((PropertyModel.Bedrooms != null) && (PropertyModel.Bedrooms != ""))
                    {
                        Property.Bedrooms = PropertyModel.Bedrooms;
                    }
                    if ((PropertyModel.Community != null) && (PropertyModel.Community != ""))
                    {
                        Property.Community = PropertyModel.Community;
                    }
                    if ((PropertyModel.Kitchen != null) && (PropertyModel.Kitchen != ""))
                    {
                        Property.Kitchen = PropertyModel.Kitchen;
                    }
                    if ((PropertyModel.LocationPrefered != null) && (PropertyModel.LocationPrefered != ""))
                    {
                        Property.LocationPrefered = PropertyModel.LocationPrefered;
                    }
                    if ((PropertyModel.Price != null) && (PropertyModel.Price != ""))
                    {
                        Property.Price = PropertyModel.Price;
                    }
                    if ((PropertyModel.MaximumPrice != null) && (PropertyModel.MaximumPrice != ""))
                    {
                        Property.MaximumPrice = PropertyModel.MaximumPrice;
                    }
                    if ((PropertyModel.MininumPrice != null) && (PropertyModel.MininumPrice != ""))
                    {
                        Property.MininumPrice = PropertyModel.MininumPrice;
                    }
                    if ((PropertyModel.MLS != null) && (PropertyModel.MLS != ""))
                    {
                        Property.MLS = PropertyModel.MLS;
                    }
                    if ((PropertyModel.Remark != null) && (PropertyModel.Remark != ""))
                    {
                        Property.Remark = PropertyModel.Remark;
                    }
                    if ((PropertyModel.SideDoorEntrance != null) && (PropertyModel.SideDoorEntrance != ""))
                    {
                        Property.SideDoorEntrance = PropertyModel.SideDoorEntrance;
                    }
                    if ((PropertyModel.Size != null) && (PropertyModel.Size != ""))
                    {
                        Property.Size = PropertyModel.Size;
                    }

                    if ((PropertyModel.Style != null) && (PropertyModel.Style != ""))
                    {
                        Property.Style = PropertyModel.Style;
                    }
                    if ((PropertyModel.PropertyType != null) && (PropertyModel.PropertyType != ""))
                    {
                        Property.PropertyType = PropertyModel.PropertyType;
                    }
                    if ((PropertyModel.Garage != null) && (PropertyModel.Garage != ""))
                    {
                        Property.Garage = PropertyModel.Garage;
                    }
                    if ((PropertyModel.Type != null) && (PropertyModel.Type != ""))
                    {
                        Property.Type = PropertyModel.Type;
                    }
                    if ((PropertyModel.Balcony != null) && (PropertyModel.Balcony != ""))
                    {
                        Property.Balcony = PropertyModel.Balcony;
                    }
                    if ((PropertyModel.Alivator != null) && (PropertyModel.Alivator != ""))
                    {
                        Property.Alivator = PropertyModel.Alivator;
                    }
                    if ((PropertyModel.GarageType != null) && (PropertyModel.GarageType != ""))
                    {
                        Property.GarageType = PropertyModel.GarageType;
                    }
                    if ((PropertyModel.ParkingSpace != null) && (PropertyModel.ParkingSpace != ""))
                    {
                        Property.ParkingSpace = PropertyModel.ParkingSpace;
                    }
                    if ((PropertyModel.Level != null) && (PropertyModel.Level != ""))
                    {
                        Property.Level = PropertyModel.Level;
                    }
                    if ((PropertyModel.Loundry != null) && (PropertyModel.Loundry != ""))
                    {
                        Property.Loundry = PropertyModel.Loundry;
                    }

                    if ((PropertyModel.Use != null) && (PropertyModel.Use != ""))
                    {
                        Property.Use = PropertyModel.Use;
                    }
                    if ((PropertyModel.Zoning != null) && (PropertyModel.Zoning != ""))
                    {
                        Property.Zoning = PropertyModel.Zoning;
                    }
                    if ((PropertyModel.TypeCommercial != null) && (PropertyModel.TypeCommercial != ""))
                    {
                        Property.TypeCommercial = PropertyModel.TypeCommercial;
                    }
                    if ((PropertyModel.TypeTaxes != null) && (PropertyModel.TypeTaxes != ""))
                    {
                        Property.TypeTaxes = PropertyModel.TypeTaxes;
                    }
                    if ((PropertyModel.CategoryCommercial != null) && (PropertyModel.CategoryCommercial != ""))
                    {
                        Property.CategoryCommercial = PropertyModel.CategoryCommercial;
                    }
                    if ((PropertyModel.ListPriceCode != null) && (PropertyModel.ListPriceCode != ""))
                    {
                        Property.ListPriceCode = PropertyModel.ListPriceCode;
                    }


                    var PropertyImages = _PropertyImageService.GetPropertyImages().Where(c => c.PropertyId == Property.PropertyId);
                    foreach (var Propertyimage in PropertyImages)
                    {
                        if (Propertyimage.ImagePath.Contains('.'))
                        {
                            if ((Propertyimage.ImagePath != "") && (Propertyimage.ImagePath != null))
                            {
                                DeleteImage(Propertyimage.ImagePath);
                                Propertyimage.ImagePath = "";
                                _PropertyImageService.DeletePropertyImage(Propertyimage);
                            }

                        }
                    }

                    Property.IsActive = false;
                    Property.IsPropertyUpdated = true;
                    _PropertyService.UpdateProperty(Property);
                    PropertyModel.Style = Property.Style;
                    PropertyModel.PropertyTypeStatus = PropertyType;
                    PropertyModel.PropertyType = Property.PropertyType;
                    PropertyModel.LocationPrefered = Property.LocationPrefered;
                    string body = "New property has been updated by user and waiting for your approval.";
                    var Customer = _CustomerService.GetCustomer(Convert.ToInt32(Property.CustomerId));
                    string Subject = "Property Updated by user";
                    if (Customer != null)
                    {
                        string FirstName = Customer.FirstName + " " + Customer.MiddleName + " " + Customer.LastName;
                        PropertyModel.CustomerTrebId = Customer.TrebId;
                        SendMailToUpdateUser(FirstName, Customer.EmailId, PropertyModel);
                        SendMailToAdmin(FirstName, Customer.EmailId, PropertyModel, Subject, body);
                    }

                    return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("success", Property.PropertyId), Configuration.Formatters.JsonFormatter);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("success", "Property is not found."), Configuration.Formatters.JsonFormatter);
                }
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.Message.ToString();
                ErrorLogging.LogError(ex);
                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Please try later."), Configuration.Formatters.JsonFormatter);
            }
        }

        [Route("TestMail")]
        [HttpGet]
        public HttpResponseMessage TestMail()
        {

            //string UserName = "Maninder";
            //string EmailAddress = "maninder.singh.2114@gmail.com";
            //string TrebId = "123456";
            //string PropertyType = "New Hot Listing";
            //string Subject = "New Property Register.";
            //PropertyModel PropertyTypes = new PropertyModel();
            //PropertyTypes.LocationPrefered = "Brampton";
            //PropertyTypes.Style = "";
            //PropertyTypes.PropertyTypeStatus = "New Hot Listing";
            //PropertyTypes.LocationPrefered = "Brampton";
            //PropertyTypes.PropertyType = "Att/Row/TwnHouse";
            //SendMailToUser(UserName, EmailAddress, PropertyTypes);
            // SendMailToAdmin(UserName, EmailAddress, PropertyTypes, Subject);        
            // SendMailToUpdateUser(UserName, EmailAddress, PropertyTypes);
            return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Please try later."), Configuration.Formatters.JsonFormatter);
        }

        [Route("GetNotificationCount")]
        [HttpGet]
        public HttpResponseMessage GetNotificationCount([FromUri] int CustomerId)
        {
            try
            {
                if ((CustomerId == null) || (CustomerId == 0))
                {
                    return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "UserId is not found."), Configuration.Formatters.JsonFormatter);
                }

                List<int> ExclusiveCommercial = new List<int>();
                List<int> ExclusiveResidential = new List<int>();
                List<int> ExclusiveResidentialCondo = new List<int>();
                List<int> LookingForCommercial = new List<int>();
                List<int> LookingForResidential = new List<int>();
                List<int> LookingForResidentialCondo = new List<int>();
                List<int> NewHotListingCommercial = new List<int>();
                List<int> NewHotListingResidential = new List<int>();
                List<int> NewHotListingResidentialCondo = new List<int>();



                NotificationCountModel NotificationCountModel = new Models.NotificationCountModel();
                var NotificationCount = 0;
                var NotificationsList = _Notification.GetNotifications().Where(x => x.NotificationSendTo == CustomerId && x.IsRead == false).AsEnumerable();
                if (NotificationsList != null)
                {
                    foreach (var Notifications in NotificationsList)
                    {

                        if (Notifications.Flag == (int)EnumValue.PropertySatus.ExclusiveCommercial)
                        {
                            ExclusiveCommercial.Add((int)EnumValue.PropertySatus.ExclusiveCommercial);
                        }
                        else if (Notifications.Flag == (int)EnumValue.PropertySatus.ExclusiveResidential)
                        {
                            ExclusiveResidential.Add((int)EnumValue.PropertySatus.ExclusiveResidential);
                        }
                        else if (Notifications.Flag == (int)EnumValue.PropertySatus.ExclusiveResidentialCondo)
                        {
                            ExclusiveResidentialCondo.Add((int)EnumValue.PropertySatus.ExclusiveResidentialCondo);
                        }
                        else if (Notifications.Flag == (int)EnumValue.PropertySatus.LookingForCommercial)
                        {
                            LookingForCommercial.Add((int)EnumValue.PropertySatus.LookingForCommercial);
                        }
                        else if (Notifications.Flag == (int)EnumValue.PropertySatus.LookingForResidential)
                        {
                            LookingForResidential.Add((int)EnumValue.PropertySatus.LookingForResidential);
                        }
                        else if (Notifications.Flag == (int)EnumValue.PropertySatus.LookingForResidentialCondo)
                        {
                            LookingForResidentialCondo.Add((int)EnumValue.PropertySatus.LookingForResidentialCondo);
                        }
                        else if (Notifications.Flag == (int)EnumValue.PropertySatus.NewHotListingCommercial)
                        {
                            NewHotListingCommercial.Add((int)EnumValue.PropertySatus.NewHotListingCommercial);
                        }
                        else if (Notifications.Flag == (int)EnumValue.PropertySatus.NewHotListingResidential)
                        {
                            NewHotListingResidential.Add((int)EnumValue.PropertySatus.NewHotListingResidential);
                        }
                        else if (Notifications.Flag == (int)EnumValue.PropertySatus.NewHotListingResidentialCondo)
                        {
                            NewHotListingResidentialCondo.Add((int)EnumValue.PropertySatus.NewHotListingResidentialCondo);
                        }


                        Notifications.IsRead = true;
                        _Notification.UpdateNotification(Notifications);
                    }
                    //NotificationCount = NotificationsList.Count();

                    NotificationCountModel.ExclusiveCommercial = ExclusiveCommercial.Count();
                    NotificationCountModel.ExclusiveResidential = ExclusiveResidential.Count();
                    NotificationCountModel.ExclusiveCondo = ExclusiveResidentialCondo.Count();
                    NotificationCountModel.LookingForCommercial = LookingForCommercial.Count();
                    NotificationCountModel.LookingForResidential = LookingForResidential.Count();
                    NotificationCountModel.LookingForCondo = LookingForResidentialCondo.Count();
                    NotificationCountModel.NewHotCommercial = NewHotListingCommercial.Count();
                    NotificationCountModel.NewHotResidential = NewHotListingResidential.Count();
                    NotificationCountModel.NewHotCondo = NewHotListingResidentialCondo.Count();

                    return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("success", NotificationCountModel), Configuration.Formatters.JsonFormatter);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "No notification found."), Configuration.Formatters.JsonFormatter);
                }


            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.Message.ToString();
                ErrorLogging.LogError(ex);
                return Request.CreateResponse(HttpStatusCode.NotImplemented, CommonCls.CreateMessage("error", "Plese try later."), Configuration.Formatters.JsonFormatter);
            }
        }
        [Route("SetNotificationSound")]
        [HttpGet]
        public HttpResponseMessage SetNotificationSound([FromUri] int CustomerId, [FromUri] string Status)
        {
            try
            {
                if ((CustomerId == null) || (CustomerId == 0))
                {
                    return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "UserId is not found."), Configuration.Formatters.JsonFormatter);
                }
                if (Status == null || Status == "")
                {
                    return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Notification Sound value is null."), Configuration.Formatters.JsonFormatter);
                }
                if ((Status != "on") && (Status != "off"))
                {
                    return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "Notification Sound value  is wrong."), Configuration.Formatters.JsonFormatter);
                }
                var Customer = _CustomerService.GetCustomer(CustomerId);
                if (Customer != null)
                {
                    var Message = "";
                    if (Status == "on")
                    {
                        Customer.IsNotificationSoundOn = true;
                        Message = "Notification sound on.";
                    }
                    else
                    {
                        Customer.IsNotificationSoundOn = false;
                        Message = "Notification sound off.";
                    }


                    _CustomerService.UpdateCustomer(Customer);
                    return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("success", Message), Configuration.Formatters.JsonFormatter);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "UserId is not found."), Configuration.Formatters.JsonFormatter);
                }
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.Message.ToString();
                ErrorLogging.LogError(ex);
                return Request.CreateResponse(HttpStatusCode.NotImplemented, CommonCls.CreateMessage("error", "Please try later."), Configuration.Formatters.JsonFormatter);
            }
        }
        public void SendMailToAdmin(string UserName, string EmailAddress, PropertyModel PropertyType, string Subject, string Body)
        {
            try
            {

                string Logourl = CommonCls.GetURL() + "/images/EmailLogo.png";
                string Imageurl = CommonCls.GetURL() + "/images/EmailPic.png";

                // Send mail.
                MailMessage mail = new MailMessage();

                string FromEmailID = EmailAddress == "" ? WebConfigurationManager.AppSettings["FromEmailID"] : EmailAddress;
                string FromEmailPassword = WebConfigurationManager.AppSettings["FromEmailPassword"];
                string ToEmailID = WebConfigurationManager.AppSettings["ToEmailID"];
                SmtpClient smtpClient = new SmtpClient(ConfigurationManager.AppSettings["SmtpServer"]);
                int _Port = Convert.ToInt32(WebConfigurationManager.AppSettings["Port"].ToString());
                Boolean _UseDefaultCredentials = Convert.ToBoolean(WebConfigurationManager.AppSettings["UseDefaultCredentials"].ToString());
                Boolean _EnableSsl = Convert.ToBoolean(WebConfigurationManager.AppSettings["EnableSsl"].ToString());
                mail.To.Add(new MailAddress(ToEmailID));
                mail.From = new MailAddress(FromEmailID);
                mail.Subject = Subject;
                //string LogoPath = Common.GetURL() + "/images/logo.png";
                string msgbody = "";
                msgbody += "";
                msgbody += "<div>";
                msgbody += "<div style='float:left;width:100%;'>";
                msgbody += "<h2 style='float:left; width:100%; font-size:16px; font-family:arial; margin:0 0 10px;'>Dear Admin,</h2>";
                msgbody += "<div style='float:left;width:100%; margin:4px 0;'><p style='float:left; font-size:14px; font-family:arial; line-height:25px; margin:5px 11px 0 0;'>Property Type :</p> <span style='float:left; font-size:14px; font-family:arial; margin:10px 0 0 0;'><b>" + PropertyType.PropertyTypeStatus + "</b></span>";
                msgbody += "</div>";
                msgbody += "<p style='float:left;width:100%; font-size:14px; font-family:arial; line-height:25px; margin:0;'>" + Body + "";//
                msgbody += "</p>";
                msgbody += "<p style='float:left;width:100%; font-size:14px; font-family:arial; line-height:25px; margin:0;'>User Name: " + UserName + "";//
                msgbody += "</p>";
                msgbody += "<p style='float:left;width:100%; font-size:14px; font-family:arial; line-height:25px; margin:0;'>Treb Id: " + PropertyType.CustomerTrebId + "";//
                msgbody += "</p>";
                msgbody += "<span style='float:left; font-size:15px; font-family:arial; margin:12px 0 0 0; width:100%;'>Team</span>";
                msgbody += "<div style='width:auto; float:left;'>";
                msgbody += " <p style='font-family:arial; font-size:12px; font-weight:bold; margin:0px; padding:0px;'>Call: 416 844 5725 | 647 977 3268  </p>";
                msgbody += " <p style='font-family:arial; font-size:12px; font-weight:bold; margin:0px; padding:0px;'> Fax: 647 497 5646 </p>";
                msgbody += " <a style='color:red; width:100%; font-size:13px; float:left; margin:5px 0 0 0px; text-decoration:none; font-family:arial;' href='http://www.only4agents.com'>Web: www.only4agents.com</a>";
                msgbody += " <a style='color:red; width:100%;font-size:13px; float:left; margin:5px 0 0 0px; text-decoration:none; font-family:arial;' href='#'>Email: info@only4agents.com</a>";
                msgbody += " <a style='color:red; width:100%;font-size:13px; float:left; margin:5px 0 0 0px; text-decoration:none; font-family:arial;' href='http://app.only4agents.com/'>Click here to login: www.app.only4agents.com</a>";
                msgbody += " </div>";
                msgbody += "<div style='float:left; width:100%; margin:12px 0 6px 0;'>";
                //msgbody +="<img style='float:left; width:150px;' src='data:image/png;base64,"+ LogoBase64 +"' /> <img style='float:left; width:310px; margin:30px 0 0 30px;' src='data:image/png;base64,"+ ImageBase64 +"' />";
                msgbody += "<img style='float:left; width:500px;' src='" + Logourl + "' /> ";
                //<img style='float:left; width:310px; margin:30px 0 0 30px;' src='" + Imageurl + "' />
                msgbody += "</div>";
                msgbody += "</div>";
                msgbody += "</div>";
                mail.Body = msgbody;
                mail.IsBodyHtml = true;







                SmtpClient smtp = new SmtpClient();
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Host = "smtp.gmail.com"; //_Host;
                smtp.Port = _Port;
                //smtp.UseDefaultCredentials = _UseDefaultCredentials;
                smtp.Credentials = new System.Net.NetworkCredential(FromEmailID, FromEmailPassword);// Enter senders User name and password
                smtp.EnableSsl = _EnableSsl;
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
        }
        public void SendMailToUser(string UserName, string EmailAddress, PropertyModel PropertyType)
        {
            try
            {
                var ConcateData = "";
                if (PropertyType.Style != "" && PropertyType.Style != null)
                {
                    ConcateData += PropertyType.Style + ",";
                }
                if (PropertyType.PropertyType != "" && PropertyType.PropertyType != null)
                {
                    ConcateData += PropertyType.PropertyType + ",";
                }
                if (PropertyType.LocationPrefered != "" && PropertyType.LocationPrefered != null)
                {
                    ConcateData += PropertyType.LocationPrefered;
                }

                // Send mail.
                MailMessage mail = new MailMessage();
                string Logourl = CommonCls.GetURL() + "/images/EmailLogo.png";
                string Imageurl = CommonCls.GetURL() + "/images/EmailPic.png";

                string FromEmailID = WebConfigurationManager.AppSettings["FromEmailID"];
                string FromEmailPassword = WebConfigurationManager.AppSettings["FromEmailPassword"];
                string ToEmailID = WebConfigurationManager.AppSettings["ToEmailID"];
                SmtpClient smtpClient = new SmtpClient(ConfigurationManager.AppSettings["SmtpServer"]);
                int _Port = Convert.ToInt32(WebConfigurationManager.AppSettings["Port"].ToString());
                Boolean _UseDefaultCredentials = Convert.ToBoolean(WebConfigurationManager.AppSettings["UseDefaultCredentials"].ToString());
                Boolean _EnableSsl = Convert.ToBoolean(WebConfigurationManager.AppSettings["EnableSsl"].ToString());
                mail.To.Add(new MailAddress(EmailAddress));
                mail.From = new MailAddress(FromEmailID);
                mail.Subject = "Property successfully registered. ";
                //string LogoPath = Common.GetURL() + "/images/logo.png";
                string msgbody = "";
                msgbody += "";
                msgbody += " <div>";
                msgbody += " <div style='float:left;width:100%;'>";
                msgbody += "<h2 style='float:left; width:100%; font-size:16px; font-family:arial; margin:0 0 10px;'>Dear " + UserName + ",</h2>";
                msgbody += "<div style='float:left;width:100%; margin:4px 0;'><p style='float:left; font-size:14px; font-family:arial; line-height:25px; margin:0 11px 0 0;'>Treb Id:</p> <span style='float:left; font-size:14px; font-family:arial; margin:5px 0 0 0;'><b>" + PropertyType.CustomerTrebId + "<b></span>";
                msgbody += " </div>";
                msgbody += "<div style='float:left;width:100%; margin:4px 0;'><p style='float:left; font-size:14px; font-family:arial; line-height:25px; margin:0 11px 0 0;'>This message refers to your post under:</p> <span style='float:left; font-size:14px; font-family:arial; margin:5px 0 0 0;'><b>" + PropertyType.PropertyTypeStatus + "<b></span>";
                msgbody += " </div>";

                msgbody += "<p style='float:left;width:100%; font-size:14px; font-family:arial; line-height:25px; margin:0;'>Your post is under review, we will notify you once approved.</p>";

                msgbody += "<span style='float:left; font-size:15px; font-family:arial; margin:12px 0 0 0; width:100%;'>Team</span>";
                msgbody += "<div style='width:auto; float:left;'>";
                msgbody += " <p style='font-family:arial; font-size:12px; font-weight:bold; margin:0px; padding:0px;'>Call: 416 844 5725 | 647 977 3268  </p>";
                msgbody += " <p style='font-family:arial; font-size:12px; font-weight:bold; margin:0px; padding:0px;'> Fax: 647 497 5646 </p>";
                msgbody += " <a style='color:red; width:100%; font-size:13px; float:left; margin:5px 0 0 0px; text-decoration:none; font-family:arial;' href='http://www.only4agents.com'>Web: www.only4agents.com</a>";
                msgbody += " <a style='color:red; width:100%;font-size:13px; float:left; margin:5px 0 0 0px; text-decoration:none; font-family:arial;' href='#'>Email: info@only4agents.com</a>";
                msgbody += " </div>";
                msgbody += "<div style='float:left; width:100%; margin:5px 0;'>";
                msgbody += "<img style='float:left; width:500px;' src='" + Logourl + "' />";
                msgbody += "</div>";
                msgbody += "</div>";
                msgbody += "</div>";
                mail.Body = msgbody;
                mail.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient();
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Host = "smtp.gmail.com"; //_Host;
                smtp.Port = _Port;
                //smtp.UseDefaultCredentials = _UseDefaultCredentials;
                smtp.Credentials = new System.Net.NetworkCredential(FromEmailID, FromEmailPassword);// Enter senders User name and password
                smtp.EnableSsl = _EnableSsl;
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
        }
        public void SendMailToUpdateUser(string UserName, string EmailAddress, PropertyModel PropertyType)
        {
            try
            {
                var ConcateData = "";
                if (PropertyType.Style != "" && PropertyType.Style != null)
                {
                    ConcateData += PropertyType.Style + ",";
                }
                if (PropertyType.PropertyType != "" && PropertyType.PropertyType != null)
                {
                    ConcateData += PropertyType.PropertyType + ",";
                }
                if (PropertyType.LocationPrefered != "" && PropertyType.LocationPrefered != null)
                {
                    ConcateData += PropertyType.LocationPrefered;
                }
                // Send mail.
                MailMessage mail = new MailMessage();
                string Logourl = CommonCls.GetURL() + "/images/EmailLogo.png";
                string Imageurl = CommonCls.GetURL() + "/images/EmailPic.png";

                string FromEmailID = WebConfigurationManager.AppSettings["FromEmailID"];
                string FromEmailPassword = WebConfigurationManager.AppSettings["FromEmailPassword"];
                string ToEmailID = WebConfigurationManager.AppSettings["ToEmailID"];
                SmtpClient smtpClient = new SmtpClient(ConfigurationManager.AppSettings["SmtpServer"]);
                int _Port = Convert.ToInt32(WebConfigurationManager.AppSettings["Port"].ToString());
                Boolean _UseDefaultCredentials = Convert.ToBoolean(WebConfigurationManager.AppSettings["UseDefaultCredentials"].ToString());
                Boolean _EnableSsl = Convert.ToBoolean(WebConfigurationManager.AppSettings["EnableSsl"].ToString());
                mail.To.Add(new MailAddress(EmailAddress));
                mail.From = new MailAddress(FromEmailID);
                mail.Subject = "Property update - Pending";
                //string LogoPath = Common.GetURL() + "/images/logo.png";
                string msgbody = "";
                msgbody += "";
                msgbody += " <div>";
                msgbody += " <div style='float:left;width:100%;'>";
                msgbody += "<h2 style='float:left; width:100%; font-size:16px; font-family:arial; margin:0 0 10px;'>Dear " + UserName + ",</h2>";
                msgbody += "<div style='float:left;width:100%; margin:4px 0;'><p style='float:left; font-size:14px; font-family:arial; line-height:25px; margin:5px 11px 0 0;'>This message refers to your post:</p> <span style='float:left; font-size:14px; font-family:arial; margin:9px 0 0 0;'><b>" + PropertyType.PropertyTypeStatus + "</b></span>";
                msgbody += " </div>";
                msgbody += "<p style='float:left;width:100%; font-size:14px; font-family:arial; line-height:25px; margin:0;'>Your post is under review, we will notify you once approved.</p>";

                msgbody += "<div style='float:left; width:100%; margin:5px 0;'>";
                msgbody += "<span style='float:left; font-size:15px; font-family:arial; margin:12px 0 0 0; width:100%;'></span><span style='float:left; font-size:15px; font-family:arial; margin:8px 0 0 0; width:100%;'></span>";
                msgbody += " </div>";
                msgbody += "<div style='width:auto; float:left;'>";
                msgbody += " <p style='font-family:arial; font-size:12px; font-weight:bold; margin:0px; padding:0px;'>Call: 416 844 5725 | 647 977 3268 </p>";
                msgbody += " <p style='font-family:arial; font-size:12px; font-weight:bold; margin:0px; padding:0px;'>Fax: 647 497 5646 </p>";
                msgbody += " <a style='color:red; width:100%; font-size:13px; float:left; margin:5px 0 0 0px; text-decoration:none; font-family:arial;' href='http://www.only4agents.com'>Web: www.only4agents.com</a>";
                msgbody += " <a style='color:red; width:100%; font-size:13px; float:left; margin:5px 0 0 0px; text-decoration:none; font-family:arial;' href='#'>Email: info@only4agents.com</a>";
                msgbody += " </div>";
                msgbody += "<div style='float:left; width:100%; margin:5px 0;'>";
                //msgbody += "<img style='float:left; width:150px;' src='data:image/png;base64," + LogoBase64 + "' /> <img style='float:left; width:310px; margin:30px 0 0 30px;' src='data:image/png;base64," + ImageBase64 + "' />";
                msgbody += "<img style='float:left; width:500px;' src='" + Logourl + "' />";
                //<img style='float:left; width:310px; margin:30px 0 0 30px;' src='" + Imageurl + "' />


                msgbody += "</div>";
                msgbody += "</div>";
                msgbody += "</div>";
                mail.Body = msgbody;
                mail.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient();
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Host = "smtp.gmail.com"; //_Host;
                smtp.Port = _Port;
                //smtp.UseDefaultCredentials = _UseDefaultCredentials;
                smtp.Credentials = new System.Net.NetworkCredential(FromEmailID, FromEmailPassword);// Enter senders User name and password
                smtp.EnableSsl = _EnableSsl;
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
        }
        public PropertyPhotosAndCustomerModel GetPropertyDetail(Property Property)
        {
            PropertyPhotosAndCustomerModel PropertyPhotosAndCustomerModel = new Web.Models.PropertyPhotosAndCustomerModel();
            //Get customer data.
            var Customer = _CustomerService.GetCustomer(Convert.ToInt32(Property.CustomerId));
            PropertyPhotosAndCustomerModel.CustomerEmail = Customer.EmailId;
            PropertyPhotosAndCustomerModel.CustomerName = Customer.FirstName + " " + Customer.MiddleName + " " + Customer.LastName;
            PropertyPhotosAndCustomerModel.CustomerPhoneNo = Customer.MobileNo;



            if (Customer.IsActive == true && Customer.UpdateStatus == true)
            {
                PropertyPhotosAndCustomerModel.CustomerPhoto = Customer.PhotoPath;
            }
            else
            {
                PropertyPhotosAndCustomerModel.CustomerPhoto = ConfigurationManager.AppSettings["LiveURL"].ToString() + "/images/noImage.jpg"; ;
            }
            PropertyPhotosAndCustomerModel.WebsiteUrl = Customer.WebsiteUrl;
            //Get property photos.
            List<PropertyImages> PropertyImagesList = new List<Web.Models.PropertyImages>();
            var PropertyPhotoList = _PropertyImageService.GetPropertyImages().Where(c => c.PropertyId == Property.PropertyId);
            foreach (var Propertyphoto in PropertyPhotoList)
            {
                PropertyImages PropertyImages = new Web.Models.PropertyImages();
                PropertyImages.imagelist = Propertyphoto.ImagePath;
                if (Property.IsActive == false)
                {
                    PropertyImages.imagelist = ConfigurationManager.AppSettings["LiveURL"].ToString() + "/images/noImage.jpg";
                }
                PropertyImagesList.Add(PropertyImages);

            }
            PropertyPhotosAndCustomerModel.PropertyPhotolist = PropertyImagesList;
            return PropertyPhotosAndCustomerModel;

        }
        public bool CheckExpireDate(Property Property)
        {

            TimeSpan ts = DateTime.Now - Convert.ToDateTime(Property.CreatedOn);
            bool Status = false;

            // passing 'PropertyStatusId' to return model according to PropertyType.
            if (Property.PropertyStatusId != null)
            {
                if (Property.PropertyStatusId == (int)EnumValue.PropertySatus.ExclusiveCommercial && ts.Days < 30)
                {
                    Status = true;
                }
                else if (Property.PropertyStatusId == (int)EnumValue.PropertySatus.ExclusiveResidential && ts.Days < 30)
                {
                    Status = true;
                }
                else if (Property.PropertyStatusId == (int)EnumValue.PropertySatus.ExclusiveResidentialCondo && ts.Days < 30)
                {
                    Status = true;
                }
                else if (Property.PropertyStatusId == (int)EnumValue.PropertySatus.LookingForCommercial && ts.Days < 30)
                {
                    Status = true;
                }
                else if (Property.PropertyStatusId == (int)EnumValue.PropertySatus.LookingForResidential && ts.Days < 30)
                {
                    Status = true;
                }
                else if (Property.PropertyStatusId == (int)EnumValue.PropertySatus.LookingForResidentialCondo && ts.Days < 30)
                {
                    Status = true;
                }
                else if (Property.PropertyStatusId == (int)EnumValue.PropertySatus.NewHotListingCommercial && ts.Days < 30)
                {
                    Status = true;
                }
                else if (Property.PropertyStatusId == (int)EnumValue.PropertySatus.NewHotListingResidential && ts.Days < 30)
                {
                    Status = true;
                }
                else if (Property.PropertyStatusId == (int)EnumValue.PropertySatus.NewHotListingResidentialCondo && ts.Days <30)
                {
                    Status = true;
                }

            }


            return Status;

        }
        public string GetPropertyType(int? StatusId)
        {

            string PropertyType = "";
            if (StatusId == (int)EnumValue.PropertySatus.ExclusiveCommercial)
            {
                PropertyType = EnumValue.PropertySatus.ExclusiveCommercial.ToString();
            }
            else if (StatusId == (int)EnumValue.PropertySatus.ExclusiveResidential)
            {
                PropertyType = EnumValue.PropertySatus.ExclusiveResidential.ToString();
            }
            else if (StatusId == (int)EnumValue.PropertySatus.ExclusiveResidentialCondo)
            {
                PropertyType = EnumValue.PropertySatus.ExclusiveResidentialCondo.ToString();
            }
            else if (StatusId == (int)EnumValue.PropertySatus.LookingForCommercial)
            {
                PropertyType = EnumValue.PropertySatus.LookingForCommercial.ToString();
            }
            else if (StatusId == (int)EnumValue.PropertySatus.LookingForResidential)
            {
                PropertyType = EnumValue.PropertySatus.LookingForResidential.ToString();
            }
            else if (StatusId == (int)EnumValue.PropertySatus.LookingForResidentialCondo)
            {
                PropertyType = EnumValue.PropertySatus.LookingForResidentialCondo.ToString();
            }
            else if (StatusId == (int)EnumValue.PropertySatus.NewHotListingCommercial)
            {
                PropertyType = EnumValue.PropertySatus.NewHotListingCommercial.ToString();
            }
            else if (StatusId == (int)EnumValue.PropertySatus.NewHotListingResidential)
            {
                PropertyType = EnumValue.PropertySatus.NewHotListingResidential.ToString();
            }
            else if (StatusId == (int)EnumValue.PropertySatus.NewHotListingResidentialCondo)
            {
                PropertyType = EnumValue.PropertySatus.NewHotListingResidentialCondo.ToString();
            }
            return PropertyType;
        }
        public string SaveImage(string Base64String)
        {
            string fileName = Guid.NewGuid() + ".png";
            Image image = CommonCls.Base64ToImage(Base64String);
            var subPath = HttpContext.Current.Server.MapPath("~/PropertyPhoto");
            var path = Path.Combine(subPath, fileName);
            image.Save(path, System.Drawing.Imaging.ImageFormat.Png);

            string URL = CommonCls.GetURL() + "/PropertyPhoto/" + fileName;
            return URL;
        }
        public String ConvertImageURLToBase64(String url)
        {
            string Base64 = Convert.ToBase64String(File.ReadAllBytes(url));
            return Base64;
        }

        public void DeleteImage(string filePath)
        {
            try
            {
                var uri = new Uri(filePath);
                var fileName = Path.GetFileName(uri.AbsolutePath);
                var subPath = HttpContext.Current.Server.MapPath("~/PropertyPhoto");
                var path = Path.Combine(subPath, fileName);

                FileInfo file = new FileInfo(path);
                if (file.Exists)
                {
                    file.Delete();
                }
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.Message.ToString();
                ErrorLogging.LogError(ex);
            }
        }

    }
}