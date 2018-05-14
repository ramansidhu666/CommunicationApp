using AutoMapper;
using CommunicationApp.Controllers;
using CommunicationApp.Infrastructure;
using CommunicationApp.Models;
using CommunicationApp.Services;
using CommunicationApp.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CommunicationApp.Web.Controllers
{
    public class SupplierController : BaseController 
    {
        public ISubCategoryService _SubCategoryService { get; set; }
        public ICategoryService _CategoryService { get; set; }
        public ISupplierService _SupplierService { get; set; }
        public IDivisionService _DivisionService { get; set; }
        // GET: SubCategory

        public SupplierController(ICustomerService CustomerService, IUserService UserService, IRoleService RoleService, IUserRoleService UserRoleService, IFormService FormService, IRoleDetailService RoleDetailService, ISubCategoryService SubCategoryService, ICategoryService CategoryService, ISupplierService SupplierService, IDivisionService DivisionService)
            : base(CustomerService,UserService, RoleService, FormService, RoleDetailService, UserRoleService)
        {
            this._SubCategoryService = SubCategoryService;
            this._CategoryService = CategoryService;
            this._SupplierService = SupplierService;//
            this._DivisionService = DivisionService;//

        }
        private void CheckPermission()
        {
            RoleDetailModel roleDetail = UserPermission("supplier");
            TempData["Create"] = roleDetail.IsCreate;
            TempData["Delete"] = roleDetail.IsDelete;
            TempData["Detail"] = roleDetail.IsDetail;
            TempData["Edit"] = roleDetail.IsEdit;
            TempData["View"] = roleDetail.IsView;
        }
        public ActionResult Index(string FirstName, string MobileNo)
        {
            UserPermissionAction("supplier", RoleAction.create.ToString());
            CheckPermission();
            List<SupplierModel> model = new List<SupplierModel>();
            var Suppliers = _SupplierService.GetSuppliers();

            //For First Name
            if (!string.IsNullOrEmpty(FirstName))
            {
                Suppliers = Suppliers.Where(c => c.FirstName.ToLower().Contains(FirstName.ToLower())).ToList();
            }
            //For First Name
            if (!string.IsNullOrEmpty(MobileNo))
            {
                Suppliers = Suppliers.Where(c => c.MobileNo.ToLower().Contains(MobileNo.ToLower())).ToList();
            }
            Mapper.CreateMap<CommunicationApp.Entity.Supplier, CommunicationApp.Models.SupplierModel>();
            foreach(var supplier in Suppliers)
            {
                var Skill = _SubCategoryService.GetSubCategory(Convert.ToInt32(supplier.SubCategoryId));
                var models = Mapper.Map<CommunicationApp.Entity.Supplier, CommunicationApp.Models.SupplierModel>(supplier);
                if (Skill!=null)
                {
                    models.Skill = Skill.SubCategoryName;
                }
                model.Add(models);
            }
            return View(model.OrderByDescending(c=>c.SupplierId));
        }


        public ActionResult Create()
        {
            UserPermissionAction("supplier", RoleAction.create.ToString());
            CheckPermission();
            var Categories = _CategoryService.Categories();
            var SubCategories=_SubCategoryService.GetSubCategories();
            var CategoryIds = SubCategories.Where(c => c.CategoryId != null).ToList().Select(c => c.CategoryId);
            SupplierModel SupplierModel = new CommunicationApp.Models.SupplierModel();
            ViewBag.CategoryId = new SelectList(Categories.Where(c => CategoryIds.Contains(c.CategoryId)), "Categoryid", "CategoryName");
            ViewBag.SubCategoryId = new SelectList(SubCategories, "SubCategoryId", "SubCategoryName");
            ViewBag.Region = new SelectList(_DivisionService.GetDivisions(), "DivisionId", "DivisionName");

            return View(SupplierModel);
        }


        [HttpPost]
        public ActionResult Create([Bind(Include = "SupplierId,AdminId,FirstName,LastName,Address,Description,CategoryId,SubCategoryId,EmailID,photopath,MobileNo,Region,SubRegion,IsActive")] SupplierModel SupplierModel, HttpPostedFileBase file)
        {
            TempData["ShowMessage"] = "";
            TempData["MessageBody"] = "";
            UserPermissionAction("supplier", RoleAction.create.ToString());
            CheckPermission();
            Mapper.CreateMap<CommunicationApp.Models.SupplierModel, CommunicationApp.Entity.Supplier>();
            CommunicationApp.Entity.Supplier Supplier = Mapper.Map<CommunicationApp.Models.SupplierModel, CommunicationApp.Entity.Supplier>(SupplierModel);

            if (ModelState.IsValid)
            {
                var customerFound = _SupplierService.GetSuppliers().Where(x => x.MobileNo == Supplier.MobileNo ).FirstOrDefault();
                if (customerFound == null)
                {
                    if (file != null)
                    {
                        Supplier.Photopath = Savefile(file, Supplier.Photopath);
                    }
                        _SupplierService.InsertSupplier(Supplier);
                        TempData["ShowMessage"] = "Success";
                        TempData["MessageBody"] = "Admin successfully register.";

                }
                else
                {

                     if (customerFound.MobileNo == SupplierModel.MobileNo)
                    {
                        TempData["ShowMessage"] = "Error";
                        TempData["MessageBody"] = "MobileNos is already exist.";
                    }
                    else
                    {
                        TempData["ShowMessage"] = "Error";
                        TempData["MessageBody"] = "Some error occured.";
                    }

                }
            }
            else
            {
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Key, x.Value.Errors }).ToArray();
                var modelStateErrors = this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors);
                TempData["ShowMessage"] = "Error";
                TempData["MessageBody"] = "Please fill the required data.";
                ViewBag.CategoryId = new SelectList(_CategoryService.Categories(), "Categoryid", "CategoryName");
                ViewBag.SubCategoryId = new SelectList(_SubCategoryService.GetSubCategories(), "SubCategoryId", "SubCategoryName");
                ViewBag.Region = new SelectList(_DivisionService.GetDivisions(), "DivisionId", "DivisionName");
                return View(SupplierModel);
            }

            ViewBag.CategoryId = new SelectList(_CategoryService.Categories(), "Categoryid", "CategoryName");
            ViewBag.SubCategoryId = new SelectList(_SubCategoryService.GetSubCategories(), "SubCategoryId", "SubCategoryName");
            ViewBag.Region = new SelectList(_DivisionService.GetDivisions(), "DivisionId", "DivisionName");
            return RedirectToAction("Index");
        }
        public ActionResult Edit(int Id)
        {
            UserPermissionAction("supplier", RoleAction.create.ToString());
            CheckPermission();
            var Supplier = _SupplierService.GetSupplier(Id);
            Mapper.CreateMap<CommunicationApp.Entity.Supplier, CommunicationApp.Models.SupplierModel>();
            CommunicationApp.Models.SupplierModel SupplierModel = Mapper.Map<CommunicationApp.Entity.Supplier, CommunicationApp.Models.SupplierModel>(Supplier);

            ViewBag.CategoryIdList = new SelectList(_CategoryService.Categories(), "Categoryid", "CategoryName", SupplierModel.CategoryId);

            ViewBag.SubCategoryIdList = new SelectList(_SubCategoryService.GetSubCategories(), "SubCategoryId", "SubCategoryName", SupplierModel.SubCategoryId);
            ViewBag.RegionList = new SelectList(_DivisionService.GetDivisions(), "DivisionId", "DivisionName", SupplierModel.Region);
            ViewBag.SubRegionList = new SelectList(_DivisionService.GetDivisions(), "DivisionId", "DivisionName", SupplierModel.SubRegion);
            return View(SupplierModel);
        }
        [HttpPost]
        public ActionResult Edit([Bind(Include = "SupplierId,AdminId,FirstName,LastName,Address,Description,CategoryId,SubCategoryId,MobileNo,EmailID,Photopath,Region,SubRegion,IsActive")] SupplierModel SupplierModel, HttpPostedFileBase file)
        {
            TempData["ShowMessage"] = "";
            TempData["MessageBody"] = "";
            UserPermissionAction("supplier", RoleAction.create.ToString());
            CheckPermission();
            Mapper.CreateMap<CommunicationApp.Models.SupplierModel, CommunicationApp.Entity.Supplier>();
            CommunicationApp.Entity.Supplier Supplier = Mapper.Map<CommunicationApp.Models.SupplierModel, CommunicationApp.Entity.Supplier>(SupplierModel);
            var photoPath = "";
            if (ModelState.IsValid)
            {
                var customerFound = _SupplierService.GetSuppliers().Where(x => x.MobileNo == Supplier.MobileNo && x.SupplierId!=Supplier.SupplierId).FirstOrDefault();
                if (customerFound == null)
                {
                    if (Supplier.Photopath != null && Supplier.Photopath!="")
                    {
                        photoPath = Supplier.Photopath;
                    }
                        if (file != null)
                        {
                            Supplier.Photopath = Savefile(file, photoPath);
                        }
                   
                   
                    _SupplierService.UpdateSupplier(Supplier);
                    TempData["ShowMessage"] = "Success";
                    TempData["MessageBody"] = "Admin successfully updated.";

                }
                else
                {

                    if (customerFound.MobileNo == SupplierModel.MobileNo)
                    {
                        TempData["ShowMessage"] = "Error";
                        TempData["MessageBody"] = "MobileNos is already exist.";
                    }
                    else
                    {
                        TempData["ShowMessage"] = "Error";
                        TempData["MessageBody"] = "Some error occured.";
                    }

                }
            }
            else
            {
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Key, x.Value.Errors }).ToArray();
                var modelStateErrors = this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors);
                TempData["ShowMessage"] = "Error";
                TempData["MessageBody"] = "Please fill the required data.";
                ViewBag.CategoryId = new SelectList(_CategoryService.Categories(), "Categoryid", "CategoryName", SupplierModel.CategoryId);

                ViewBag.SubCategoryId = new SelectList(_SubCategoryService.GetSubCategories(), "SubCategoryId", "SubCategoryName", SupplierModel.SubCategoryId);
                return View(SupplierModel);
            }


            return RedirectToAction("Index");
        }
        public ActionResult Delete(int Id)
        {
            UserPermissionAction("supplier", RoleAction.create.ToString());
            CheckPermission();
            try
            {
                var Supplier = _SupplierService.GetSupplier(Id);
                if (Supplier != null)
                {
                   
                    _SupplierService.DeleteSupplier(Supplier);
                }
                TempData["ShowMessage"] = "Success";
                TempData["MessageBody"] = "Record successfully deleted.";
            }
            catch (Exception ex)
            {
                CommonCls.ErrorLog(ex.ToString());
                TempData["ShowMessage"] = "Error";
                TempData["MessageBody"] = "Please fill the required data.";
            }
           
            return RedirectToAction("Index");
        }
        public string Savefile(HttpPostedFileBase file,string Photpath)
        {
            if (Photpath != "" && Photpath !=null)
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


        public JsonResult GetSubRegion(string id)
        {
            Int32 DivisionId = (id == "" ? -1 : Convert.ToInt32(id));
            var SubDivisions = _DivisionService.GetDivisions().Where(x => x.ParentId == DivisionId).ToList();


            List<SelectListItem> SubdivisionList = new List<SelectListItem>();
            SubdivisionList.Add(new SelectListItem { Text = "<--Select SubRegion-->", Value = "" });
            foreach (var subdivision in SubDivisions)
            {
                SubdivisionList.Add(new SelectListItem { Text = subdivision.DivisionName, Value = subdivision.DivisionId.ToString() });
            }
            return Json(new SelectList(SubdivisionList, "Value", "Text"));
        }

	}
}