using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationApp.Entity
{
    public class Message
    {
        public int MessageId { get; set; }
        public int? AdminId { get; set; }
        public string CustomerIds { get; set; }
        public string Messages { get; set; }
        public string Heading { get; set; }
        public string ImageUrl { get; set; }
        public Nullable<DateTime> CreatedOn { get; set; }
        public Nullable<DateTime> LastUpdatedon { get; set; }
        public bool IsActive { get; set; }
        public virtual Customer Customers { get; set; }
    }
}
