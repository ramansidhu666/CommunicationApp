using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace CommunicationApp.Models
{
    public  class FeedBackModel
    {
        
        public int FeedBackId { get; set; }
        public int? Customerid { get; set; }
        [Display(Name = "Name")]
        public string CustomerName { get; set; }
        [Display(Name = "Photo")]
        public string CustomerPhoto { get; set; }
        [Display(Name = "Subject")]
        public string Subject { get; set; }
        public string Message { get; set; }
        public bool? IsRead { get; set; }
        public Nullable<DateTime> CreatedOn { get; set; } 
      
    }
}
