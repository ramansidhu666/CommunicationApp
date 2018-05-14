using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace CommunicationApp.Models
{
    public class NewsLetter_Model
    {
        public int NewsLetterId { get; set; }
        public int? AdminId { get; set; }
        [Display(Name = "Name")]
        [Required(ErrorMessage = "NewsLetter Name is Required")]
        public string NewsLetterName { get; set; }
        [Display(Name = "Photo")]
        public string Image { get; set; }
        [Display(Name = "Order")]
        public int OrderNo { get; set; }
        public string SelectedUsers { get; set; }

        public List<string> Select_users { get; set; } 

        public Nullable<DateTime> fwd_date { get; set; }
        public bool IsActive { get; set; }


        public string first_img { get; set; }
        public string second_img { get; set; }


        public IEnumerable<string> SelectedCustomer { get; set; }
        public IEnumerable<SelectListItem> CustomersList { get; set; }        
       
        public IEnumerable<SelectListItem> OrderList { get; set; }
    }


}