using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace CommunicationApp.Entity
{
    public class RoleDetail
    {
        public int RoleDetailID { get; set; }
        public bool IsCreate { get; set; }
        public bool IsEdit { get; set; }
        public bool IsView { get; set; }
        public bool IsDelete { get; set; }
        public bool IsDetail { get; set; }
        public bool IsDownload { get; set; }
        public System.DateTime CreateDate { get; set; }
        public int FormId { get; set; }
        public int RoleId { get; set; }

        public virtual Form Forms { get; set; }
        public virtual Role Roles { get; set; }
    }
}
