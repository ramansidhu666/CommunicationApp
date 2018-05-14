using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationApp.Entity
{
    public class Banner
    {
        public int BannerId { get; set; }
        public int? CustomerId { get; set; }
        public string Url { get; set; }       
        public Nullable<DateTime> CreatedOn { get; set; }
        public Nullable<DateTime> LastUpdatedon { get; set; }
        public bool IsActive { get; set; }
        public virtual Customer Customers { get; set; }
    }
}
