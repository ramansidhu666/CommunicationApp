using CommunicationApp.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationApp.Data.Mapping
{
    class OfferPrepFormMap : EntityTypeConfiguration<OfferPrepForm>
    {
        public OfferPrepFormMap()
        {
            //table
            ToTable("OfferPrepForm");

            HasKey(t => t.Id).Property(c => c.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            //properties
            Property(t => t.CustomerId);
            Property(t => t.Email);
            Property(t => t.Fax);
            Property(t => t.AgentName);
            Property(t => t.AgreementofPurchaseandSale);
            Property(t => t.AgreementDate);
            Property(t => t.MLS);
            Property(t => t.Buyer);
            Property(t => t.PurchasePrice);
            Property(t => t.Price_InWords);
            Property(t => t.DepositAmt);
            Property(t => t.Amt_InWords);
            Property(t => t.Deposit);
            Property(t => t.CompletionDate);
            Property(t => t.Irrevocable);
            Property(t => t.TitleSearch);
            Property(t => t.Co_OperatingBrokerCommission);
            Property(t => t.Arewethe);
            Property(t => t.ChattelsIncluded);
            Property(t => t.Excluded);
            Property(t => t.Clauses);
            Property(t => t.FinalView);
            Property(t => t.FinalView_Option);
            Property(t => t.Remarks);
            Property(t => t.ChattelsCount);

        }
    }
}
