using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CommunicationApp.Models
{
    public class RoleModel
    {
        public RoleModel()
        {
            //this.Roles = new HashSet<RoleDetailModel>();
        }

        public int RoleId { get; set; }

        [Display(Name = "Role Name")]
        [Required]
        public string RoleName { get; set; }
        [Display(Name = "Role Description")]
        public string RoleDescription { get; set; }

        [Display(Name = "Role Type")]
        [Required]
        public string RoleType { get; set; }
        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }
        [Display(Name = "Created On")]
        public Nullable<System.DateTime> CreatedOn { get; set; }

        //public virtual ICollection<UserRoleModel> UserRoles { get; set; }
        //public virtual ICollection<RoleDetailModel> Roles { get; set; }
    }
    public class RoleTypes
    {
        public string RoleTypeId { get; set; }
        public string RoleType { get; set; }
        public static IEnumerable<RoleTypes> GetRoleType(bool AddSuperAdmin=false)
        {
            List<RoleTypes> lstRoleType = new List<RoleTypes>();
            //Add SuperAdmin
            if (AddSuperAdmin)
            {
                lstRoleType.Add(new RoleTypes { RoleTypeId = "SuperAdmin", RoleType = "SuperAdmin" });
            }
            //Add Admin
            lstRoleType.Add(new RoleTypes { RoleTypeId = "Admin", RoleType = "Admin" });
            //Add User
            lstRoleType.Add(new RoleTypes { RoleTypeId = "User", RoleType = "User" });
            //Add Driver
            lstRoleType.Add(new RoleTypes { RoleTypeId = "Driver", RoleType = "Driver" });

            return lstRoleType;
        }
        public enum RoleTypeValue
        {
            SuperAdmin,
            Admin,
            User,
            Driver
        }
    }
    
}