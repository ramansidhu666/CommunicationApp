using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CommunicationApp.Web.Models
{
    public class BrokeragerDetailModel
    {
      
        public int BrokerageDetailId { get; set; }
        public int? BrokerageId { get; set; }
        public int? BrokerageServicesId { get; set; }
        public int? BrokerageServicesValue { get; set; }
        public int? ParentId { get; set; }
        public string BrokerageServices { get; set; }
        public string BrokerageValue { get; set; }
       
    }//

    public class PDFDetailModel
    {

        public int BrokerageDetailId { get; set; }
        public int? BrokerageId { get; set; }
        public int? BrokerageServicesId { get; set; }
        public int? BrokerageServicesValue { get; set; }
        public int? ParentId { get; set; }
        public string BrokerageServices { get; set; }
        public string BrokerageValue { get; set; }

    }//
}
