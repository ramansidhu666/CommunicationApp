using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationApp.Entity
{
    public class Event
    {
        public int EventId { get; set; }
        public int? CustomerId { get; set; }      
        public string EventDescription { get; set; }
        public DateTime EventDate { get; set; }
        public string EventName { get; set; }
        public string EventImage { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public Nullable<DateTime> CreatedOn { get; set; }
        public Nullable<DateTime> LastUpdatedon { get; set; }
        public bool IsActive { get; set; }

        public virtual Customer Customers { get; set; }//
        public virtual ICollection<EventCustomer> EventCustomers { get; set; }
       
        
    }
}
