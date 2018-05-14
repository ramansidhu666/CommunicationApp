using AutoMapper;
using CommunicationApp.Controllers;
using CommunicationApp.Core.Infrastructure;
using CommunicationApp.Entity;
using CommunicationApp.Infrastructure;
using CommunicationApp.Models;
using CommunicationApp.Services;
using CommunicationApp.Web.Infrastructure.PushNotificationFile;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CommunicationApp.Web.Controllers
{
    public class EventController : BaseController
    {

        public ICompanyService _CompanyService { get; set; }
        public IPropertyService _PropertyService { get; set; }
        public IPropertyImageService _PropertyImageService { get; set; }
        public ICustomerService _CustomerService { get; set; }
        public IEventService _EventService { get; set; }
        public IEventCustomerService _EventCustomerService { get; set; }
        public INotification _Notification { get; set; }


        public EventController(INotification Notification, ICustomerService CustomerService, IEventCustomerService EventCustomerService, IEventService EventService, IUserService UserService, IRoleService RoleService, IFormService FormService, IRoleDetailService RoleDetailService, IRoleService _RoleService, IUserRoleService UserroleService, IPropertyImageService PropertyImageService, IPropertyService PropertyService, ICompanyService CompanyService)
            : base(CustomerService, UserService, RoleService, FormService, RoleDetailService, UserroleService)
        {
            this._CompanyService = CompanyService;
            this._PropertyService = PropertyService;
            this._PropertyImageService = PropertyImageService;
            this._CustomerService = CustomerService;
            this._EventService = EventService;
            this._EventCustomerService = EventCustomerService;//Notification
            this._Notification = Notification;//

        }
        //

         public string ConvertDate(string dateSt)
        {
            string[] DateArr = dateSt.Split('/');
            //From : 0-MM,1-DD,2-YYYY HH:MM:ss
            //From : 0-DD,1-MM,2-YYYY HH:MM:ss
            return DateArr[1] + "/" + DateArr[0] + "/" + DateArr[2];
        }

        private void CheckPermission()
        {
            RoleDetailModel roleDetail = UserPermission("event");
            TempData["Create"] = roleDetail.IsCreate;
            TempData["Delete"] = roleDetail.IsDelete;
            TempData["Detail"] = roleDetail.IsDetail;
            TempData["Edit"] = roleDetail.IsEdit;
            TempData["View"] = roleDetail.IsView;
        }
        // GET: /Event/
        public ActionResult Index(string StartDate, string EndDate, string Description, string FirstName)
        {
           
            List<EventModel> EventModelList = new List<EventModel>();
            if (!string.IsNullOrEmpty(StartDate))
            {
                if (CheckDate(StartDate) == false)
                {
                    TempData["ShowMessage"] = "error";
                    TempData["MessageBody"] = "Please select a valid From Date.";
                    return View(EventModelList);
                }
            }
            if (!string.IsNullOrEmpty(EndDate))
            {
                if (CheckDate(EndDate) == false)
                {
                    TempData["ShowMessage"] = "error";
                    TempData["MessageBody"] = "Please select a valid To Date.";
                    return View(EventModelList);
                }
            }

            if (!string.IsNullOrEmpty(StartDate) && !string.IsNullOrEmpty(EndDate))
            {
                if (Convert.ToDateTime(Convert.ToDateTime(StartDate).ToString("MM/dd/yyyy")) > Convert.ToDateTime(Convert.ToDateTime(EndDate).ToString("MM/dd/yyyy")))
                {
                    TempData["ShowMessage"] = "error";
                    TempData["MessageBody"] = "Please select a valid date range.";

                    return View(EventModelList);
                }

            }
            UserPermissionAction("event", RoleAction.view.ToString());
            CheckPermission();

            CultureInfo Culture = new CultureInfo("en-US");
            DateTime TodayDate = Convert.ToDateTime(Convert.ToDateTime(DateTime.Now).ToString("MM/dd/yyyy"), Culture);

            var Events = _EventService.GetEvents().Where(c => Convert.ToDateTime(Convert.ToDateTime(c.EventDate).ToString("MM/dd/yyyy"), Culture) >= TodayDate && c.IsActive == true).OrderBy(c => c.EventDate).ToList();
            //  var Events = _EventService.GetEvents().Where(c => c.IsActive == true).OrderByDescending(c => c.CreatedOn).ToList();

            //For Search By Name
            if (!string.IsNullOrEmpty(FirstName))
            {
                Events = Events.Where(c => c.EventName.ToLower().Contains(FirstName.ToLower().Trim())).ToList();
            }
            //For Search By Description
            if (!string.IsNullOrEmpty(Description))
            {
                Events = Events.Where(c => c.EventDescription.ToLower().Contains(Description.ToLower().Trim())).ToList();
            }
            //For Search By date
            if (!string.IsNullOrEmpty(StartDate) && !string.IsNullOrEmpty(EndDate) && StartDate.Trim() != "" && EndDate.Trim() != "")
            {
                CultureInfo culture = new CultureInfo("en-US");
                DateTime startDate = Convert.ToDateTime(Convert.ToDateTime(StartDate).ToString("MM/dd/yyyy"), culture);
                DateTime Enddate = Convert.ToDateTime(Convert.ToDateTime(EndDate).ToString("MM/dd/yyyy"), culture);
                Events = Events.Where(c => Convert.ToDateTime(Convert.ToDateTime(c.EventDate).ToString("MM/dd/yyyy"), culture) >= startDate && Convert.ToDateTime(Convert.ToDateTime(c.EventDate).ToString("MM/dd/yyyy"), culture) <= Enddate).ToList();//.Where(c => c.FirstName.ToLower().Contains(FirstName.ToLower()));
            }
            Mapper.CreateMap<CommunicationApp.Entity.Event, CommunicationApp.Models.EventModel>();
            foreach (var Event in Events)
            {
                CommunicationApp.Models.EventModel EventModel = Mapper.Map<CommunicationApp.Entity.Event, CommunicationApp.Models.EventModel>(Event);
                if (Event.EventImage == null || Event.EventImage == "")
                {
                    EventModel.EventImage = CommonCls.GetURL() + "/images/noImage.jpg";
                }
                EventModelList.Add(EventModel);
            }
            return View(EventModelList);
        }

        public ActionResult EventHistory(string StartDate, string EndDate, string Description, string FirstName)
        {
          
            List<EventModel> EventModelList = new List<EventModel>();
            if (!string.IsNullOrEmpty(StartDate))
            {
                if (CheckDate(StartDate) == false)
                {
                    TempData["ShowMessage"] = "error";
                    TempData["MessageBody"] = "Please select a valid From Date.";
                    return View(EventModelList);
                }
            }
            if (!string.IsNullOrEmpty(EndDate))
            {
                if (CheckDate(EndDate) == false)
                {
                    TempData["ShowMessage"] = "error";
                    TempData["MessageBody"] = "Please select a valid To Date.";
                    return View(EventModelList);
                }
            }

            if (!string.IsNullOrEmpty(StartDate) && !string.IsNullOrEmpty(EndDate))
            {
                if (Convert.ToDateTime(Convert.ToDateTime(StartDate).ToString("MM/dd/yyyy")) > Convert.ToDateTime(Convert.ToDateTime(EndDate).ToString("MM/dd/yyyy")))
                {
                    TempData["ShowMessage"] = "error";
                    TempData["MessageBody"] = "Please select a valid date range.";

                    return View(EventModelList);
                }

            }
            UserPermissionAction("event", RoleAction.view.ToString());
            CheckPermission();

            CultureInfo Culture = new CultureInfo("en-US");
            DateTime TodayDate = Convert.ToDateTime(Convert.ToDateTime(DateTime.Now).ToString("MM/dd/yyyy"), Culture);
            var Events = _EventService.GetEvents().Where(c => Convert.ToDateTime(Convert.ToDateTime(c.EventDate).ToString("MM/dd/yyyy"), Culture) < TodayDate && c.IsActive == true).OrderByDescending(c => c.CreatedOn).ToList();

            //For Search By Name
            if (!string.IsNullOrEmpty(FirstName))
            {
                Events = Events.Where(c => c.EventName.ToLower().Contains(FirstName.ToLower().Trim())).ToList();
            }
            //For Search By Description
            if (!string.IsNullOrEmpty(Description))
            {
                Events = Events.Where(c => c.EventDescription.ToLower().Contains(Description.ToLower().Trim())).ToList();
            }
            //For Search By date
            if (!string.IsNullOrEmpty(StartDate) && !string.IsNullOrEmpty(EndDate) && StartDate.Trim() != "" && EndDate.Trim() != "")
            {
                CultureInfo culture = new CultureInfo("en-US");
                DateTime startDate = Convert.ToDateTime(Convert.ToDateTime(StartDate).ToString("MM/dd/yyyy"), culture);
                DateTime Enddate = Convert.ToDateTime(Convert.ToDateTime(EndDate).ToString("MM/dd/yyyy"), culture);
                Events = Events.Where(c => Convert.ToDateTime(Convert.ToDateTime(c.EventDate).ToString("MM/dd/yyyy"), culture) >= startDate && Convert.ToDateTime(Convert.ToDateTime(c.EventDate).ToString("MM/dd/yyyy"), culture) <= Enddate).ToList();//.Where(c => c.FirstName.ToLower().Contains(FirstName.ToLower()));
            }
            Mapper.CreateMap<CommunicationApp.Entity.Event, CommunicationApp.Models.EventModel>();
            foreach (var Event in Events)
            {
                CommunicationApp.Models.EventModel EventModel = Mapper.Map<CommunicationApp.Entity.Event, CommunicationApp.Models.EventModel>(Event);
                if (Event.EventImage == null || Event.EventImage == "")
                {
                    EventModel.EventImage = CommonCls.GetURL() + "/images/noImage.jpg";
                }
                EventModelList.Add(EventModel);
            }
            return View(EventModelList);
        }

        public ActionResult Create()
        {
            EventModel EventModel = new EventModel();
            var CustomerList = _CustomerService.GetCustomers();
            EventModel.CustomersList = CustomerList.Select(x => new SelectListItem { Value = x.CustomerId.ToString(), Text = x.FirstName +" "+ x.LastName }).ToList();
            EventModel.CustomerId = 1;
            return View(EventModel);
        }



        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(EventModel EventModel, HttpPostedFileBase file)
        {
            UserPermissionAction("event", RoleAction.view.ToString());
            CheckPermission();
            TempData["ShowMessage"] = "";
            TempData["MessageBody"] = "";
            try
            {
                if (ModelState.IsValid)
                {
                    if (string.IsNullOrEmpty(EventModel.EventDate.ToString()))
                    {

                        TempData["ShowMessage"] = "error";
                        TempData["MessageBody"] = "Please select a valid Event Date.";
                        return View("Create", EventModel);
                    }

                    if (string.IsNullOrEmpty(EventModel.EventName))
                    {

                        TempData["ShowMessage"] = "error";
                        TempData["MessageBody"] = "Please Fill Event Name.";
                        return View("Create", EventModel);

                    }

                    if (string.IsNullOrEmpty(EventModel.EventDescription))
                    {
                        TempData["ShowMessage"] = "error";
                        TempData["MessageBody"] = "Please Fill  EventDescription.";
                        return View("Create", EventModel);
                    }
                    //CultureInfo culture = new CultureInfo("en-US");
                    //DateTime TodayDate = Convert.ToDateTime(DateTime.Now.ToString("MM/dd/yyyy"), culture);
                    //if (Convert.ToDateTime(EventModel.EventDate.ToString("MM/dd/yyyy"), culture) < TodayDate)
                    //{
                       
                    //    TempData["ShowMessage"] = "error";
                    //    TempData["MessageBody"] = "You cannot add old date event.";
                    //    return View("Create", EventModel);
                    //}
                   
                    Mapper.CreateMap<CommunicationApp.Models.EventModel, CommunicationApp.Entity.Event>();
                    CommunicationApp.Entity.Event Event = Mapper.Map<CommunicationApp.Models.EventModel, CommunicationApp.Entity.Event>(EventModel);
                    string EventImage = "";
                    if (file != null)
                    {

                        if (Event.EventImage != "")
                        {   //Delete Old Image
                            string pathDel = Server.MapPath("~/EventPhoto");

                            FileInfo objfile = new FileInfo(pathDel);
                            if (objfile.Exists) //check file exsit or not
                            {
                                objfile.Delete();
                            }
                            //End :Delete Old Image
                        }

                        //Save the photo in Folder
                        var fileExt = Path.GetExtension(file.FileName);
                        string fileName = Guid.NewGuid() + fileExt;
                        var subPath = Server.MapPath("~/EventPhoto");

                        //Check SubPath Exist or Not
                        if (!Directory.Exists(subPath))
                        {
                            Directory.CreateDirectory(subPath);
                        }
                        //End : Check SubPath Exist or Not

                        var path = Path.Combine(subPath, fileName);
                        file.SaveAs(path);

                        EventImage = CommonCls.GetURL() + "/EventPhoto/" + fileName;
                    }

                    Event.CustomerId = Convert.ToInt32(Session["CustomerId"]);//CutomerId
                    Event.IsActive = true;
                    Event.CreatedOn = DateTime.Now;
                    Event.EventImage = EventImage;
                    _EventService.InsertEvent(Event);                   
                    List<int> CustomerIds = new List<int>();
                    var Customers = new List<Customer>();
                    try
                    {
                        if (EventModel.All == true)
                        {
                            EventModel.SelectedCustomer = null;
                            Customers = _CustomerService.GetCustomers().ToList();
                            foreach (var Customer in Customers)
                            {
                                CustomerIds.Add(Convert.ToInt32(Customer.CustomerId));
                                EventCustomer EventCustomer = new Entity.EventCustomer();
                                EventCustomer.EventId = Event.EventId;
                                EventCustomer.CustomerId = Customer.CustomerId;
                                _EventCustomerService.InsertEventCustomer(EventCustomer);
                            }
                        }
                        else if (EventModel.SelectedCustomer != null)
                        {

                            foreach (var Customer in EventModel.SelectedCustomer)
                            {
                                CustomerIds.Add(Convert.ToInt32(Customer));
                                EventCustomer EventCustomer = new Entity.EventCustomer();
                                EventCustomer.EventId = Event.EventId;
                                EventCustomer.CustomerId = Event.CustomerId;
                                _EventCustomerService.InsertEventCustomer(EventCustomer);

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorLogging.LogError(ex);
                        throw;
                    }
                    

                


                    string Flag = "12";//status for Event;
                    var Message = "New event saved.";
                    //send notification
                    try
                    {
                        var CustomerList = _CustomerService.GetCustomers().Where(c => CustomerIds.Contains(c.CustomerId) && c.CustomerId != EventModel.CustomerId && c.IsActive == true).ToList();
                        foreach (var Customer in CustomerList)
                        {
                            if (Customer != null)
                            {
                                if (Customer.ApplicationId != null && Customer.ApplicationId != "")
                                {
                                    bool NotificationStatus = true;

                                    string JsonMessage = "{\"Flag\":\"" + Flag + "\",\"Message\":\"" + Message + "\"}";
                                    try
                                    {
                                        //Save Notification
                                        Notification Notification = new Notification();
                                        Notification.NotificationSendBy = 1;
                                        Notification.NotificationSendTo = Convert.ToInt32(Customer.CustomerId);
                                        Notification.IsRead = false;
                                        Notification.Flag = Convert.ToInt32(Flag);
                                        Notification.RequestMessage = Message;
                                        _Notification.InsertNotification(Notification);
                                        if (Customer.DeviceType == EnumValue.GetEnumDescription(EnumValue.DeviceType.Android))
                                        {
                                            CommonCls.SendGCM_Notifications(Customer.ApplicationId, JsonMessage, true);
                                        }
                                        else
                                        {

                                           
                                            int count = _Notification.GetNotifications().Where(c => c.NotificationSendTo == Convert.ToInt32(Customer.CustomerId) && c.IsRead == false).ToList().Count();
                                            //Dictionary<string, object> Dictionary = new Dictionary<string, object>();
                                            //Dictionary.Add("Flag", Flag);
                                            //Dictionary.Add("Message", Message);
                                            //NotificationStatus = PushNotificatinAlert.SendPushNotification(Customer.ApplicationId, Message, Flag, JsonMessage, Dictionary, 1, Convert.ToBoolean(Customer.IsNotificationSoundOn));
                                            CommonCls.TestSendFCM_Notifications(Customer.ApplicationId, JsonMessage, Message,count, true);
                                            ////Save Notification
                                            //Notification Notification = new Notification();
                                            //Notification.NotificationSendBy = 1;
                                            //Notification.NotificationSendTo = Customer.CustomerId;
                                            //Notification.IsRead = false;
                                            //Notification.RequestMessage = Message;
                                            //_Notification.InsertNotification(Notification);
                                        }
                                    }
                                    catch(Exception ex)
                                    {
                                        CommonCls.ErrorLog(ex.ToString());
                                    }
                                    
                                }

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorLogging.LogError(ex);
                        throw;
                    }
                
                    TempData["ShowMessage"] = "success";
                    TempData["MessageBody"] = "Event successfully saved.";
                    return RedirectToAction("Index");
                }
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Key, x.Value.Errors }).ToArray();
                var modelStateErrors = this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors);
                var CustomerList1 = _CustomerService.GetCustomers();
                EventModel.CustomersList = CustomerList1.Select(x => new SelectListItem { Value = x.CustomerId.ToString(), Text = x.FirstName }).ToList();
                EventModel.CustomerId = 1;
                return View(EventModel);
            }
            catch (Exception ex)
            {
                ErrorLogging.LogError(ex);

                return View(EventModel);
            }

        }


        [HttpGet]
        public ActionResult CancelEvent(int? id)
        {
            var EventId = id;
            UserPermissionAction("event", RoleAction.view.ToString());
            CheckPermission();
            TempData["ShowMessage"] = "";
            TempData["MessageBody"] = "";
            try
            {
                if (EventId != 0)
                {
                    var Event = _EventService.GetEvent(Convert.ToInt32(EventId));

                    if (Event != null)
                    {
                        Event.IsActive = false;
                        _EventService.UpdateEvent(Event);
                        TempData["ShowMessage"] = "success";
                        TempData["MessageBody"] = "Event cancel successfully.";
                    }
                    else
                    {
                        TempData["ShowMessage"] = "error";
                        TempData["MessageBody"] = "Event not found.";
                    }



                    return RedirectToAction("Index");
                }
                TempData["ShowMessage"] = "error";
                TempData["MessageBody"] = "No event found.";
                return View();
            }
            catch (Exception ex)
            {
                TempData["ShowMessage"] = "error";
                TempData["MessageBody"] = "";
                ErrorLogging.LogError(ex);
                return View();
            }

        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            var EventId = id;
            UserPermissionAction("event", RoleAction.view.ToString());
            CheckPermission();
            TempData["ShowMessage"] = "";
            TempData["MessageBody"] = "";
            try
            {
                if (EventId != 0)
                {
                    var Event = _EventService.GetEvent(Convert.ToInt32(EventId));
                    CultureInfo culture = new CultureInfo("en-US");
                    DateTime TodayDate = Convert.ToDateTime(DateTime.Now.ToString("MM/dd/yyyy"), culture);
                    if (Convert.ToDateTime(Event.EventDate.ToString("MM/dd/yyyy"), culture) < TodayDate)
                    {
                        TempData["ShowMessage"] = "error";
                        TempData["MessageBody"] = "You cannot edit old date event.";
                        return RedirectToAction("Index");
                    }
                    if (Event != null)
                    {
                        Event.IsActive = false;
                        _EventService.UpdateEvent(Event);
                    }
                    //Mapper.CreateMap<CommunicationApp.Entity.Event, CommunicationApp.Models.EventModel>();
                    //CommunicationApp.Models.EventModel eventModel = Mapper.Map<CommunicationApp.Entity.Event, CommunicationApp.Models.EventModel>(Event);
                    //if (Event.EventImage == null || Event.EventImage == "")
                    //{
                    //    eventModel.EventImage = Common.GetURL() + "/images/no-image-available.jpg";
                    //}
                    TempData["ShowMessage"] = "success";
                    TempData["MessageBody"] = "Event cancel successfully.";

                    return RedirectToAction("Index");
                }
                TempData["ShowMessage"] = "error";
                TempData["MessageBody"] = "No event found.";
                return View();
            }
            catch (Exception ex)
            {
                TempData["ShowMessage"] = "error";
                TempData["MessageBody"] = "";
                ErrorLogging.LogError(ex);
                return View();
            }

        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(EventModel EventModel, HttpPostedFileBase file)
        {
            UserPermissionAction("event", RoleAction.view.ToString());
            CheckPermission();
            TempData["ShowMessage"] = "";
            TempData["MessageBody"] = "";
            try
            {
                if (ModelState.IsValid)
                {
                    if (string.IsNullOrEmpty(EventModel.EventDate.ToString()))
                    {

                        TempData["ShowMessage"] = "error";
                        TempData["MessageBody"] = "Please select a valid Event Date.";
                        return View("Create", EventModel);
                    }

                    if (string.IsNullOrEmpty(EventModel.EventName))
                    {

                        TempData["ShowMessage"] = "error";
                        TempData["MessageBody"] = "Please Fill Event Name.";
                        return View("Create", EventModel);

                    }
                    if (string.IsNullOrEmpty(EventModel.EventDescription))
                    {
                        TempData["ShowMessage"] = "error";
                        TempData["MessageBody"] = "Please Fill  EventDescription.";
                        return View("Create", EventModel);
                    }
                    Mapper.CreateMap<CommunicationApp.Models.EventModel, CommunicationApp.Entity.Event>();
                    CommunicationApp.Entity.Event Event = Mapper.Map<CommunicationApp.Models.EventModel, CommunicationApp.Entity.Event>(EventModel);
                    var EventImage = "";
                    if (file != null)
                    {

                        if (Event.EventImage != "")
                        {   //Delete Old Image
                            string pathDel = Server.MapPath("~/EventPhoto");

                            FileInfo objfile = new FileInfo(pathDel);
                            if (objfile.Exists) //check file exsit or not
                            {
                                objfile.Delete();
                            }
                            //End :Delete Old Image
                        }

                        //Save the photo in Folder
                        var fileExt = Path.GetExtension(file.FileName);
                        string fileName = Guid.NewGuid() + fileExt;
                        var subPath = Server.MapPath("~/EventPhoto");

                        //Check SubPath Exist or Not
                        if (!Directory.Exists(subPath))
                        {
                            Directory.CreateDirectory(subPath);
                        }
                        //End : Check SubPath Exist or Not

                        var path = Path.Combine(subPath, fileName);
                        file.SaveAs(path);

                        EventImage = CommonCls.GetURL() + "/EventPhoto/" + fileName;
                    }
                    Event.LastUpdatedon = DateTime.Now;
                    Event.EventImage = EventImage;
                    _EventService.UpdateEvent(Event);
                    TempData["ShowMessage"] = "success";
                    TempData["MessageBody"] = "Event successfully updated.";
                    return RedirectToAction("Index");
                }
                return View(EventModel);
            }
            catch (Exception ex)
            {
                ErrorLogging.LogError(ex);

                return View(EventModel);
            }

        }
        public ActionResult Delete(int? id)
        {
            UserPermissionAction("event", RoleAction.view.ToString());
            CheckPermission();
            TempData["ShowMessage"] = "";
            TempData["MessageBody"] = "";
            var Id = Convert.ToInt32(id);
            try
            {
                var Event = _EventService.GetEvent(Id);
                if (Event != null)
                {

                    _EventService.DeleteEvent(Event);
                    TempData["ShowMessage"] = "success";
                    TempData["MessageBody"] = "Event is delete successfully.";
                    return RedirectToAction("EventHistory");
                }
            }
            catch (Exception ex)
            {
                ErrorLogging.LogError(ex);
                TempData["MessageBody"] = "No record is deleted.";
                return RedirectToAction("EventHistory");
            }



            return RedirectToAction("EventHistory");
        }

        protected bool CheckDate(String date)
        {
            try
            {
                DateTime dt = DateTime.Parse(date);
                return true;
            }
            catch
            {
                return false;
            }

        }

    }
}