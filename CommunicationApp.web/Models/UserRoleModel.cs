using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CommunicationApp.Models
{
    public class UserRoleModel
    {
        public int UserRoleId { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }

        public virtual RoleModel Role { get; set; }
        public virtual UserModel User { get; set; }
    }
}