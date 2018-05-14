using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CommunicationApp.Models
{
    public class MessageModel
    {       
        public int MessageId { get; set; }
              
        public string CustomerIds { get; set; }
        [Required(ErrorMessage = "Message is Required")]
        [Display(Name = "Message")]
        //[MaxLength(2000,ErrorMessage="max length 2000 only.")]
        public string Messages { get; set; }
        [Required(ErrorMessage = "Message heading is Required")]
        [Display(Name = "Heading")]
        public string Heading { get; set; }
        public Nullable<DateTime> CreatedOn { get; set; }
        public Nullable<DateTime> LastUpdatedon { get; set; }
        public bool IsActive { get; set; }
        [Display(Name = "All Users")]
        public bool All { get; set; }
        [Display(Name = "Are you want to choose image")]
        public bool IsWithImage { get; set; }

        [Display(Name = "Choose Image")]
        public string ImageUrl { get; set; }

       
        public List<MessageImageModel> ImageUrlList { get; set; }
        [Display(Name = "Message For")]        
        public IEnumerable<string> SelectedCustomer { get; set; }
        public IEnumerable<SelectListItem> CustomersList { get; set; }
    }

   
}