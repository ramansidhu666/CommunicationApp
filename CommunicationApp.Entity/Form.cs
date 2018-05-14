using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace CommunicationApp.Entity
{
    public class Form
    {
        public int FormId { get; set; }
        public string FormName { get; set; }
        public string ControllerName { get; set; }

        public virtual ICollection<RoleDetail> RoleDetails { get; set; }
    }
}
