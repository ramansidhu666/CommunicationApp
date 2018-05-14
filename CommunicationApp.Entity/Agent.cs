using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace CommunicationApp.Entity
{
    public partial class Agent
    {
        public int AgentId { get; set; }
        public int? CompanyId { get; set; }
        public int? CustomerId { get; set; }        
        public string City { get; set; }
        public string MLS { get; set; }
        public string Date { get; set; }
        public string FromTime { get; set; }
        public string ToTime { get; set; }
        public string Date2 { get; set; }
        public string FromTime2 { get; set; }
        public string ToTime2 { get; set; }
        public decimal? Price { get; set; }
        public string Comments { get; set; }
        public int? AgentStatusId { get; set; }
        public int? ParentId { get; set; }
        public Nullable<DateTime> CreatedOn { get; set; }
        public Nullable<DateTime> LastUpdatedon { get; set; }
        public bool? IsActive { get; set; }

      
        public virtual Company Companies { get; set; }
        public virtual Customer Customers { get; set; }
       
      
    }
}
