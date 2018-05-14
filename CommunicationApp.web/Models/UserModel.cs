using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;

namespace CommunicationApp.Models
{
    public class UserModel
    {
        public int UserId { get; set; }
   
        [Display(Name = "First Name")]
        [Required(ErrorMessage = "Required")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string LastName { get; set; }
        [Display(Name = "User Name")]
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z0-9]*$", ErrorMessage = "Username should contain only alpha-numeric characters.")]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Required")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Required")]
        [Display(Name = "Email")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}"
            + @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\"
            + @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Invalid Office E-mail Id")]
        
        public string UserEmailAddress { get; set; }
        public string TrebId { get; set; }

        [Display(Name = "Company Name")]
        [Required(ErrorMessage = "Company is required")]
        public int CompanyID { get; set; }

        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<System.DateTime> LastUpdatedOn { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }

        public bool? IsNewsLetterSend { get; set; }

        
        public string ShowMessage { get; set; }

       // public virtual CompanyModel Companys { get; set; }
        //public virtual ICollection<UserRoleModel> UserRoles { get; set; }
    }
}