using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace CommunicationApp.Models
{
    public class RoleDetailModel
    {
        public int RoleDetailID { get; set; }

        [Display(Name = "Create")]
        public bool IsCreate { get; set; }
        [Display(Name = "Edit")]
        public bool IsEdit { get; set; }
        [Display(Name = "View")]
        public bool IsView { get; set; }
        [Display(Name = "Delete")]
        public bool IsDelete { get; set; }
        [Display(Name = "Detail")]
        public bool IsDetail { get; set; }
        [Display(Name = "Download")]
        public bool IsDownload { get; set; }
        [Display(Name = "Create Date")]
        public System.DateTime CreateDate { get; set; }
        public int FormId { get; set; }
        public int RoleId { get; set; }

        public string ControllerName { get; set; }
        public string FormName { get; set; }

        public virtual FormModel form { get; set; }
        public virtual RoleModel Role { get; set; }
    }
    public enum ControllerName
    {
        User, RoleDetail, UserRoleType
    }
}
