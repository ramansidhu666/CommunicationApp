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
using System.Data.Spatial;
using System.Spatial;
using CommunicationApp.Core.UtilityManager;
using System.Net.Mail;
using System.Web.Configuration;
using CommunicationApp.Web.Models;

namespace CommunicationApp.Controllers.WebApi
{
    [RoutePrefix("Property")]
    public class PropertyApiController : ApiController
    {
        public IUserService _UserService { get; set; }
        public IUserRoleService _UserRoleService { get; set; }
        public IPropertyService _PropertyService { get; set; }
        public IPropertyImageService _PropertyImageService { get; set; }
        public IOpenHouseService _OpenHouseService { get; set; }
        public ICustomerService _CustomerService { get; set; }
        public PropertyApiController(ICustomerService CustomerService, IOpenHouseService OpenHouseService, IPropertyImageService PropertyImageService, IPropertyService PropertyService, IUserService UserService, IUserRoleService UserRoleService)
        {
            this._PropertyService = PropertyService;
            this._UserService = UserService;
            this._UserRoleService = UserRoleService;
            this._PropertyImageService = PropertyImageService;
            this._OpenHouseService = OpenHouseService;
            this._CustomerService = CustomerService;
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
        public HttpResponseMessage GetAllProperty(string PropertyTypeStatus)
        {
            int PropertyStatusId = 0;
            try
            {
                if (PropertyTypeStatus == "" || PropertyTypeStatus == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, Common.CreateMessage("error", "Property Type Status cannot be blank."), Configuration.Formatters.JsonFormatter);
                }
                if (PropertyTypeStatus == EnumValue.GetEnumDescription(EnumValue.PropertySatus.ExclusiveCommercial))
                {
                    PropertyStatusId = (int)EnumValue.PropertySatus.ExclusiveCommercial;
                }
                else if (PropertyTypeStatus == EnumValue.GetEnumDescription(EnumValue.PropertySatus.ExclusiveResidential))
                {
                    PropertyStatusId = (int)EnumValue.PropertySatus.ExclusiveResidential;
                }
                else if (PropertyTypeStatus == EnumValue.GetEnumDescription(EnumValue.PropertySatus.NewHotListingCommercial))
                {
                    PropertyStatusId = (int)EnumValue.PropertySatus.NewHotListingCommercial;
                }
                else if (PropertyTypeStatus == EnumValue.GetEnumDescription(EnumValue.PropertySatus.NewHotListingResidential))
                {
                    PropertyStatusId = (int)EnumValue.PropertySatus.NewHotListingResidential;
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, Common.CreateMessage("error", "Property Type Status is incorrect."), Configuration.Formatters.JsonFormatter);
                }
                //int? PropertyTypeStatusValue = CheckPropertyStatus(PropertyTypeStatus);


                if (PropertyStatusId != 0)
                {
                    var Properties = _PropertyService.GetPropertys().Where(c => c.PropertyStatusId == PropertyStatusId).OrderByDescending(c=>c.PropertyId);
                    var models = new List<PropertyModel>();
                    Mapper.CreateMap<CommunicationApp.Entity.Property, CommunicationApp.Web.Models.PropertyModel>();
                    foreach (var Property in Properties)
                    {
                        PropertyModel PropertyModel = new Web.Models.PropertyModel();

                     
                    }

                    return Request.CreateResponse(HttpStatusCode.OK, Common.CreateMessage("success", models), Configuration.Formatters.JsonFormatter);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, Common.CreateMessage("error", "No record found"), Configuration.Formatters.JsonFormatter);

                }

            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.OK, Common.CreateMessage("error", "Please try later."), Configuration.Formatters.JsonFormatter);

            }

        }

        [Route("GetAllOpenHouse")]
        [HttpGet]
        public HttpResponseMessage GetAllOpenHouse()
        {
            try
            {
                var Properties = _OpenHouseService.GetOpenHouses().OrderByDescending(x=>x.OpenHouseId);
                var models = new List<OpenHouseModel>();
                Mapper.CreateMap<CommunicationApp.Entity.OpenHouse, CommunicationApp.Models.OpenHouseModel>();
                foreach (var OpenHouse in Properties)
                {

                    var _Model = Mapper.Map<CommunicationApp.Entity.OpenHouse, CommunicationApp.Models.OpenHouseModel>(OpenHouse);
                    var customer = _CustomerService.GetCustomer(Convert.ToInt32(OpenHouse.CustomerId));
                    _Model.CustomerName = customer.FirstName;
                    _Model.CustomerPhoto = customer.PhotoPath;
                    models.Add(_Model);
                }
                return Request.CreateResponse(HttpStatusCode.OK, Common.CreateMessage("success", models), Configuration.Formatters.JsonFormatter);
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.OK, Common.CreateMessage("error", "Please try later."), Configuration.Formatters.JsonFormatter);
            }
        }

        [Route("GetPropertyByID")]
        [HttpGet]
        public HttpResponseMessage GetPropertyByID([FromUri] int PropertyId)
        {
            try
            {
                if (PropertyId!=0)
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
                    PropertyModel.CustomerName = Customer.FirstName;
                    PropertyModel.CustomerPhoto = Customer.PhotoPath;
                    return Request.CreateResponse(HttpStatusCode.OK, Common.CreateMessage("success", PropertyModel), Configuration.Formatters.JsonFormatter);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, Common.CreateMessage("error", "Property id null."), Configuration.Formatters.JsonFormatter);

                }
               
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.Message.ToString();
                ErrorLogging.LogError(ex);
                return Request.CreateResponse(HttpStatusCode.OK, Common.CreateMessage("error", "Property not found."), Configuration.Formatters.JsonFormatter);
            }
        }
        [Route("SaveProperty")]
        [HttpPost]
        public HttpResponseMessage SaveProperty([FromBody]PropertyModel PropertyModel)
        {

            try
            {
                Mapper.CreateMap<CommunicationApp.Web.Models.PropertyModel, CommunicationApp.Entity.Property>();
                CommunicationApp.Entity.Property Property = Mapper.Map<CommunicationApp.Web.Models.PropertyModel, CommunicationApp.Entity.Property>(PropertyModel);
                if (PropertyModel.PropertyTypeStatus == "" || PropertyModel.PropertyTypeStatus == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, Common.CreateMessage("error", "Property Type Status cannot be blank."), Configuration.Formatters.JsonFormatter);
                }
                if (PropertyModel.PropertyTypeStatus == EnumValue.GetEnumDescription(EnumValue.PropertySatus.ExclusiveCommercial))
                {
                    Property.PropertyStatusId = (int)EnumValue.PropertySatus.ExclusiveCommercial;
                }
                else if (PropertyModel.PropertyTypeStatus == EnumValue.GetEnumDescription(EnumValue.PropertySatus.ExclusiveResidential))
                {
                    Property.PropertyStatusId = (int)EnumValue.PropertySatus.ExclusiveResidential;
                }
                else if (PropertyModel.PropertyTypeStatus == EnumValue.GetEnumDescription(EnumValue.PropertySatus.NewHotListingCommercial))
                {
                    Property.PropertyStatusId = (int)EnumValue.PropertySatus.NewHotListingCommercial;
                }
                else if (PropertyModel.PropertyTypeStatus == EnumValue.GetEnumDescription(EnumValue.PropertySatus.NewHotListingResidential))
                {
                    Property.PropertyStatusId = (int)EnumValue.PropertySatus.NewHotListingResidential;
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, Common.CreateMessage("error", "Property Type Status is incorrect."), Configuration.Formatters.JsonFormatter);
                }
                //int? PropertyTypeStatusValue = CheckPropertyStatus(PropertyModel.PropertyTypeStatus);
                _PropertyService.InsertProperty(Property);

                // Save Multiple Images
                if (PropertyModel.Imagelist[0] != null)
                {
                    if (PropertyModel.Imagelist[0].Contains("[{"))
                    {
                        string[] ImageListStr = PropertyModel.Imagelist[0].Replace("[", "").Replace("{", "").Replace("}", "").Replace("]", "").Replace(" ", "").Split(',');
                        foreach (var arr in ImageListStr)
                        {
                            PropertyImage propertyImage = new PropertyImage();
                            propertyImage.ImagePath = SaveImage(arr);
                            propertyImage.PropertyId = Property.PropertyId;
                            _PropertyImageService.InsertPropertyImage(propertyImage);
                        }
                    }
                    else
                    {
                        foreach (var arr in PropertyModel.Imagelist)
                        {
                            PropertyImage propertyImage = new PropertyImage();
                            propertyImage.ImagePath = SaveImage(arr);
                            propertyImage.PropertyId = Property.PropertyId;
                            _PropertyImageService.InsertPropertyImage(propertyImage);
                        }
                    }
                }




                return Request.CreateResponse(HttpStatusCode.OK, Common.CreateMessage("success", PropertyModel), Configuration.Formatters.JsonFormatter);

            }
            catch (Exception ex)
            {
                //var UserRole = _UserRoleService.GetUserRoles().Where(x => x.UserId == UserID).FirstOrDefault();
                //if (UserRole != null)
                //{
                //    _UserRoleService.DeleteUserRole(UserRole); // delete user role
                //}
                //var User = _UserService.GetUsers().Where(x => x.UserId == UserID).FirstOrDefault();
                //if (User != null)
                //{
                //    _UserService.DeleteUser(User); // delete user 
                //}
                string ErrorMsg = ex.Message.ToString();
                ErrorLogging.LogError(ex);
                return Request.CreateResponse(HttpStatusCode.OK, Common.CreateMessage("error", "Please try later."), Configuration.Formatters.JsonFormatter);
            }
        }

        [Route("SaveOpenHouse")]
        [HttpPost]
        public HttpResponseMessage SaveOpenHouse([FromBody]OpenHouseModel OpenHouseModel)
        {

            try
            {
                Mapper.CreateMap<CommunicationApp.Models.OpenHouseModel, CommunicationApp.Entity.OpenHouse>();
                CommunicationApp.Entity.OpenHouse OpenHouse = Mapper.Map<CommunicationApp.Models.OpenHouseModel, CommunicationApp.Entity.OpenHouse>(OpenHouseModel);
                if (OpenHouseModel.FromDateTimeStr != "" && OpenHouseModel.FromDateTimeStr != null)
                {
                    //Event.EventStartDate = Convert.ToDateTime(ConvertDate(eventModel.StartDate));
                    OpenHouse.FromDateTime = Convert.ToDateTime(OpenHouseModel.FromDateTimeStr);
                }
                if (OpenHouseModel.ToDateTimeStr != "" && OpenHouseModel.ToDateTimeStr != null)
                {
                    //Event.EventEndDate = Convert.ToDateTime(ConvertDate(eventModel.EndDate));
                    OpenHouse.ToDateTime = Convert.ToDateTime(OpenHouseModel.ToDateTimeStr);
                }
                OpenHouse.CompanyId = 1;
                OpenHouse.IsActive = true;
                _OpenHouseService.InsertOpenHouse(OpenHouse);


                return Request.CreateResponse(HttpStatusCode.OK, Common.CreateMessage("success", OpenHouseModel), Configuration.Formatters.JsonFormatter);

            }
            catch (Exception ex)
            {
                //var UserRole = _UserRoleService.GetUserRoles().Where(x => x.UserId == UserID).FirstOrDefault();
                //if (UserRole != null)
                //{
                //    _UserRoleService.DeleteUserRole(UserRole); // delete user role
                //}
                //var User = _UserService.GetUsers().Where(x => x.UserId == UserID).FirstOrDefault();
                //if (User != null)
                //{
                //    _UserService.DeleteUser(User); // delete user 
                //}
                string ErrorMsg = ex.Message.ToString();
                ErrorLogging.LogError(ex);
                return Request.CreateResponse(HttpStatusCode.OK, Common.CreateMessage("error", "Please try later."), Configuration.Formatters.JsonFormatter);
            }
        }



        [Route("DeleteProperty/{Propertyid}")]
        [HttpGet]
        public HttpResponseMessage DeleteProperty(int PropertyId)
        {
            try
            {
                //Delete from Property, It will delete from user, user role & Propertys
                var Property = _PropertyService.GetProperty(PropertyId);
                if (Property != null)
                {
                    _PropertyService.DeleteProperty(Property);
                    return Request.CreateResponse(HttpStatusCode.OK, Common.CreateMessage("success", "Property deleted successfully."), Configuration.Formatters.JsonFormatter);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, Common.CreateMessage("success", "No record found."), Configuration.Formatters.JsonFormatter);
                }

            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.Message.ToString();
                ErrorLogging.LogError(ex);
                return Request.CreateResponse(HttpStatusCode.OK, Common.CreateMessage("error", "Please try later."), Configuration.Formatters.JsonFormatter);
            }
        }

        //[Route("UpdateProfile")]
        //[HttpPost]
        //public HttpResponseMessage UpdateProfile([FromBody]UserPropertyModel userProperty)
        //{
        //    try
        //    {
        //        CommunicationApp.Entity.Property Property = _PropertyService.GetPropertys().Where(x => x.PropertyId == userProperty.PropertyId).FirstOrDefault();
        //        if (Property != null)
        //        {
        //            Property.FirstName = userProperty.Name;

        //            if (!userProperty.Image.Contains('.'))
        //            {
        //                if ((userProperty.Image != "") && (Property.PhotoPath != "") && (Property.PhotoPath != null))
        //                {
        //                    DeleteImage(Property.PhotoPath);
        //                }

        //                Property.PhotoPath = SaveImage(userProperty.Image);
        //            }
        //            _PropertyService.UpdateProperty(Property);

        //            return Request.CreateResponse(HttpStatusCode.OK, Common.CreateMessage("success", Property.PhotoPath), Configuration.Formatters.JsonFormatter);
        //        }
        //        else
        //        {
        //            return Request.CreateResponse(HttpStatusCode.OK, Common.CreateMessage("success", "RideId is not found."), Configuration.Formatters.JsonFormatter);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string ErrorMsg = ex.Message.ToString();
        //        ErrorLogging.LogError(ex);
        //        return Request.CreateResponse(HttpStatusCode.OK, Common.CreateMessage("error", "Please try later."), Configuration.Formatters.JsonFormatter);
        //    }
        //}


        public string SaveImage(string Base64String)
        {
            string fileName = Guid.NewGuid() + ".png";
            Image image = Common.Base64ToImage(Base64String);
            var subPath = HttpContext.Current.Server.MapPath("~/PropertyPhoto");
            var path = Path.Combine(subPath, fileName);
            image.Save(path, System.Drawing.Imaging.ImageFormat.Png);

            string URL = Common.GetURL() + "/PropertyPhoto/" + fileName;
            return URL;
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
        public void SendMailToUser(string UserName, string EmailAddress, string EmailVerifyCode)
        {
            try
            {
                // Send mail.
                MailMessage mail = new MailMessage();

                string FromEmailID = WebConfigurationManager.AppSettings["FromEmailID"];
                string FromEmailPassword = WebConfigurationManager.AppSettings["FromEmailPassword"];
                string ToEmailID = WebConfigurationManager.AppSettings["ToEmailID"];
                SmtpClient smtpClient = new SmtpClient(ConfigurationManager.AppSettings["SmtpServer"]);
                int _Port = Convert.ToInt32(WebConfigurationManager.AppSettings["Port"].ToString());
                Boolean _UseDefaultCredentials = Convert.ToBoolean(WebConfigurationManager.AppSettings["UseDefaultCredentials"].ToString());
                Boolean _EnableSsl = Convert.ToBoolean(WebConfigurationManager.AppSettings["EnableSsl"].ToString());
                mail.To.Add(new MailAddress(EmailAddress));
                mail.From = new MailAddress(FromEmailID);
                mail.Subject = "CommunicationApp-Verification Code";
                //string LogoPath = Common.GetURL() + "/images/logo.png";
                string msgbody = "";
                msgbody += "";
                msgbody += "<div style='margin: 0px; padding: 0px; overflow-y: auto; overflow-x: hidden;'>";
                msgbody += "    <div style='background: #ffffff; width: 100%; height: auto; float: left; margin: 0px; padding: 0px;'>";
                // msgbody += "        <div style='padding: 5px; float: left; text-align: left; margin-bottom: 10px;'><img width='100%' src='" + LogoPath + "' alt='Ghetty' title='Ghetty' /></div>";
                msgbody += "    </div>";
                msgbody += "    <span style='font-size: 15px; color: black; padding: 5px; margin: 0px; float: left;font-family: Gotham, Helvetica Neue, Helvetica, Arial, sans-serif; width: 20px;'>Hi </span>";
                msgbody += "    <span style='font-size: 15px; color: #3ab051; padding: 5px; margin: 0px;float: left; font-family: Gotham, Helvetica Neue, Helvetica, Arial, sans-serif;width: 20%;'>" + UserName + "</span>";
                msgbody += "    <span style='font-size: 15px; color: black; padding: 5px;margin: 10px 0 0 0px; float: left; font-family: Gotham, Helvetica Neue, Helvetica, Arial, sans-serif;width: 100%;'>You're almost done! </span>";
                msgbody += "    <span style='font-size: 15px; color: black;padding: 5px; margin: 10px 0 0 0px; float: left; font-family: Gotham, Helvetica Neue, Helvetica, Arial, sans-serif;width: 100%;'>To verify your email, just enter the four digit code below into yourapp. </span>";
                msgbody += "    <span style='font-size: 27px; color: black; padding: 5px; margin: 10px 0 0 0px;float: left; font-family: Gotham, Helvetica Neue, Helvetica, Arial, sans-serif;width: 100%;'>" + EmailVerifyCode + "</span>";
                msgbody += "    <span style='font-size: 15px; color: black; padding: 5px;margin: 10px 0 0 0px; float: left; font-family: Gotham, Helvetica Neue, Helvetica, Arial, sans-serif;width: 100%;'>From your friends at Ghetty. </span>";
                msgbody += "    <br />";
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
        //public int? CheckPropertyStatus(string PropertyTypeStatus)
        //{

        //    if (PropertyTypeStatus == EnumValue.GetEnumDescription(EnumValue.PropertySatus.ExclusiveCommercial))
        //    {
        //        Property.PropertyStatusId = (int)EnumValue.PropertySatus.ExclusiveCommercial;
        //    }
        //    else if (PropertyTypeStatus == EnumValue.GetEnumDescription(EnumValue.PropertySatus.ExclusiveResidential))
        //    {
        //        Property.PropertyStatusId = (int)EnumValue.PropertySatus.ExclusiveResidential;
        //    }
        //    else if (PropertyTypeStatus == EnumValue.GetEnumDescription(EnumValue.PropertySatus.NewHotListingCommercial))
        //    {
        //        Property.PropertyStatusId = (int)EnumValue.PropertySatus.NewHotListingCommercial;
        //    }
        //    else if (PropertyTypeStatus == EnumValue.GetEnumDescription(EnumValue.PropertySatus.NewHotListingResidential))
        //    {
        //        Property.PropertyStatusId = (int)EnumValue.PropertySatus.NewHotListingResidential;
        //    }
        //    return Property.PropertyStatusId;
        //    //else
        //    //{
        //    //    return Request.CreateResponse(HttpStatusCode.OK, Common.CreateMessage("error", "Property Type Status is incorrect."), Configuration.Formatters.JsonFormatter);
        //    //}
        //}
    }
}