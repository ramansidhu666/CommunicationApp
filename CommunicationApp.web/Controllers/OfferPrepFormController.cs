using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommunicationApp.Services;
using CommunicationApp.Web.Models;
using AutoMapper;
using CommunicationApp.Models;
using CommunicationApp.Infrastructure;
using CommunicationApp.Core.Infrastructure;
using System.Web.UI.WebControls;
using System.Net.Mail;
using System.Web.Configuration;
using System.Configuration;
using System.Data;
using CommunicationApp.Entity;
using System.Linq;
using CommunicationApp.Controllers;
using System.IO;
using System.Web.UI;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.html;

namespace CommunicationApp.Web.Controllers
{
    public class OfferPrepFormController : BaseController
    {

        public ICustomerService _CustomerService { get; set; }
        public IOfferPrepFormService _OfferPrepFormService { get; set; }
        public ILeaseFormService _LeaseFormService { get; set; }
        public IChattelsTypesServices _ChattelsTypesServices { get; set; }
        public IClauseTypeServices _ClauseTypeServices { get; set; }
        public IOfficeLocationService _OfficeLocationService { get; set; }


        public OfferPrepFormController(IAgentService AgentService, ICustomerService CustomerService, IUserService UserService, IRoleService RoleService, IFormService FormService, IRoleDetailService RoleDetailService, IRoleService _RoleService, IUserRoleService UserroleService, IOfferPrepFormService OfferPrepFormService, IChattelsTypesServices ChattelsTypesServices, IClauseTypeServices ClauseTypeServices, IOfficeLocationService OfficeLocationService, ILeaseFormService LeaseFormService)
            : base(CustomerService, UserService, RoleService, FormService, RoleDetailService, UserroleService)
        {

            this._CustomerService = CustomerService;
            this._OfferPrepFormService = OfferPrepFormService;
            this._LeaseFormService = LeaseFormService;
            this._ChattelsTypesServices = ChattelsTypesServices;
            this._ClauseTypeServices = ClauseTypeServices;
            this._OfficeLocationService = OfficeLocationService;

        }
        // GET: /Brokerage/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult OfferPrepForm(int CustomerId)
        {

            OfferPrepFormModel OfferPrepFormModel = new Web.Models.OfferPrepFormModel();
            List<ChattelsTypesModel> ChattelsTypesModelList = new List<ChattelsTypesModel>();
            List<ClausesTypeModel> ClausesTypeModelList = new List<ClausesTypeModel>();
            var ChattelsTypesServices = _ChattelsTypesServices.GetChattelsTypes();
            var ClauseTypeServices = _ClauseTypeServices.GetClauseTypes();
            Mapper.CreateMap<CommunicationApp.Entity.ChattelsTypes, CommunicationApp.Web.Models.ChattelsTypesModel>();
            foreach (var ChattelsTypesService in ChattelsTypesServices)
            {
                var _model = Mapper.Map<CommunicationApp.Entity.ChattelsTypes, CommunicationApp.Web.Models.ChattelsTypesModel>(ChattelsTypesService);
                ChattelsTypesModelList.Add(_model);
            }
            Mapper.CreateMap<CommunicationApp.Entity.ClauseType, CommunicationApp.Web.Models.ClausesTypeModel>();
            foreach (var ClauseTypeService in ClauseTypeServices)
            {
                var _model = Mapper.Map<CommunicationApp.Entity.ClauseType, CommunicationApp.Web.Models.ClausesTypeModel>(ClauseTypeService);
                var ids = ClouseIds();
                if (ids.Contains(_model.Id))
                {
                    _model.Ischeked = true;
                }
                else
                {
                    _model.Ischeked = false;
                }
                ClausesTypeModelList.Add(_model);
            }

            OfferPrepFormModel.ChattelsIds = ChattelsIds();
            OfferPrepFormModel.ChattelsTypesModelList = ChattelsTypesModelList;
            OfferPrepFormModel.ClausesTypeModelList = ClausesTypeModelList;
            ViewBag.EmailList = new SelectList(_OfficeLocationService.GetOfficeLocations(), "Email", "City");
            return View(OfferPrepFormModel);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult OfferPrepForm(OfferPrepFormModel model)
        {
            if (ModelState.IsValid)
            {
                OfferPrepForm OfferPrepForm_entity = new Entity.OfferPrepForm();
                Mapper.CreateMap<CommunicationApp.Web.Models.OfferPrepFormModel, CommunicationApp.Entity.OfferPrepForm>();
                OfferPrepForm_entity = Mapper.Map<CommunicationApp.Web.Models.OfferPrepFormModel, CommunicationApp.Entity.OfferPrepForm>(model);
                if (model.Chattelschk != null)
                {
                    OfferPrepForm_entity.ChattelsIncluded = String.Join(",", model.Chattelschk);
                }
                else
                {
                    TempData["Message"] ="must choose chattels";
                    return RedirectToAction("OfferPrepForm", new
                    {
                        CustomerId = model.CustomerId

                    }); 
                }

                if (model.Clauseschk != null)
                {
                    OfferPrepForm_entity.Clauses = String.Join(",", model.Clauseschk);
                }
                else
                {
                    TempData["Message"] = "must choose clause";
                    return RedirectToAction("OfferPrepForm", new
                    {
                        CustomerId = model.CustomerId

                    });
                }
                
               
               // OfferPrepForm_entity.ChattelsCount = model.StainlessSteelFridge + "," + model.Fridge=="null"?"":model.Fridge + "," + model.StainlessSteelStove + "," + model.Stove + "," + model.FrontLoadingWasher + "," + model.Washer + "," + model.FrontLoadingDryer + "," + model.Dryer;

              

                OfferPrepForm_entity.ChattelsCount = ((model.StainlessSteelFridge == "" || model.StainlessSteelFridge == null) ? "" : model.StainlessSteelFridge) + "," +( (model.Fridge == "" || model.Fridge == null) ? "" : model.Fridge) + "," + ((model.StainlessSteelStove == "" || model.StainlessSteelStove == null) ? "" : model.StainlessSteelStove) + "," +( (model.Stove == "" || model.Stove == null) ? "" : model.Stove) + "," + ((model.FrontLoadingWasher == "" || model.FrontLoadingWasher == null) ? "" : model.FrontLoadingWasher) + "," + ((model.Washer == "" || model.Washer == null) ? "" : model.Washer) + "," + ((model.FrontLoadingDryer == "" || model.FrontLoadingDryer == null) ? "" : model.FrontLoadingDryer) + "," + ((model.Dryer == "" || model.Dryer == null) ? "" : model.Dryer);
                
                if (OfferPrepForm_entity != null)
                {
                    _OfferPrepFormService.InsertOfferPrepForm(OfferPrepForm_entity);
                }
                ViewBag.EmailList = new SelectList(_OfficeLocationService.GetOfficeLocations(), "Email", "City");
                var UserDetail = _CustomerService.GetCustomer(OfferPrepForm_entity.CustomerId);
                if (UserDetail != null)
                {
                    string subject = "New Prep Form filled.";
                    string Body = "Please click the following link to watch the Prep Filled Form";
                    string FilledFormUrl = "http://communicationapp.only4agents.com/OfferPrepForm/OfferPrepFormDetail?OfferPrepFormId=" + OfferPrepForm_entity.Id;
                    SendOfferMailToAdmin(UserDetail.FirstName + "" + UserDetail.LastName, model.Email, subject, Body, UserDetail.TrebId, FilledFormUrl,UserDetail.EmailId);
                }


                return RedirectToAction("OfferPrepFormDetail", new
                {
                    OfferPrepFormId = OfferPrepForm_entity.Id

                });
            }
            return RedirectToAction("OfferPrepForm", new
            {
                CustomerId = model.CustomerId

            });
        }

        public ActionResult OfferPrepFormDetail(int OfferPrepFormId)
        {

            OfferPrepFormModel OfferPrepFormModel = new Web.Models.OfferPrepFormModel();
            List<ChattelsTypesModel> ChattelsTypesModelList = new List<ChattelsTypesModel>();
            List<ClausesTypeModel> ClausesTypeModelList = new List<ClausesTypeModel>();

            if (OfferPrepFormId != 0)
            {
                var chattelcount = 0;
                var Offerprepform = _OfferPrepFormService.GetOfferPrepForm(OfferPrepFormId);
                Mapper.CreateMap<CommunicationApp.Entity.OfferPrepForm, CommunicationApp.Web.Models.OfferPrepFormModel>();
                OfferPrepFormModel = Mapper.Map<CommunicationApp.Entity.OfferPrepForm, CommunicationApp.Web.Models.OfferPrepFormModel>(Offerprepform);
                OfferPrepFormModel.Email = _OfficeLocationService.GetOfficeLocations().Where(c => c.Email == OfferPrepFormModel.Email).FirstOrDefault().City;
                List<int> ChattelsIds = Offerprepform.ChattelsIncluded.Split(',').Select(int.Parse).ToList();
                var ChattelsTypesServices = _ChattelsTypesServices.GetChattelsTypes().Where(c => ChattelsIds.Contains(c.Id));
                Mapper.CreateMap<CommunicationApp.Entity.ChattelsTypes, CommunicationApp.Web.Models.ChattelsTypesModel>();
                var splitChattels = OfferPrepFormModel.ChattelsCount.Split(',').Where(c => c != "").ToArray();
                
                foreach (var ChattelsTypesService in ChattelsTypesServices)
                {

                    var _model = Mapper.Map<CommunicationApp.Entity.ChattelsTypes, CommunicationApp.Web.Models.ChattelsTypesModel>(ChattelsTypesService);
                    if (splitChattels.Count() > chattelcount)
                    {
                        _model.ChattelsCount = splitChattels[chattelcount];
                    }

                    ChattelsTypesModelList.Add(_model);
                    chattelcount += 1;
                }

                List<int> ClausesIds = Offerprepform.Clauses.Split(',').Select(int.Parse).ToList();
                var ClauseTypeServices = _ClauseTypeServices.GetClauseTypes().Where(c => ClausesIds.Contains(c.Id));
                Mapper.CreateMap<CommunicationApp.Entity.ClauseType, CommunicationApp.Web.Models.ClausesTypeModel>();
                foreach (var ClauseTypeService in ClauseTypeServices)
                {

                    var _model = Mapper.Map<CommunicationApp.Entity.ClauseType, CommunicationApp.Web.Models.ClausesTypeModel>(ClauseTypeService);
                    ClausesTypeModelList.Add(_model);
                }

                if (OfferPrepFormModel.AgreementofPurchaseandSale == ((int)EnumValue.AgreementofPurchaseandSale.TownHouseWithFee).ToString())
                {
                    OfferPrepFormModel.AgreementofPurchaseandSale = EnumValue.GetEnumDescription(EnumValue.AgreementofPurchaseandSale.TownHouseWithFee);
                }
                else if (OfferPrepFormModel.AgreementofPurchaseandSale == ((int)EnumValue.AgreementofPurchaseandSale.CONDO).ToString())
                {
                    OfferPrepFormModel.AgreementofPurchaseandSale = EnumValue.GetEnumDescription(EnumValue.AgreementofPurchaseandSale.CONDO);
                }
                else if (OfferPrepFormModel.AgreementofPurchaseandSale == ((int)EnumValue.AgreementofPurchaseandSale.FREEHOLD).ToString())
                {
                    OfferPrepFormModel.AgreementofPurchaseandSale = EnumValue.GetEnumDescription(EnumValue.AgreementofPurchaseandSale.FREEHOLD);
                }

                if (OfferPrepFormModel.Deposit == ((int)EnumValue.Deposit.HereWith).ToString())
                {
                    OfferPrepFormModel.Deposit = EnumValue.GetEnumDescription(EnumValue.Deposit.HereWith).ToString();
                }
                else if (OfferPrepFormModel.Deposit == ((int)EnumValue.Deposit.Uponacceptance).ToString())
                {
                    OfferPrepFormModel.Deposit = EnumValue.GetEnumDescription(EnumValue.Deposit.Uponacceptance).ToString();
                }


                if (OfferPrepFormModel.Arewethe == ((int)EnumValue.Arewethe.Co_OperatingBrokerage).ToString())
                {
                    OfferPrepFormModel.Arewethe = EnumValue.Arewethe.Co_OperatingBrokerage.ToString();
                }
                else if (OfferPrepFormModel.Arewethe == ((int)EnumValue.Arewethe.ListingBrokerageCoOperatingBrokerage).ToString())
                {
                    OfferPrepFormModel.Arewethe = EnumValue.Arewethe.ListingBrokerageCoOperatingBrokerage.ToString();
                }

                if (OfferPrepFormModel.FinalView_Option == ((int)EnumValue.FinalView_Option.One).ToString())
                {
                    OfferPrepFormModel.FinalView_Option = EnumValue.FinalView_Option.One.ToString();
                }
                else if (OfferPrepFormModel.FinalView_Option == ((int)EnumValue.FinalView_Option.TwoMoreTimes).ToString())
                {
                    OfferPrepFormModel.FinalView_Option = EnumValue.FinalView_Option.TwoMoreTimes.ToString();
                }



                //OfferPrepFormModel.StainlessSteelFridge = splitChattels[0];
                //OfferPrepFormModel.Fridge = splitChattels[1];
                //OfferPrepFormModel.StainlessSteelStove = splitChattels[2];
                //OfferPrepFormModel.Stove = splitChattels[3];
                //OfferPrepFormModel.FrontLoadingWasher = splitChattels[4];
                //OfferPrepFormModel.Washer = splitChattels[5];
                //OfferPrepFormModel.FrontLoadingDryer = splitChattels[6];
                //OfferPrepFormModel.Dryer = splitChattels[7];

            }

            OfferPrepFormModel.ChattelsTypesModelList = ChattelsTypesModelList;
            OfferPrepFormModel.ClausesTypeModelList = ClausesTypeModelList;
            ViewBag.EmailList = new SelectList(_OfficeLocationService.GetOfficeLocations(), "Email", "Email");
            return View(OfferPrepFormModel);
        }


        public ActionResult LeaseForm(int CustomerId)
        {

            LeaseFormModel LeaseFormModel = new Web.Models.LeaseFormModel();
            List<ChattelsTypesModel> ChattelsTypesModelList = new List<ChattelsTypesModel>();
            List<ClausesTypeModel> ClausesTypeModelList = new List<ClausesTypeModel>();
            var removeIds=RemoveChattleIds();
            var ChattelsTypesServices = _ChattelsTypesServices.GetChattelsTypes().Where(c => !removeIds.Contains(c.Id));
            var ClauseTypeServices = _ClauseTypeServices.GetClauseTypes();
            Mapper.CreateMap<CommunicationApp.Entity.ChattelsTypes, CommunicationApp.Web.Models.ChattelsTypesModel>();
            foreach (var ChattelsTypesService in ChattelsTypesServices)
            {
                var _model = Mapper.Map<CommunicationApp.Entity.ChattelsTypes, CommunicationApp.Web.Models.ChattelsTypesModel>(ChattelsTypesService);
                ChattelsTypesModelList.Add(_model);
            }
            Mapper.CreateMap<CommunicationApp.Entity.ClauseType, CommunicationApp.Web.Models.ClausesTypeModel>();
            //foreach (var ClauseTypeService in ClauseTypeServices)
            //{
            //    var _model = Mapper.Map<CommunicationApp.Entity.ClauseType, CommunicationApp.Web.Models.ClausesTypeModel>(ClauseTypeService);
            //    var ids = ClouseIds();
            //    if (ids.Contains(_model.Id))
            //    {
            //        _model.Ischeked = true;
            //    }
            //    else
            //    {
            //        _model.Ischeked = false;
            //    }
            //    ClausesTypeModelList.Add(_model);
            //}

            LeaseFormModel.ChattelsIds = ChattelsIds();
            LeaseFormModel.ChattelsTypesModelList = ChattelsTypesModelList;
            LeaseFormModel.ClausesTypeModelList = ClausesTypeModelList;
            ViewBag.EmailList = new SelectList(_OfficeLocationService.GetOfficeLocations(), "Email", "City");
            return View(LeaseFormModel);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult LeaseForm(LeaseFormModel model)
        {
            if (ModelState.IsValid)
            {
                LeaseForm LeaseForm_entity = new Entity.LeaseForm();
                Mapper.CreateMap<CommunicationApp.Web.Models.LeaseFormModel, CommunicationApp.Entity.LeaseForm>();
                LeaseForm_entity = Mapper.Map<CommunicationApp.Web.Models.LeaseFormModel, CommunicationApp.Entity.LeaseForm>(model);
                if (model.Chattelschk != null)
                {
                    LeaseForm_entity.ChattelsIncluded = String.Join(",", model.Chattelschk);
                }
                else
                {
                    TempData["Message"] = "must choose chattels";
                    return RedirectToAction("OfferPrepForm", new {CustomerId = model.CustomerId});
                }
                //if (model.Clauseschk != null)
                //{
                //    LeaseForm_entity.Clauses = String.Join(",", model.Clauseschk);
                //}
                //else
                //{
                //    TempData["Message"] = "must choose clause";
                //    return RedirectToAction("LeaseForm", new{CustomerId = model.CustomerId});
                //}
                LeaseForm_entity.ChattelsCount = ((model.StainlessSteelFridge == "" || model.StainlessSteelFridge == null) ? "" : model.StainlessSteelFridge) + "," + ((model.Fridge == "" || model.Fridge == null) ? "" : model.Fridge) + "," + ((model.StainlessSteelStove == "" || model.StainlessSteelStove == null) ? "" : model.StainlessSteelStove) + "," + ((model.Stove == "" || model.Stove == null) ? "" : model.Stove) + "," + ((model.FrontLoadingWasher == "" || model.FrontLoadingWasher == null) ? "" : model.FrontLoadingWasher) + "," + ((model.Washer == "" || model.Washer == null) ? "" : model.Washer) + "," + ((model.FrontLoadingDryer == "" || model.FrontLoadingDryer == null) ? "" : model.FrontLoadingDryer) + "," + ((model.Dryer == "" || model.Dryer == null) ? "" : model.Dryer);
                
               // LeaseForm_entity.ChattelsCount = model.StainlessSteelFridge + "," + model.Fridge + "," + model.StainlessSteelStove + "," + model.Stove + "," + model.FrontLoadingWasher + "," + model.Washer + "," + model.FrontLoadingDryer + "," + model.Dryer;
                if (LeaseForm_entity != null)
                {
                   
                    _LeaseFormService.InsertLeaseForm(LeaseForm_entity);
                }
                ViewBag.EmailList = new SelectList(_OfficeLocationService.GetOfficeLocations(), "Email", "City");
                var UserDetail = _CustomerService.GetCustomer(LeaseForm_entity.CustomerId);
                if (UserDetail != null)
                {
                    string subject = "New Prep Form filled.";
                    string Body = "Please click the following link to watch the Prep Filled Form";
                    string FilledFormUrl = "http://communicationapp.only4agents.com/OfferPrepForm/LeaseFormDetail?LeaseFormId=" + LeaseForm_entity.Id;
                    SendOfferMailToAdmin(UserDetail.FirstName + "" + UserDetail.LastName, model.Email, subject, Body, UserDetail.TrebId, FilledFormUrl,UserDetail.EmailId);
                }


                return RedirectToAction("LeaseFormDetail", new
                {
                    LeaseFormId = LeaseForm_entity.Id

                });
            }
            return RedirectToAction("LeaseForm", new
            {
                CustomerId = model.CustomerId

            });
        }

        public ActionResult LeaseFormDetail(int LeaseFormId)
        {

            LeaseFormModel LeaseFormModel = new Web.Models.LeaseFormModel();
            List<ChattelsTypesModel> ChattelsTypesModelList = new List<ChattelsTypesModel>();
            List<ClausesTypeModel> ClausesTypeModelList = new List<ClausesTypeModel>();

            if (LeaseFormId != 0)
            {
                var chattelcount = 0;
                var leaseform = _LeaseFormService.GetLeaseForm(LeaseFormId);
                Mapper.CreateMap<CommunicationApp.Entity.LeaseForm, CommunicationApp.Web.Models.LeaseFormModel>();
                LeaseFormModel = Mapper.Map<CommunicationApp.Entity.LeaseForm, CommunicationApp.Web.Models.LeaseFormModel>(leaseform);
                LeaseFormModel.Email = _OfficeLocationService.GetOfficeLocations().Where(c => c.Email == LeaseFormModel.Email).FirstOrDefault().City;
                List<int> ChattelsIds = leaseform.ChattelsIncluded.Split(',').Select(int.Parse).ToList();
                var ChattelsTypesServices = _ChattelsTypesServices.GetChattelsTypes().Where(c => ChattelsIds.Contains(c.Id));
                Mapper.CreateMap<CommunicationApp.Entity.ChattelsTypes, CommunicationApp.Web.Models.ChattelsTypesModel>();
                var splitChattels = LeaseFormModel.ChattelsCount.Split(',').Where(c => c != "").ToArray();;
                foreach (var ChattelsTypesService in ChattelsTypesServices)
                {
                    var _model = Mapper.Map<CommunicationApp.Entity.ChattelsTypes, CommunicationApp.Web.Models.ChattelsTypesModel>(ChattelsTypesService);
                    if (splitChattels.Count() > chattelcount)
                    {
                        _model.ChattelsCount = splitChattels[chattelcount];
                    }

                    ChattelsTypesModelList.Add(_model);
                    chattelcount += 1;
                }

                //List<int> ClausesIds = leaseform.Clauses.Split(',').Select(int.Parse).ToList();
                //var ClauseTypeServices = _ClauseTypeServices.GetClauseTypes().Where(c => ClausesIds.Contains(c.Id));
                Mapper.CreateMap<CommunicationApp.Entity.ClauseType, CommunicationApp.Web.Models.ClausesTypeModel>();
                //foreach (var ClauseTypeService in ClauseTypeServices)
                //{

                //    var _model = Mapper.Map<CommunicationApp.Entity.ClauseType, CommunicationApp.Web.Models.ClausesTypeModel>(ClauseTypeService);
                //    ClausesTypeModelList.Add(_model);
                //}

                if (LeaseFormModel.AgreementofPurchaseandSale == ((int)EnumValue.AgreementofPurchaseandSale.TownHouseWithFee).ToString())
                {
                    LeaseFormModel.AgreementofPurchaseandSale = EnumValue.GetEnumDescription(EnumValue.AgreementofPurchaseandSale.TownHouseWithFee);
                }
                else if (LeaseFormModel.AgreementofPurchaseandSale == ((int)EnumValue.AgreementofPurchaseandSale.CONDO).ToString())
                {
                    LeaseFormModel.AgreementofPurchaseandSale = EnumValue.GetEnumDescription(EnumValue.AgreementofPurchaseandSale.CONDO);
                }
                else if (LeaseFormModel.AgreementofPurchaseandSale == ((int)EnumValue.AgreementofPurchaseandSale.FREEHOLD).ToString())
                {
                    LeaseFormModel.AgreementofPurchaseandSale = EnumValue.GetEnumDescription(EnumValue.AgreementofPurchaseandSale.FREEHOLD);
                }

                if (LeaseFormModel.GarbageRemovalOrCondoFee == ((int)EnumValue.GarbageRemovalOrCondoFee.GarbageRemoval).ToString())
                {
                    LeaseFormModel.GarbageRemovalOrCondoFee = EnumValue.GetEnumDescription(EnumValue.GarbageRemovalOrCondoFee.GarbageRemoval);
                }
                else if (LeaseFormModel.GarbageRemovalOrCondoFee == ((int)EnumValue.GarbageRemovalOrCondoFee.CondoFee).ToString())
                {
                    LeaseFormModel.GarbageRemovalOrCondoFee = EnumValue.GetEnumDescription(EnumValue.GarbageRemovalOrCondoFee.CondoFee);
                }



                if (LeaseFormModel.Deposit == ((int)EnumValue.Deposit.HereWith).ToString())
                {
                    LeaseFormModel.Deposit = EnumValue.GetEnumDescription(EnumValue.Deposit.HereWith).ToString();
                }
                else if (LeaseFormModel.Deposit == ((int)EnumValue.Deposit.Uponacceptance).ToString())
                {
                    LeaseFormModel.Deposit = EnumValue.GetEnumDescription(EnumValue.Deposit.Uponacceptance).ToString();
                }


                if (LeaseFormModel.Arewethe == ((int)EnumValue.Arewethe.Co_OperatingBrokerage).ToString())
                {
                    LeaseFormModel.Arewethe = EnumValue.Arewethe.Co_OperatingBrokerage.ToString();
                }
                else if (LeaseFormModel.Arewethe == ((int)EnumValue.Arewethe.ListingBrokerageCoOperatingBrokerage).ToString())
                {
                    LeaseFormModel.Arewethe = EnumValue.Arewethe.ListingBrokerageCoOperatingBrokerage.ToString();
                }

                if (LeaseFormModel.FinalView_Option == ((int)EnumValue.FinalView_Option.One).ToString())
                {
                    LeaseFormModel.FinalView_Option = EnumValue.FinalView_Option.One.ToString();
                }
                else if (LeaseFormModel.FinalView_Option == ((int)EnumValue.FinalView_Option.TwoMoreTimes).ToString())
                {
                    LeaseFormModel.FinalView_Option = EnumValue.FinalView_Option.TwoMoreTimes.ToString();
                }


                //LeaseFormModel.StainlessSteelFridge = splitChattels[0];
                //LeaseFormModel.Fridge = splitChattels[1];
                //LeaseFormModel.StainlessSteelStove = splitChattels[2];
                //LeaseFormModel.Stove = splitChattels[3];
                //LeaseFormModel.FrontLoadingWasher = splitChattels[4];
                //LeaseFormModel.Washer = splitChattels[5];
                //LeaseFormModel.FrontLoadingDryer = splitChattels[6];
                //LeaseFormModel.Dryer = splitChattels[7];

            }

            LeaseFormModel.ChattelsTypesModelList = ChattelsTypesModelList;
            LeaseFormModel.ClausesTypeModelList = ClausesTypeModelList;
            ViewBag.EmailList = new SelectList(_OfficeLocationService.GetOfficeLocations(), "Email", "City");
            return View(LeaseFormModel);
        }

        public string ConvertDate(string dateSt)
        {
            string[] DateArr = dateSt.Split('/');
            int year = Convert.ToInt32(DateArr[2]);
            year += 1;
            return DateArr[0] + "/" + DateArr[1] + "/" + year.ToString();
        }
        public List<int> ClouseIds()
        {
            List<int> ids = new List<int>();
            ids.Add(1);
            ids.Add(2);
            ids.Add(3);
            ids.Add(7);
            ids.Add(8);
            ids.Add(9);
            ids.Add(10);
            ids.Add(11);
            ids.Add(12);
            ids.Add(1002);
            return ids;
        }
        public List<int> RemoveChattleIds()
        {
            List<int> ids = new List<int>();
            ids.Add(1002);
            ids.Add(1003);
            ids.Add(1004);
            ids.Add(1005);
            ids.Add(1006);
            ids.Add(1007);
            ids.Add(1010);
            ids.Add(1011);
            return ids;
        }
        public List<int> ChattelsIds()
        {
            List<int> ids = new List<int>();
            ids.Add(1);
            ids.Add(2);
            ids.Add(3);
            ids.Add(4);
            ids.Add(8);
            ids.Add(9);
            ids.Add(10);
            ids.Add(11);
            return ids;
        }
        #region Send mail to Brokerage
        public void SendMailToAdmin(string UserName, string EmailAddress, string subject, string Body, string TrebId, string FilledFormUrl)
        {
            try
            {

                string Logourl = CommonCls.GetURL() + "/images/EmailLogo.png";
                string Imageurl = CommonCls.GetURL() + "/images/EmailPic.png";

                // Send mail.
                MailMessage mail = new MailMessage();

                string FromEmailID = WebConfigurationManager.AppSettings["FromEmailID"];
                string FromEmailPassword = WebConfigurationManager.AppSettings["FromEmailPassword"];
                string ToEmailID = WebConfigurationManager.AppSettings["ToEmailID"];
                SmtpClient smtpClient = new SmtpClient(ConfigurationManager.AppSettings["SmtpServer"]);
                int _Port = Convert.ToInt32(WebConfigurationManager.AppSettings["Port"].ToString());
                Boolean _UseDefaultCredentials = Convert.ToBoolean(WebConfigurationManager.AppSettings["UseDefaultCredentials"].ToString());
                Boolean _EnableSsl = Convert.ToBoolean(WebConfigurationManager.AppSettings["EnableSsl"].ToString());
                mail.To.Add(new MailAddress("maninder.singh.2114@gmail.com"));
                mail.From = new MailAddress(FromEmailID);
                mail.Subject = subject;
                string msgbody = "";
                msgbody += "";
                msgbody += "<div>";
                msgbody += "<div style='float:left;width:100%;'>";
                msgbody += "<h2 style='float:left; width:100%; font-size:16px; font-family:arial; margin:0 0 10px;'>Dear Admin,</h2>";

                msgbody += "<p style='float:left;width:100%; font-size:14px; font-family:arial; line-height:25px; margin:0;'>" + Body + "";
                msgbody += "</p>";
                msgbody += "<p style='float:left;width:100%; font-size:14px; font-family:arial; line-height:25px; margin:0;'>User Name: <b>" + UserName + "</b>";
                msgbody += "<p style='float:left;width:100%; font-size:14px; font-family:arial; line-height:25px; margin:0;'>Treb Id: <b>" + TrebId + "</b>";
                msgbody += " <a style='color:red; width:100%;font-size:13px; float:left; margin:5px 0 0 0px; text-decoration:none; font-family:arial;' href='" + FilledFormUrl + "'>Click here to Check filled from: </a>";
                msgbody += "</p>";
                msgbody += "</p>";
                msgbody += "<span style='float:left; font-size:15px; font-family:arial; margin:12px 0 0 0; width:100%;'>Team</span>";
                msgbody += "<div style='width:auto; float:left;'>";
                msgbody += " <p style='font-family:arial; font-size:12px; font-weight:bold; margin:0px; padding:0px;'>Call: 416 844 5725 | 647 977 3268  </p>";
                msgbody += " <p style='font-family:arial; font-size:12px; font-weight:bold; margin:0px; padding:0px;'>Fax: 647 497 5646 </p>";
                msgbody += " <a style='color:red; width:100%; font-size:13px; float:left; margin:5px 0 0 0px; text-decoration:none; font-family:arial;' href='www.http://www.only4agents.com'>Web:www.only4agents.com</a>";
                msgbody += " <a style='color:red; width:100%;font-size:13px; float:left; margin:5px 0 0 0px; text-decoration:none; font-family:arial;' href='info@only4agents.com'>Email: info@only4agents.com</a>";
                msgbody += " <a style='color:red; width:100%;font-size:13px; float:left; margin:5px 0 0 0px; text-decoration:none; font-family:arial;' href='http://app.only4agents.com/'>Click here to login: www.app.only4agents.com/</a>";
                msgbody += " </div>";
                msgbody += "<div style='float:left; width:100%; margin:12px 0 6px 0;'>";
                msgbody += "<img style='float:left; width:500px;' src='" + Logourl + "' /> ";
                msgbody += "</div>";
                msgbody += "</div>";
                msgbody += "</div>";
                mail.Body = msgbody;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Host = "smtp.gmail.com";
                smtp.Port = _Port;
                smtp.Credentials = new System.Net.NetworkCredential(FromEmailID, FromEmailPassword);// Enter senders User name and password
                smtp.EnableSsl = _EnableSsl;
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
        }
        #endregion

        #region Send mail to Brokerage
        public void SendOfferMailToAdmin(string UserName, string EmailAddress, string subject, string Body, string TrebId, string FilledFormUrl,string EmailId)
        {
            try
            {

                string Logourl = CommonCls.GetURL() + "/images/EmailLogo.png";
                string Imageurl = CommonCls.GetURL() + "/images/EmailPic.png";

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
                mail.CC.Add(new MailAddress(ToEmailID));
                mail.To.Add(new MailAddress(EmailId));
                mail.From = new MailAddress(FromEmailID);
                mail.Subject = subject;
                string msgbody = "";
                msgbody += "";
                msgbody += "<div>";
                msgbody += "<div style='float:left;width:100%;'>";
                msgbody += "<h2 style='float:left; width:100%; font-size:16px; font-family:arial; margin:0 0 10px;'>Dear Admin,</h2>";

                msgbody += "<p style='float:left;width:100%; font-size:14px; font-family:arial; line-height:25px; margin:0;'>" + Body + "";
                msgbody += "</p>";
                msgbody += "<p style='float:left;width:100%; font-size:14px; font-family:arial; line-height:25px; margin:0;'>User Name: <b>" + UserName + "</b>";
                msgbody += "<p style='float:left;width:100%; font-size:14px; font-family:arial; line-height:25px; margin:0;'>Treb Id: <b>" + TrebId + "</b>";
                msgbody += " <a style='color:red; width:100%;font-size:13px; float:left; margin:5px 0 0 0px; text-decoration:none; font-family:arial;' href='" + FilledFormUrl + "'>Click here to Check filled from: </a>";
                msgbody += "</p>";
                msgbody += "</p>";
                msgbody += "<span style='float:left; font-size:15px; font-family:arial; margin:12px 0 0 0; width:100%;'>Team</span>";
                msgbody += "<div style='width:auto; float:left;'>";
                msgbody += " <p style='font-family:arial; font-size:12px; font-weight:bold; margin:0px; padding:0px;'>Call: 416 844 5725 | 647 977 3268  </p>";
                msgbody += " <p style='font-family:arial; font-size:12px; font-weight:bold; margin:0px; padding:0px;'>Fax: 647 497 5646 </p>";
                msgbody += " <a style='color:red; width:100%; font-size:13px; float:left; margin:5px 0 0 0px; text-decoration:none; font-family:arial;' href='www.http://www.only4agents.com'>Web:www.only4agents.com</a>";
                msgbody += " <a style='color:red; width:100%;font-size:13px; float:left; margin:5px 0 0 0px; text-decoration:none; font-family:arial;' href='info@only4agents.com'>Email: info@only4agents.com</a>";
                msgbody += " <a style='color:red; width:100%;font-size:13px; float:left; margin:5px 0 0 0px; text-decoration:none; font-family:arial;' href='http://app.only4agents.com/'>Click here to login: www.app.only4agents.com/</a>";
                msgbody += " </div>";
                msgbody += "<div style='float:left; width:100%; margin:12px 0 6px 0;'>";
                msgbody += "<img style='float:left; width:500px;' src='" + Logourl + "' /> ";
                msgbody += "</div>";
                msgbody += "</div>";
                msgbody += "</div>";
                mail.Body = msgbody;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Host = "smtp.gmail.com";
                smtp.Port = _Port;
                smtp.Credentials = new System.Net.NetworkCredential(FromEmailID, FromEmailPassword);// Enter senders User name and password
                smtp.EnableSsl = _EnableSsl;
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
        }
        #endregion
    }
}