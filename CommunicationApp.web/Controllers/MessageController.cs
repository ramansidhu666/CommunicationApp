using AutoMapper;
using CommunicationApp.Controllers;
using CommunicationApp.Infrastructure;
using CommunicationApp.Models;
using CommunicationApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommunicationApp.Web.Infrastructure.AsyncTask;
using System.IO;
using CommunicationApp.Entity;
namespace CommunicationApp.Web.Controllers
{
    public class MessageController : BaseController
    {
        public ICompanyService _CompanyService { get; set; }
        public IMessageService _MessageService { get; set; }
        public IMessageImageService _MessageImageService { get; set; }

        public MessageController(ICustomerService CustomerService, IUserService UserService, IFormService FormService, IRoleDetailService RoleDetailService, IRoleService RoleService, IUserRoleService UserRoleService, IMessageService MessageService, IMessageImageService MessageImageService)
            : base(CustomerService, UserService, RoleService, FormService, RoleDetailService, UserRoleService)
        {
            this._CustomerService = CustomerService;
            this._UserService = UserService;
            this._UserRoleService = UserRoleService;
            this._MessageService = MessageService;
            this._MessageImageService = MessageImageService;
        }
        private void CheckPermission()
        {
            RoleDetailModel roleDetail = UserPermission("message");
            TempData["Create"] = roleDetail.IsCreate;
            TempData["Delete"] = roleDetail.IsDelete;
            TempData["Detail"] = roleDetail.IsDetail;
            TempData["Edit"] = roleDetail.IsEdit;
            TempData["View"] = roleDetail.IsView;
        }
        public ActionResult Index(string StartDate, string EndDate, string Heading, string Message)
        {

            List<MessageModel> MessageModelList = new List<MessageModel>();
           
            UserPermissionAction("event", RoleAction.view.ToString());
            CheckPermission();
            var Messages = _MessageService.GetMessages().OrderByDescending(c => c.MessageId).ToList();

            //For Search By Name
            if (!string.IsNullOrEmpty(Message))
            {
                Messages = Messages.Where(c => c.Messages.ToLower().Contains(Message.ToLower().Trim())).ToList();
            }

            //For Search By Description
            if (!string.IsNullOrEmpty(Heading))
            {
                Messages = Messages.Where(c => c.Heading.ToLower().Contains(Heading.ToLower().Trim())).ToList();
            }

            Mapper.CreateMap<CommunicationApp.Entity.Message, CommunicationApp.Models.MessageModel>();
            foreach (var Msg in Messages)
            {
                MessageModel MessageModel = new CommunicationApp.Models.MessageModel();
                var _model = Mapper.Map<CommunicationApp.Entity.Message, CommunicationApp.Models.MessageModel>(Msg);
                var MessageImages = _MessageImageService.GetMessageImages().Where(c => c.MessageId == Msg.MessageId).ToList();
               List<MessageImageModel> MessageImageModelList=new List<MessageImageModel>();
                foreach (var MessageImage in MessageImages)
                {
                    MessageImageModel MessageImageModel = new MessageImageModel();
                    MessageImageModel.ImageUrl = MessageImage.ImageUrl;
                    MessageImageModelList.Add(MessageImageModel);
                }
                _model.ImageUrlList = MessageImageModelList;

                MessageModelList.Add(_model);
            }
            return View(MessageModelList);
        }
        //Create 
        [HttpGet]
        public ActionResult Create()
        {
            MessageModel MessageModel = new CommunicationApp.Models.MessageModel();
            //Check User Permission

            UserPermissionAction("message", RoleAction.create.ToString());
            CheckPermission();
            var AdminId = Convert.ToInt32(Session["CustomerID"]) == null ? 0 : Convert.ToInt32(Session["CustomerID"]);
            var CustomerList = _CustomerService.GetCustomers().Where(c => c.ParentId == AdminId);
            MessageModel.CustomersList = CustomerList.Select(x => new SelectListItem { Value = x.CustomerId.ToString(), Text = x.FirstName + " " + x.LastName }).AsEnumerable();
            return View(MessageModel);
        }
        [HttpPost]
        public ActionResult Create([Bind(Include = "MessageId,CustomerIds,Messages,Heading,CreatedOn,LastUpdatedOn,IsActive,SelectedCustomer")]MessageModel MessageModel, HttpPostedFileBase[] files)
        {
            UserPermissionAction("message", RoleAction.create.ToString());
            CheckPermission();
            string PhotoPath = "";
            try
            {
                int AdminId = Convert.ToInt32(Session["CustomerID"]) == null ? 0 : Convert.ToInt32(Session["CustomerID"]);
                TempData["ShowMessage"] = "";
                TempData["MessageBody"] = "";
                if (ModelState.IsValid)
                {
                    var Customerids = "";
                    Mapper.CreateMap<CommunicationApp.Models.MessageModel, CommunicationApp.Entity.Message>();
                    CommunicationApp.Entity.Message Message = Mapper.Map<CommunicationApp.Models.MessageModel, CommunicationApp.Entity.Message>(MessageModel);
                    if (MessageModel.SelectedCustomer != null)
                    {
                        Customerids = string.Join(",", MessageModel.SelectedCustomer);
                    }
                    else
                    {
                        var CustomerIdList = _CustomerService.GetCustomers().Select(c => c.CustomerId);
                        Customerids = string.Join(",", CustomerIdList);
                    }
                    if (Customerids != null && Customerids != "")
                    {
                        Message.CustomerIds = Customerids;
                    }
                    Message.CreatedOn = DateTime.Now;
                    Message.AdminId = AdminId;
                    Message.IsActive = true;

                    _MessageService.InsertMessage(Message);
                    if (files[0] != null)
                    {
                        foreach (HttpPostedFileBase file in files)
                        {
                            MessageImage MessageImage = new MessageImage();
                            MessageImage.MessageId = Message.MessageId; ;
                            MessageImage.ImageUrl = SaveFile(PhotoPath, file);
                            _MessageImageService.InsertMessageImage(MessageImage);

                        }
                    }
                    JobScheduler.SendMessageNotification(AdminId.ToString(), Customerids, Message.Messages, Message.Heading, Message.ImageUrl, MessageModel.IsWithImage.ToString());

                    TempData["ShowMessage"] = "success";
                    TempData["MessageBody"] = "message is saved successfully.";
                    return RedirectToAction("Index");
                }
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Key, x.Value.Errors }).ToArray();
                var modelStateErrors = this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors);



            }
            catch (Exception ex)
            {
                ErrorLogging.LogError(ex);

                TempData["ShowMessage"] = "error";
                TempData["MessageBody"] = "Some unknown problem occured while proccessing save operation on Tip.";

            }
            var CustomerList1 = _CustomerService.GetCustomers();
            MessageModel.CustomersList = CustomerList1.Select(x => new SelectListItem { Value = x.CustomerId.ToString(), Text = x.FirstName }).ToList();
            return View(MessageModel);
        }

        public ActionResult DeleteConfirmed(int id)
        {
            UserPermissionAction("message", RoleAction.delete.ToString());
            CheckPermission();
            var message = _MessageService.GetMessage(id);
            if (message != null)
            {

                try
                {
                    _MessageService.DeleteMessage(message);
                    TempData["ShowMessage"] = "success";
                    TempData["MessageBody"] = message.Heading + " is deleted successfully.";
                }
                catch (Exception ex)
                {
                    if (CommonClass.ErrorLog(ex.InnerException.InnerException.Message.ToString()) == "fk")
                    {
                        TempData["ShowMessage"] = "error";
                        TempData["MessageBody"] = "This   " + message.Heading + " is used in another pages.";
                    }
                    else
                    {
                        TempData["ShowMessage"] = "error";
                        TempData["MessageBody"] = "Some problem occured while proccessing delete operation on " + message.Heading + " .";
                    }
                }

            }

            return RedirectToAction("Index");
        }
        public string SaveFile(string PhotoPath, HttpPostedFileBase file)
        {
            if (PhotoPath != "")
            {   //Delete Old Image
                string pathDel = Server.MapPath("~/CustomerPhoto");

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
            var subPath = Server.MapPath("~/CustomerPhoto");

            //Check SubPath Exist or Not
            if (!Directory.Exists(subPath))
            {
                Directory.CreateDirectory(subPath);
            }
            //End : Check SubPath Exist or Not

            var path = Path.Combine(subPath, fileName);
            file.SaveAs(path);

            return CommonCls.GetURL() + "/CustomerPhoto/" + fileName;
        }
    }
}