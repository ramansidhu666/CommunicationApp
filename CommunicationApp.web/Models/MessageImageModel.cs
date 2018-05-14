using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CommunicationApp.Models
{
    public class MessageImageModel
    {
        public int MessageImageId { get; set; }
        public int MessageId { get; set; } 
        [Display(Name = "Choose Image")]
        public string ImageUrl { get; set; }

       
    }

   
}