using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommunicationApp.Web.Models
{
    public class BrokeragerServiceModel
    {
        public int BrokerageServicesId { get; set; }
        public string BrokerageServices { get; set; }
        public bool? Status { get; set; }
        public int? ParentId { get; set; }     

    }
}