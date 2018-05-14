using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CommunicationApp.Data;
using CommunicationApp.Entity;
using CommunicationApp.Services;
using CommunicationApp.Controllers;
using CommunicationApp.Models;
using AutoMapper;
using CommunicationApp.Web.Models;



namespace CommunicationApp.Web.Controllers
{
    public class SubCategoryController : BaseController
    {
        public ISubCategoryService _SubCategoryService { get; set; }
        public ICategoryService _CategoryService { get; set; }
        public ISupplierService _SupplierService { get; set; }

        public SubCategoryController(ICustomerService CustomerService, IUserService UserService, IRoleService RoleService, IUserRoleService UserRoleService, IFormService FormService, IRoleDetailService RoleDetailService, ISubCategoryService SubCategoryService, ICategoryService CategoryService, ISupplierService SupplierService)
            : base(CustomerService, UserService, RoleService, FormService, RoleDetailService, UserRoleService)
        {
            this._SubCategoryService = SubCategoryService;
            this._CategoryService = CategoryService;
            this._SupplierService = SupplierService;

        }

        public ActionResult Index()
        {
            var subcategories = _SubCategoryService.GetSubCategories().OrderBy(c=>c.SubCategoryName);
            var models = new List<SubCategoryModel>();
            Mapper.CreateMap<CommunicationApp.Entity.SubCategory, CommunicationApp.Models.SubCategoryModel>();
            foreach (var subcategory in subcategories)
            {
                var Category = _CategoryService.GetCategory(subcategory.CategoryId);
                var _model = Mapper.Map<CommunicationApp.Entity.SubCategory, CommunicationApp.Models.SubCategoryModel>(subcategory);
                if (Category != null)
                {
                    _model.CategoryName = Category.CategoryName;
                }
                models.Add(_model);
            }

            return View(models);


        }


        // GET: SubCategory/Create
        public ActionResult Create()
        {
            SubCategoryModel SubCategoryModel = new CommunicationApp.Models.SubCategoryModel();
            ViewBag.CategoryId = new SelectList(_CategoryService.Categories(), "CategoryId", "CategoryName");
            return View(SubCategoryModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SubCategoryId,SubCategoryName,CategoryId,IsActive")] SubCategoryModel SubCategoryModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Mapper.CreateMap<CommunicationApp.Models.SubCategoryModel, CommunicationApp.Entity.SubCategory>();
                    CommunicationApp.Entity.SubCategory SubCategory = Mapper.Map<CommunicationApp.Models.SubCategoryModel, CommunicationApp.Entity.SubCategory>(SubCategoryModel);
                    _SubCategoryService.InsertSubCategory(SubCategory);
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                CommonClass.ErrorLog(ex.ToString());
            }
            ViewBag.CategoryId = new SelectList(_CategoryService.Categories(), "CategoryId", "CategoryName", SubCategoryModel.CategoryId);
            return View(SubCategoryModel);
        }

        // GET: SubCategory/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SubCategory subCategory = _SubCategoryService.GetSubCategory(Convert.ToInt32(id));
            Mapper.CreateMap<CommunicationApp.Entity.SubCategory, CommunicationApp.Models.SubCategoryModel>();
            CommunicationApp.Models.SubCategoryModel SubCategoryModel = Mapper.Map<CommunicationApp.Entity.SubCategory, CommunicationApp.Models.SubCategoryModel>(subCategory);
            if (subCategory == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryIdList = new SelectList(_CategoryService.Categories(), "CategoryId", "CategoryName", subCategory.CategoryId);
            return View(SubCategoryModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SubCategoryId,SubCategoryName,CategoryId,IsActive")] SubCategoryModel SubCategoryModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Mapper.CreateMap<CommunicationApp.Models.SubCategoryModel, CommunicationApp.Entity.SubCategory>();
                    CommunicationApp.Entity.SubCategory SubCategory = Mapper.Map<CommunicationApp.Models.SubCategoryModel, CommunicationApp.Entity.SubCategory>(SubCategoryModel);
                    _SubCategoryService.UpdateSubCategory(SubCategory);
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                CommonClass.ErrorLog(ex.ToString());
            }

            ViewBag.CategoryId = new SelectList(_SubCategoryService.GetSubCategories(), "CategoryId", "CategoryName", SubCategoryModel.CategoryId);
            return View(SubCategoryModel);
        }

        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                SubCategory subCategory = _SubCategoryService.GetSubCategory(Convert.ToInt32(id));
                if (subCategory != null)
                {

                    var supplier = _SupplierService.GetSuppliers().Where(c => c.SubCategoryId == subCategory.SubCategoryId).FirstOrDefault();
                    if (supplier != null)
                    {
                        TempData["ShowMessage"] = "success";
                        TempData["MessageBody"] = "Please delete related data from suppliers.";
                        return RedirectToAction("Index");
                    }
                    _SubCategoryService.DeleteSubCategory(subCategory);
                    TempData["ShowMessage"] = "success";
                    TempData["MessageBody"] = "Record successfully deleted.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                CommonClass.ErrorLog(ex.ToString());
                TempData["ShowMessage"] = "error";
                TempData["MessageBody"] = "Error has occurred.";
                return RedirectToAction("Index");
            }


            return RedirectToAction("Index");
        }

        public JsonResult GetSubCategories(string id)
        {
            Int32 CategoryId = (id == "" ? -1 : Convert.ToInt32(id));
            var subcategories = _SubCategoryService.GetSubCategories().Where(x => x.CategoryId == CategoryId).ToList();


            List<SelectListItem> SubcategoryList = new List<SelectListItem>();
            SubcategoryList.Add(new SelectListItem { Text = "<--Select Subcategory-->", Value = "" });
            foreach (var category in subcategories)
            {
                SubcategoryList.Add(new SelectListItem { Text = category.SubCategoryName, Value = category.SubCategoryId.ToString() });
            }
            return Json(new SelectList(SubcategoryList, "Value", "Text"));
        }
        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
    }
}
