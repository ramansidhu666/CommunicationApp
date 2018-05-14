using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationApp.Entity
{
    public class User
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string TrebId { get; set; }
        public string UserEmailAddress { get; set; }
        public int CompanyID { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<System.DateTime> LastUpdatedOn { get; set; }
        public bool IsActive { get; set; }
        public bool? IsNewsLetterSend { get; set; }

        public virtual Company Companies { get; set; }
        public virtual ICollection<Customer> Customers { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
