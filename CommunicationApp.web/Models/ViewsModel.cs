using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommunicationApp.Web.Models
{
    public class ViewsModel
    {
        public int ViewsId { get; set; }
        public int? CustomerId { get; set; }
        public int? PropertyId { get; set; }
        public bool Status { get; set; }
    }
}