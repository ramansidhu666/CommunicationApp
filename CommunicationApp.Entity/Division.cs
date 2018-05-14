using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationApp.Entity
{
    public class Division
    {
        public int DivisionId { get; set; }
        public int? ParentId { get; set; }
        public string DivisionName { get; set; }
        public Nullable<System.DateTime> CreationDate { get; set; }
    }
}
