using CommunicationApp.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace CommunicationApp.Entity
{
    public class NewsLetter_Entity
    {
        public int NewsLetterId { get; set; }
        public int? AdminId { get; set; }
        public string NewsLetterName { get; set; }
        public string Image { get; set; }
        public int OrderNo { get; set; }
        public Nullable<DateTime> fwd_date { get; set; }
        public bool? IsActive { get; set; }
        public string SelectedUsers { get; set; }


        public virtual Customer Customers { get; set; }
    }


}