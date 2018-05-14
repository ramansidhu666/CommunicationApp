using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommunicationApp.Models
{
    public class DivisionModel
    {
        public int DivisionId { get; set; }
        public int? ParentId { get; set; }
        public string DivisionName{ get; set; }
        public List<DivisionModel> SubRegionModel { get; set; }
        public Nullable<System.DateTime> CreationDate { get; set; }
      
    }
}