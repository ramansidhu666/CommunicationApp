using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationApp.Entity
{
    public class Views
    {
        public int ViewsId { get; set; }
        public int? CustomerId { get; set; }
        public int? PropertyId { get; set; } 
        public bool Status { get; set; }

        public virtual Customer Customers { get; set; }
       
       
        
    }
}
