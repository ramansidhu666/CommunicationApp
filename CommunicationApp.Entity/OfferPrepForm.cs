using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationApp.Entity
{
   public class OfferPrepForm
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string Email { get; set; }
        public string Fax { get; set; }
        public string AgentName { get; set; }
        public string AgreementofPurchaseandSale { get; set; }
        public string AgreementDate { get; set; }
        public string MLS { get; set; }
        public string Buyer { get; set; }
        public string PurchasePrice { get; set; }
        public string Price_InWords { get; set; }
        public string DepositAmt { get; set; }
        public string Amt_InWords { get; set; }
        public string Deposit { get; set; }
        public string CompletionDate { get; set; }
        public string Irrevocable { get; set; }
        public string TitleSearch { get; set; }
        public string Co_OperatingBrokerCommission { get; set; }
        public string Arewethe { get; set; }
        public string ChattelsIncluded { get; set; }
        public string Excluded { get; set; }
        public string Clauses { get; set; }        
        public string FinalView { get; set; }
        public string FinalView_Option { get; set; }
        public string Remarks { get; set; }
        public string ChattelsCount { get; set; }
    }
}
