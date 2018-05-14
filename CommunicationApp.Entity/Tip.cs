using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationApp.Entity
{
    public class Tip
    {
        public int TipId { get; set; }
        public int? CustomerId { get; set; }
        public string Title { get; set; }
        public string TipUrl { get; set; }
        public Nullable<DateTime> ShowDate { get; set; }
        public Nullable<DateTime> CreatedOn { get; set; }
        public Nullable<DateTime> LastUpdatedon { get; set; }
        public bool IsActive { get; set; }

        public virtual Customer Customers { get; set; }
        
        
    }
}
