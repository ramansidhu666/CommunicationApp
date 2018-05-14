using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CommunicationApp.Models
{

    public class EmailModel
    {
        [Display(Name = "Full Name")]
        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "Invalid Full Name")]
        public string FullName { get; set; }
        
        [Display(Name = "Email ID")]
        [RegularExpression("^[a-z0-9_\\+-]+(\\.[a-z0-9_\\+-]+)*@[a-z0-9-]+(\\.[a-z0-9]+)*\\.([a-z]{2,4})$", ErrorMessage = "Invalid Email ID")]
        public string EmailID { get; set; }

        [Display(Name = "From Email ID")]
        [RegularExpression("^[a-z0-9_\\+-]+(\\.[a-z0-9_\\+-]+)*@[a-z0-9-]+(\\.[a-z0-9]+)*\\.([a-z]{2,4})$", ErrorMessage = "Invalid From Email ID")]
        public string FromEmailID { get; set; }

        [Display(Name = "From To ID")]
        [RegularExpression("^[a-z0-9_\\+-]+(\\.[a-z0-9_\\+-]+)*@[a-z0-9-]+(\\.[a-z0-9]+)*\\.([a-z]{2,4})$", ErrorMessage = "Invalid To Email ID")]
        public string ToEmailID { get; set; }

        [Display(Name = "Subject")]
        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "Invalid Subject")]
        public string Subject { get; set; }

        [Display(Name = "Message")]
        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "Invalid Message")]
        public string Message { get; set; }
        public string ShowMessage { get; set; }
    }
}