using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationApp.Entity
{
    public class BrokerageSevice
    {
        public int BrokerageServicesId { get; set; }
        public string BrokerageServices { get; set; }
        public bool? Status { get; set; }
        public int? ParentId { get; set; }
        public virtual ICollection<BrokerageDetail> BrokerageDetails { get; set; }
    }
}
