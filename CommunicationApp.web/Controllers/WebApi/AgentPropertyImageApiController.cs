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
using System.Text.RegularExpressions;
using CommunicationApp.Core.UtilityManager;
using System.Globalization;
using System.Threading.Tasks;
using System.Text;
using System.Net.Http.Headers;
using System.Reflection;
using CommunicationApp.Web.Models;

namespace CommunicationApp.Web.Controllers.WebApi
{
    //[RoutePrefix("PropertyImage")]
    public class AgentPropertyImageApiController : ApiController
    {
        public ICustomerService _CustomerService { get; set; }
        public IPropertyService _PropertyService { get; set; }
        public IPropertyImageService _PropertyImageService { get; set; }
        public AgentPropertyImageApiController(IPropertyService PropertyService, ICustomerService CustomerService, IPropertyImageService PropertyImageService)
        {
            this._PropertyService = PropertyService;
            this._CustomerService = CustomerService;
            this._PropertyImageService = PropertyImageService;
        }


        public async Task<HttpResponseMessage> UploadPropertyAgentImages()
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            string root = HttpContext.Current.Server.MapPath("~/PropertyPhoto/temp");
            CustomMultipartFormDataStreamProvider provider = new CustomMultipartFormDataStreamProvider(root);

            try
            {
                StringBuilder sb = new StringBuilder(); // Holds the response body

                Type type = typeof(AgentModel); // Get type pointer
                AgentModel AgentModel = new AgentModel();

                // Read the form data and return an async task.
                CustomMultipartFormDataStreamProvider x = await Request.Content.ReadAsMultipartAsync(provider);

                int AgentId = 0;
                // This illustrates how to get the form data.
                foreach (var key in provider.FormData.AllKeys)
                {
                    if (key == "AgentId")
                    {
                        string propertyValue = provider.FormData.GetValues(key).FirstOrDefault();
                        if (propertyValue != null)
                        {
                            AgentId = Convert.ToInt32(propertyValue);
                            break;
                        }
                    }
                }

                //Delete all already exist images
                var PropertyImages = _PropertyImageService.GetPropertyImages().Where(c => c.AgentId == Convert.ToInt32(AgentId)).ToList();
                if (PropertyImages != null)
                {
                    foreach (var images in PropertyImages)
                    {
                        DeleteImage(images.ImagePath);
                    }
                }

                // Process the list of files found in the directory.
                string[] fileEntries = Directory.GetFiles(root);
                foreach (string fileName in fileEntries)
                {
                    var fileFound = provider.FileData.Where(c => c.Headers.ContentDisposition.FileName.Replace("\"", string.Empty) == Path.GetFileName(fileName)).FirstOrDefault();
                    if (fileFound != null)
                    {
                        //string NewFileName = Guid.NewGuid() + "_Property" + PropertyId.ToString() + Path.GetExtension(fileName);
                        string NewFileName = Guid.NewGuid() + Path.GetExtension(fileName);

                        var NewRoot = Path.Combine(HttpRuntime.AppDomainAppPath, "PropertyPhoto") + "\\" + NewFileName;
                        //string NewRoot = HttpContext.Current.Server.MapPath("~/PropertyPhoto") + "\\" + NewFileName;
                        System.IO.File.Move(fileName, NewRoot);
                        string URL = CommonCls.GetURL() + "/PropertyPhoto/" + NewFileName;
                        PropertyImage propertyImage = new PropertyImage();
                        propertyImage.ImagePath = URL;
                        propertyImage.PropertyId = null;
                        propertyImage.AgentId = AgentId;
                        _PropertyImageService.InsertPropertyImage(propertyImage);

                        sb.Append(URL);
                    }
                }
                return new HttpResponseMessage()
                {
                    //Content = new StringContent(sb.ToString())
                    Content = new StringContent("Success")
                };
            }
            catch (System.Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
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
//public class CustomMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
//{
//    public CustomMultipartFormDataStreamProvider(string path) : base(path) { }

//    public override string GetLocalFileName(HttpContentHeaders headers)
//    {
//        return headers.ContentDisposition.FileName.Replace("\"", string.Empty);
//    }
//}