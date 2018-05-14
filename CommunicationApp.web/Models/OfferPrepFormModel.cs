using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace CommunicationApp.Web.Models
{
    public class OfferPrepFormModel
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "CustomerId is required")]
        public int CustomerId { get; set; }
        [Display(Name = "Office Location")]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
        public string Fax { get; set; }
        [Display(Name="Your Name")]
        [Required(ErrorMessage = "Name is required")]
        public string AgentName { get; set; }
        [Display(Name="Agreement Of Purchase And Sale")]
        [Required(ErrorMessage = "Agreement of Purchase and Sale is required")]
        public string AgreementofPurchaseandSale { get; set; }
        
        [Display(Name="Agreement Date")]
        [Required(ErrorMessage = "AgreementDate is required")]
        public string AgreementDate { get; set; }

        [Display(Name="Mls")]
        [Required(ErrorMessage = "Mls is required")]
        public string MLS { get; set; }

        [Display(Name="Buyer")]
        [Required(ErrorMessage = "Buyer is required")]
        public string Buyer { get; set; }

        [Display(Name="Purchase Price")]
        [Required(ErrorMessage = "Purchase Price is required")]
        public string PurchasePrice { get; set; }

        [Display(Name="In Words")]
        [Required(ErrorMessage = "Price in words is required")]
        public string Price_InWords { get; set; }

        [Display(Name="Deposit Amt")]
        [Required(ErrorMessage = "Deposit Amt. is required")]
        public string DepositAmt { get; set; }

        [Display(Name="In Words")]
        [Required(ErrorMessage = "Deposit Amt in words is required")]
        public string Amt_InWords { get; set; }

        [Display(Name="Deposit")]
        [Required(ErrorMessage = "Deposit is required")]
        public string Deposit { get; set; }

        //[Display(Name = "Lease Commencement Date")]
        //[Required(ErrorMessage = "Completion date is required")]
        [Display(Name = "Completion Date")]
        [Required(ErrorMessage = "Completion Date is required")]
        public string CompletionDate { get; set; }

        [Display(Name="Irrevocable Date&Time")]
        [Required(ErrorMessage = "Irrevocable is required")]
        public string Irrevocable { get; set; }

        [Display(Name="Title Search")]
        [Required(ErrorMessage = "Title Search is required")]
        public string TitleSearch { get; set; }

        [Display(Name="Co-Oprating Broker Commission")]
        [Required(ErrorMessage = "Co_Operating Broker Commission is required")]
        public string Co_OperatingBrokerCommission { get; set; }
        [Required(ErrorMessage = "Are we The is required")]
        public string Arewethe { get; set; }
        public string ChattelsIncluded { get; set; }
        
        [Display(Name="Pls add other")]        
        public string Excluded { get; set; }
        public string Clauses { get; set; }

        [Display(Name="Remarks")]        
        public string Remarks { get; set; }
        public string FinalView { get; set; }
        public string FinalView_Option { get; set; }
        public string[] Chattelschk { get; set; }
        public string[] Clauseschk { get; set; }

        public string StainlessSteelFridge { get; set; }
        public string Fridge { get; set; }
        public string StainlessSteelStove { get; set; }
        public string Stove { get; set; }
        public string FrontLoadingWasher { get; set; }
        public string Washer { get; set; }
        public string FrontLoadingDryer { get; set; }
        public string Dryer { get; set; }
        public string ChattelsCount { get; set; }
        
        public List<int> ChattelsIds { get; set; }
        public List<SelectListItem> EmailList { get; set; }
        public List<ChattelsTypesModel> ChattelsTypesModelList { get; set; }
        public List<ClausesTypeModel> ClausesTypeModelList { get; set; }
        
    }

   
}