using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommunicationApp.Web.Models
{
    public class BannerModel
    {
        public int BannerId { get; set; }
        public int? CustomerId { get; set; }
        public string Url { get; set; }
        public Nullable<DateTime> CreatedOn { get; set; }
        public Nullable<DateTime> LastUpdatedon { get; set; }
        public bool IsActive { get; set; }

    }
}