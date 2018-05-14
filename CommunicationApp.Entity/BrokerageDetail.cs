using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationApp.Entity
{
   public class BrokerageDetail
    {
        public int BrokerageDetailId { get; set; }
        public int? BrokerageId { get; set; }
        public int? BrokerageServicesId { get; set; }
        public int? BrokerageServicesValue { get; set; }
        public virtual Brokerage Brokerages { get; set; }
        public virtual BrokerageSevice BrokerageSevices { get; set; } 
    }
}
