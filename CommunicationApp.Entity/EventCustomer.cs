using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace CommunicationApp.Entity
{
    public class EventCustomer
    {
        public int EventCustomerId { get; set; }
        public int? EventId { get; set; }
        public int? CustomerId { get; set; }

        public virtual Event Events { get; set; }
        public virtual Customer Customers { get; set; }
    }
}
