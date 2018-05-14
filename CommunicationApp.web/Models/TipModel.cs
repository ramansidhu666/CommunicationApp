using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CommunicationApp.Models
{
    public class TipModel
    {
        public int TipId { get; set; }
        public int? CustomerId { get; set; }
         [Required(ErrorMessage = "Title is Required")]
        [Display(Name = "Title")]
        public string Title { get; set; }
        [Display(Name = "Download")]
        public string TipUrl { get; set; }
        [Display(Name = "Posted Date")]
        public Nullable<DateTime> ShowDate { get; set; }
        public Nullable<DateTime> CreatedOn { get; set; }
        public Nullable<DateTime> LastUpdatedon { get; set; }
        public bool IsActive { get; set; }

    }

   
}