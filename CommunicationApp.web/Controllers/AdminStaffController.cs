using AutoMapper;
using CommunicationApp.Controllers;
using CommunicationApp.Core.Infrastructure;
using CommunicationApp.Infrastructure;
using CommunicationApp.Models;
using CommunicationApp.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CommunicationApp.Web.Controllers
{
    public class AdminStaffController : BaseController
    {

        public IAdminStaffService _AdminStaffService { get; set; }



        public AdminStaffController(ICustomerService CustomerService, IUserService UserService, IFormService FormService, IRoleDetailService RoleDetailService, IRoleService RoleService, IUserRoleService UserRoleService, IAdminStaffService AdminStaffService)
            : base(CustomerService, UserService, RoleService, FormService, RoleDetailService, UserRoleService)
        {
            this._CustomerService = CustomerService;
            this._UserService = UserService;
            this._UserRoleService = UserRoleService;
            this._AdminStaffService = AdminStaffService;


        }
        private void CheckPermission()
        {
            RoleDetailModel roleDetail = UserPermission("adminstaff");
            TempData["Create"] = roleDetail.IsCreate;
            TempData["Delete"] = roleDetail.IsDelete;
            TempData["Detail"] = roleDetail.IsDetail;
            TempData["Edit"] = roleDetail.IsEdit;
            TempData["View"] = roleDetail.IsView;
        }
        public ActionResult Index()
        {
            var CustomerId = Convert.ToInt32(Session["CustomerId"]) == null ? 0 : Convert.ToInt32(Session["CustomerId"]);
            var model = new List<AdminStaffModel>();
            var AdminStaffs = _AdminStaffService.GetAdminStaffs().Where(c => c.CustomerId == CustomerId);
           // Mapper.CreateMap<CommunicationApp.Entity.Property, CommunicationApp.Web.Models.PropertyModel>();
            foreach (var AdminStaff in AdminStaffs)
            {
               // CommunicationApp.Models.AdminStaffModel AdminStaffModel = Mapper.Map<CommunicationApp.Entity.AdminStaff, CommunicationApp.Models.AdminStaffModel>(AdminStaff);
                AdminStaffModel AdminStaffModel = new CommunicationApp.Models.AdminStaffModel();
                AdminStaffModel.Address = AdminStaff.Address;
                AdminStaffModel.FirstName = AdminStaff.FirstName;
                AdminStaffModel.LastName = AdminStaff.LastName;
                AdminStaffModel.MobileNo = AdminStaff.MobileNo;
                AdminStaffModel.PhotoPath = AdminStaff.PhotoPath;
                AdminStaffModel.Designation = AdminStaff.Designation;
                AdminStaffModel.Latitude = AdminStaff.Latitude;
                AdminStaffModel.Longitude = AdminStaff.Longitude;
                AdminStaffModel.WebsiteUrl = AdminStaff.WebsiteUrl;
                AdminStaffModel.EmailId = AdminStaff.EmailId;
                AdminStaffModel.IsActive = AdminStaff.IsActive;
                AdminStaffModel.AdminStaffId = AdminStaff.AdminStaffId;
                model.Add(AdminStaffModel);
            }
            return View(model);
        }

        public ActionResult Create()
        {
            UserPermissionAction("adminStaff", RoleAction.create.ToString());
            CheckPermission();
            var CustomerId = Convert.ToInt32(Session["CustomerId"]) == null ? 0 : Convert.ToInt32(Session["CustomerId"]);
            AdminStaffModel AdminStaffModel = new CommunicationApp.Models.AdminStaffModel();
            AdminStaffModel.CustomerId = CustomerId;
            return View(AdminStaffModel);
        }

        [HttpPost]
        public ActionResult Create([Bind(Include = "AdminStaffId,CustomerId,WebsiteUrl,PhotoPath,FirstName,LastName,EmailID,MobileNo,ZipCode,Latitude,Longitude,Address,Designation,DOB,IsActive")] AdminStaffModel AdminStaffModel, HttpPostedFileBase file)
        {
            TempData["ShowMessage"] = "";
            TempData["MessageBody"] = "";
            UserPermissionAction("adminstaff", RoleAction.create.ToString());
            CheckPermission();
            Mapper.CreateMap<CommunicationApp.Models.AdminStaffModel, CommunicationApp.Entity.AdminStaff>();
            CommunicationApp.Entity.AdminStaff AdminStaff = Mapper.Map<CommunicationApp.Models.AdminStaffModel, CommunicationApp.Entity.AdminStaff>(AdminStaffModel);

            if (ModelState.IsValid)
            {

                if (AdminStaff != null)
                {

                    var PhotoPath = "";
                    if (file != null)
                    {

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

                        PhotoPath = CommonCls.GetURL() + "/CustomerPhoto/" + fileName;
                    }

                    AdminStaff.PhotoPath = PhotoPath;

                    if (AdminStaff.FirstName == null)
                    {
                        AdminStaff.FirstName = "";
                    }
                    if (AdminStaff.LastName == null)
                    {
                        AdminStaff.LastName = "";
                    }
                    var CustomerId = Convert.ToInt32(Session["CustomerId"]) == null ? 0 : Convert.ToInt32(Session["CustomerId"]);
                    AdminStaff.CustomerId = CustomerId;
                    _AdminStaffService.InsertAdminStaff(AdminStaff);
                    AdminStaffModel.CustomerId = AdminStaff.CustomerId;
                    TempData["ShowMessage"] = "Success";
                    TempData["MessageBody"] = "AdminStaff successfully saved.";

                }

            }
            else
            {
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Key, x.Value.Errors }).ToArray();
                var modelStateErrors = this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors);
                TempData["ShowMessage"] = "Error";
                TempData["MessageBody"] = "Please fill the required data.";
                return View(AdminStaffModel);
            }


            return RedirectToAction("Index");
        }
        public string Savefile(HttpPostedFileBase file)
        {
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
        // GET: /Customer/Edit/5
        public ActionResult Edit(int id)
        {
            AdminStaffModel AdminStaffModel = new CommunicationApp.Models.AdminStaffModel();
            var AdminStaff = _AdminStaffService.GetAdminStaff(id);
            if (AdminStaff != null)
            {
                var models = new List<AdminModel>();
                Mapper.CreateMap<CommunicationApp.Entity.AdminStaff, CommunicationApp.Models.AdminStaffModel>();
                AdminStaffModel = Mapper.Map<CommunicationApp.Entity.AdminStaff, CommunicationApp.Models.AdminStaffModel>(AdminStaff);
                if (AdminStaffModel.PhotoPath == null && AdminStaffModel.PhotoPath == "")
                {
                    AdminStaffModel.PhotoPath = CommonCls.GetURL() + "/images/noImage.jpg";
                }
              
            }

            return View(AdminStaffModel);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit([Bind(Include = "AdminStaffId,CustomerId,TrebId,WebsiteUrl,ApplicationID,Password,CompanyID,UserId,PhotoPath,FirstName,LastName,MiddleName,EmailID,MobileNo,Address,CountryID,StateID,CityID,ZipCode,Latitude,Longitude,CreatedOn,LastUpdatedOn,MobileVerifyCode,EmailVerifyCode,IsMobileVerified,IsEmailVerified,IsActive")] AdminStaffModel AdminStaffModel, HttpPostedFileBase file)
        {
            UserPermissionAction("property", RoleAction.view.ToString());
            CheckPermission();
            TempData["ShowMessage"] = "";
            TempData["MessageBody"] = "";
            try
            {

                if (ModelState.IsValid)
                {

                    var CustomerFound = _CustomerService.GetCustomers().Where(c => ((c.EmailId.Trim() == AdminStaffModel.EmailId.Trim() || c.MobileNo.Trim() == AdminStaffModel.MobileNo.Trim()) && c.CustomerId != AdminStaffModel.CustomerId)).FirstOrDefault();
                    if (CustomerFound == null)
                    {
                        var PhotoPath = "";
                        var AdminStaffUpdate = _AdminStaffService.GetAdminStaff(AdminStaffModel.AdminStaffId);//.Where(c => c.CustomerId == AdminModel.CustomerId).FirstOrDefault();
                        if (AdminStaffUpdate.PhotoPath != null && AdminStaffUpdate.PhotoPath!="")
                        {
                            PhotoPath = AdminStaffUpdate.PhotoPath;
                        }
                        if (AdminStaffUpdate != null)
                        {
                            // PhotoPath = CustomerUpdate.PhotoPath;
                            if (file != null)
                            {

                                if (AdminStaffUpdate.PhotoPath != "")
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

                                PhotoPath = CommonCls.GetURL() + "/CustomerPhoto/" + fileName;
                            }

                            AdminStaffUpdate.PhotoPath = PhotoPath;
                            AdminStaffUpdate.FirstName = AdminStaffModel.FirstName;
                            AdminStaffUpdate.LastName = AdminStaffModel.LastName;
                            AdminStaffUpdate.Address = AdminStaffModel.Address;
                            AdminStaffUpdate.EmailId = AdminStaffModel.EmailId;
                            AdminStaffUpdate.DOB = AdminStaffModel.DOB;
                            AdminStaffUpdate.WebsiteUrl = AdminStaffModel.WebsiteUrl;
                            AdminStaffUpdate.MobileNo = AdminStaffModel.MobileNo;
                            if (AdminStaffModel.IsActive==true)
                            {
                                AdminStaffUpdate.IsActive = true;
                            }
                            else
                            {
                                AdminStaffUpdate.IsActive = false;
                            }
                            
                            if (AdminStaffModel.Designation != null && AdminStaffModel.Designation != "")
                            {
                                AdminStaffUpdate.Designation = AdminStaffModel.Designation;
                            }
                            _AdminStaffService.UpdateAdminStaff(AdminStaffUpdate);
                            TempData["ShowMessage"] = "success";
                            TempData["MessageBody"] = AdminStaffUpdate.FirstName + " is update successfully.";
                            return RedirectToAction("Index", "AdminStaff");
                        }
                        else
                        {
                            TempData["ShowMessage"] = "error";
                            TempData["MessageBody"] = "User not found.";
                            return RedirectToAction("Index", "AdminStaff");
                        }

                    }
                    else
                    {
                        TempData["ShowMessage"] = "error";

                        if (CustomerFound.EmailId.Trim() == AdminStaffModel.EmailId.Trim())
                        {
                            TempData["MessageBody"] = AdminStaffModel.EmailId + " is already exists.";
                        }                       
                        if (CustomerFound.MobileNo.Trim() == AdminStaffModel.MobileNo.Trim())
                        {
                            TempData["MessageBody"] = "This" + " " + AdminStaffModel.MobileNo + " is already exists.";
                        }
                        else
                        {
                            TempData["MessageBody"] = "Please fill the required field with valid data";
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                CommonCls.ErrorLog(ex.ToString());
                TempData["ShowMessage"] = "error";
                TempData["MessageBody"] = "Some unknown problem occured while proccessing save operation on " + AdminStaffModel.FirstName + " client";

            }
            var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Key, x.Value.Errors }).ToArray();
            var modelStateErrors = this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors);

            return View(AdminStaffModel);
        }

        public string SaveImage(string Base64String)
        {
            string fileName = Guid.NewGuid() + ".png";
            Image image = CommonCls.Base64ToImage(Base64String);
            var subPath = Server.MapPath("~/CustomerPhoto");
            var path = Path.Combine(subPath, fileName);
            image.Save(path, System.Drawing.Imaging.ImageFormat.Png);

            string URL = CommonCls.GetURL() + "/CustomerPhoto/" + fileName;
            return URL;
        }
        public void DeleteImage(string filePath)
        {
            try
            {
                var uri = new Uri(filePath);
                var fileName = Path.GetFileName(uri.AbsolutePath);
                var subPath = Server.MapPath("~/CustomerPhoto");
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