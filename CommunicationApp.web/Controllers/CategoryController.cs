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
using CommunicationApp.Controllers;
using CommunicationApp.Services;
using CommunicationApp.Models;
using AutoMapper;
using CommunicationApp.Infrastructure;
using System.Data.Entity.Infrastructure;
using CommunicationApp.Web.Models;

namespace CommunicationApp.Web.Controllers
{
    public class CategoryController : BaseController
    {
       
        public ICategoryService _CategoryService { get; set; }
        public ISubCategoryService _SubCategoryService { get; set; }
        public ISupplierService _SupplierService { get; set; }

        public CategoryController(ICustomerService CustomerService, IUserService UserService, IRoleService RoleService, IUserRoleService UserRoleService, IFormService FormService, IRoleDetailService RoleDetailService, ICategoryService CategoryService, ISubCategoryService SubCategoryService, ISupplierService SupplierService)
            : base(CustomerService, UserService, RoleService, FormService, RoleDetailService, UserRoleService)
        {
            this._CategoryService = CategoryService;
            this._SubCategoryService = SubCategoryService;
            this._SupplierService = SupplierService;

        }

        // GET: Category
        public ActionResult Index()
        {
            //UserPermissionAction("category", RoleAction.create.ToString());
            //CheckPermission();

            var categories = _CategoryService.Categories().OrderBy(c => c.CategoryName);
            var models = new List<CategoryModel>();
            Mapper.CreateMap<CommunicationApp.Entity.Category, CommunicationApp.Models.CategoryModel>();
            foreach (var category in categories)
            {

                models.Add(Mapper.Map<CommunicationApp.Entity.Category, CommunicationApp.Models.CategoryModel>(category));
            }

            return View(models);
           
        }

        // GET: Category/Details/5
        public ActionResult Details(int id)
        {
            //UserPermissionAction("category", RoleAction.detail.ToString());
            //CheckPermission();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            Category category = _CategoryService.GetCategory(id);
            Mapper.CreateMap<CommunicationApp.Entity.Category, CommunicationApp.Models.CategoryModel>();
            CommunicationApp.Models.CategoryModel categorymodel = Mapper.Map<CommunicationApp.Entity.Category, CommunicationApp.Models.CategoryModel>(category);
            if (categorymodel == null)
            {
                return HttpNotFound();

            }
            return View(categorymodel);
        }

       
        public ActionResult Create()
        {
            //UserPermissionAction("category", RoleAction.create.ToString());
            //CheckPermission();
            CategoryModel CategoryModel = new CategoryModel();
            return View(CategoryModel);
        }

        
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Include = "CategoryId,CategoryName,IsActive")] CategoryModel categorymodel)
        {
            //UserPermissionAction("category", RoleAction.create.ToString());
            //CheckPermission();
            try
            {
                TempData["ShowMessage"] = "error";
                TempData["MessageBody"] = "Please fill the required field with valid data";

                if (ModelState.IsValid)
                {
                    Mapper.CreateMap<CommunicationApp.Models.CategoryModel, CommunicationApp.Entity.Category>();
                    CommunicationApp.Entity.Category Category = Mapper.Map<CommunicationApp.Models.CategoryModel, CommunicationApp.Entity.Category>(categorymodel);
                    Category Categorys = _CategoryService.Categories().Where(c => c.CategoryName.Trim().ToLower() == categorymodel.CategoryName.Trim().ToLower()).FirstOrDefault();
                    if (Categorys == null)
                    {

                        _CategoryService.InsertCategory(Category);
                        TempData["ShowMessage"] = "success";
                        TempData["MessageBody"] = Category.CategoryName + " is saved successfully.";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["ShowMessage"] = "error";
                        if (Categorys.CategoryName.Trim() == categorymodel.CategoryName.Trim())
                        {
                            TempData["MessageBody"] = categorymodel.CategoryName + " is already exists.";
                        }
                        else
                        {
                            TempData["MessageBody"] = "Please fill the required field with valid data";
                        }
                    }
                }
            }

            catch (RetryLimitExceededException)
            {
                TempData["ShowMessage"] = "error";
                TempData["MessageBody"] = "Some unknown problem occured while proccessing save operation on " + categorymodel.CategoryName + " ";
            }
            return View(categorymodel);
        }

        // GET: Category/Edit/5
        public ActionResult Edit(int id)
        {
            //UserPermissionAction("category", RoleAction.edit.ToString());
            //CheckPermission();
            if (id <= 0)
            {

                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var Categorys = _CategoryService.Categories().Where(c => c.CategoryId== id).FirstOrDefault();
            var models = new List<CategoryModel>();
            Mapper.CreateMap<CommunicationApp.Entity.Category, CommunicationApp.Models.CategoryModel>();
            CommunicationApp.Models.CategoryModel Categorymodel = Mapper.Map<CommunicationApp.Entity.Category, CommunicationApp.Models.CategoryModel>(Categorys);
            return View(Categorymodel);
        }

    
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CategoryId,CategoryName,IsActive")] CategoryModel categorymodel)
        {
            //UserPermissionAction("category", RoleAction.edit.ToString());
            //CheckPermission();
            try
            {
                Mapper.CreateMap<CommunicationApp.Models.CategoryModel, CommunicationApp.Entity.Category>();
                CommunicationApp.Entity.Category category = Mapper.Map<CommunicationApp.Models.CategoryModel, CommunicationApp.Entity.Category>(categorymodel);
                Category categorys = _CategoryService.Categories().Where(c => ((c.CategoryName == categorymodel.CategoryName.Trim()) && c.CategoryId != categorymodel.CategoryId)).FirstOrDefault();
                if (categorys == null)
                {
                    _CategoryService.UpdateCategory(category);
                    TempData["ShowMessage"] = "success";
                    TempData["MessageBody"] = category.CategoryName + " is update successfully.";
                    return RedirectToAction("Index");
                }

                else
                {
                    TempData["ShowMessage"] = "error";
                    if (categorys.CategoryName.Trim() == categorymodel.CategoryName.Trim())
                    {
                        TempData["MessageBody"] = categorymodel.CategoryName + " is already exists.";
                    }

                    else
                    {
                        TempData["MessageBody"] = "Please fill the required field with valid data";
                    }
                }
                
            }

            catch (RetryLimitExceededException)
            {
                TempData["ShowMessage"] = "error";
                TempData["MessageBody"] = "Some unknown problem occured while proccessing save operation on " + categorymodel.CategoryName + "category";

            }
            return View(categorymodel);
        }

        // GET: Category/Delete/5
        public ActionResult Delete(int id)
        {
            //UserPermissionAction("category", RoleAction.delete.ToString());
            //CheckPermission();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = _CategoryService.GetCategory(id); //db.tbCompanies.Find(id);
            Mapper.CreateMap<CommunicationApp.Entity.Company, CommunicationApp.Models.CompanyModel>();
            CommunicationApp.Models.CategoryModel categorymodel = Mapper.Map<CommunicationApp.Entity.Category, CommunicationApp.Models.CategoryModel>(category);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(categorymodel);
        }

        // POST: Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //UserPermissionAction("category", RoleAction.delete.ToString());
            //CheckPermission();
            string CategoryName = "";

            try
            {
                var CategoryIds=_CategoryService.Categories().Where(c=>c.CategoryId==id).Select(c=>c.CategoryId);
                var SubCategoryIds=_SubCategoryService.GetSubCategories().Where(c=>c.CategoryId==id).Select(c=>c.SubCategoryId);
                
                var suppliers=_SupplierService.GetSuppliers().Where(c=>SubCategoryIds.Contains(Convert.ToInt32(c.SubCategoryId)) ||CategoryIds.Contains(Convert.ToInt32(c.CategoryId)));                
                //Delete all supplier related to this category.
                foreach(var supplier in suppliers)
                {
                    _SupplierService.DeleteSupplier(supplier);
                }

                var SubCategories=_SubCategoryService.GetSubCategories().Where(c=>c.CategoryId==id);
                 foreach(var SubCategory in SubCategories)
                {
                    _SubCategoryService.DeleteSubCategory(SubCategory);
                }
                 Category category = _CategoryService.GetCategory(id);
                 if (category!=null)
                 {
                     CategoryName = category.CategoryName;
                     _CategoryService.DeleteCategory(category);
                     TempData["ShowMessage"] = "success";
                     TempData["MessageBody"] = CategoryName + " is deleted successfully.";
                 }

            }
            catch (Exception ex)
            {
                if (CommonClass.ErrorLog(ex.InnerException.InnerException.Message.ToString()) == "fk")
                {
                    TempData["ShowMessage"] = "error";
                    TempData["MessageBody"] = "This   " + CategoryName + " is used in another pages.";
                }
                else
                {
                    TempData["ShowMessage"] = "error";
                    TempData["MessageBody"] = "Some problem occured while proccessing delete operation on " + CategoryName + " Company.";
                }
            }
            return RedirectToAction("Index");  
        }
    }
}
    

