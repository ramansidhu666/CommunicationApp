using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace CommunicationApp.Entity
{
    public class Role
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
        public string RoleType { get; set; }
        public bool IsActive { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; }
        public virtual ICollection<RoleDetail> RoleDetails { get; set; }
    }
    public class RoleTypes
    {
        public string RoleTypeId { get; set; }
        public string RoleType { get; set; }
        public static IEnumerable<RoleTypes> GetRoleType(bool AddSuperAdmin = false)
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
