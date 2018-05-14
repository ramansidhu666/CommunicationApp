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
using CommunicationApp.Models;
using AutoMapper;
using CommunicationApp.Infrastructure;
using CommunicationApp.Core.UtilityManager;
using CommunicationApp.Web.Models;

namespace CommunicationApp.Controllers.WebApi
{
    [RoutePrefix("Category")]
    public class CategoryApiController : ApiController
    {
        public ICategoryService _CategoryService { get; set; }
        public ISubCategoryService _SubCategoryService { get; set; }
        public IDivisionService _DivisionService { get; set; }
        public ISupplierService _SupplierService { get; set; }

        public CategoryApiController(ICategoryService CategoryService, ISubCategoryService SubCategoryService, IDivisionService DivisionService, ISupplierService SupplierService)
        {
            this._CategoryService = CategoryService;
            this._SubCategoryService = SubCategoryService;
            this._DivisionService = DivisionService;
            this._SupplierService = SupplierService;
        }

        //Get api/State/States
        [Route("GetAllCategory")]
        [HttpGet]
        public HttpResponseMessage GetAllCategory(int CategoryId)
        {

            try
            {

                Mapper.CreateMap<CommunicationApp.Entity.SubCategory, CommunicationApp.Models.SubCategoryModel>();
                var Subcategories = _SubCategoryService.GetSubCategories().Where(c => c.CategoryId == CategoryId).OrderBy(c => c.SubCategoryName);
                List<SubCategoryModel> SubCategoryModelList = new List<SubCategoryModel>();
                foreach (var subcategory in Subcategories)
                {

                    var Suppliers = _SupplierService.GetSuppliers();

                    if (subcategory.CategoryId != 0)
                    {
                        Suppliers = Suppliers.Where(c => c.SubCategoryId == subcategory.SubCategoryId).ToList();
                    }
                    subcategory.SubCategoryName = subcategory.SubCategoryName + "(" + Suppliers.Count+")";
                    SubCategoryModelList.Add(Mapper.Map<CommunicationApp.Entity.SubCategory, CommunicationApp.Models.SubCategoryModel>(subcategory));

                }
                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("success", SubCategoryModelList), Configuration.Formatters.JsonFormatter);
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "No record found."), Configuration.Formatters.JsonFormatter);
            }

        }


        [Route("GetAllDivision")]
        [HttpGet]
        public HttpResponseMessage GetAllDivision(int CategoryId, int SubCategoryId)
        {

            try
            {
                var Regions = _DivisionService.GetDivisions().Where(c => c.ParentId == null).OrderBy(c => c.DivisionName);
                List<DivisionModel> RegionModelList = new List<DivisionModel>();
                Mapper.CreateMap<CommunicationApp.Entity.Division, CommunicationApp.Models.DivisionModel>();
                foreach (var Region in Regions)
                {
                    var Suppliers = _SupplierService.GetSuppliers();

                    if (CategoryId != 0)
                    {
                        Suppliers = Suppliers.Where(c => c.CategoryId == CategoryId).ToList();
                    }
                    if (SubCategoryId != 0)
                    {
                        Suppliers = Suppliers.Where(c => c.SubCategoryId == SubCategoryId).ToList();
                    }
                    if (Region.DivisionId != 0)
                    {
                        //Suppliers = Suppliers.Where(c => c.Region.Contains(Region.DivisionId.ToString())).ToList();
                        Suppliers = Suppliers.Where(c => c.Region==Region.DivisionId.ToString()).ToList();
                    }
                    Region.DivisionName = Region.DivisionName + "(" + Suppliers.Count + ")";

                    var SubRegions = _DivisionService.GetDivisions().Where(c => c.ParentId == Region.DivisionId).OrderBy(c => c.DivisionName);
                    List<DivisionModel> SubRegionModelList = new List<DivisionModel>();
                    foreach (var SubRegion in SubRegions)
                    {
                        var SubRegionSuppliers = _SupplierService.GetSuppliers();
                        if (SubRegion.DivisionId != 0)
                        {
                            SubRegionSuppliers = SubRegionSuppliers.Where(c => c.SubRegion == SubRegion.DivisionId.ToString() && c.CategoryId == CategoryId && c.SubCategoryId == SubCategoryId).ToList();
                        }
                        SubRegion.DivisionName = SubRegion.DivisionName + "(" + SubRegionSuppliers.Count + ")";
                        SubRegionModelList.Add(Mapper.Map<CommunicationApp.Entity.Division, CommunicationApp.Models.DivisionModel>(SubRegion));

                    }

                    var _Regionmodel = Mapper.Map<CommunicationApp.Entity.Division, CommunicationApp.Models.DivisionModel>(Region);
                    _Regionmodel.SubRegionModel = SubRegionModelList;
                    RegionModelList.Add(_Regionmodel);
                }

                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("success", RegionModelList), Configuration.Formatters.JsonFormatter);
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.OK, CommonCls.CreateMessage("error", "No record found."), Configuration.Formatters.JsonFormatter);
            }

        }


    }
}