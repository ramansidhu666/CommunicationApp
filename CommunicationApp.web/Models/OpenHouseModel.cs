using CommunicationApp.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;
namespace CommunicationApp.Models
{
    public partial class OpenHouseModel
    {
        public int OpenHouseId { get; set; }
        public int? CompanyId { get; set; }
        public int? CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhoto { get; set; }
        public string Address { get; set; }
        public string   City { get; set; }
        public string ContactNumber { get; set; }
        public Nullable<DateTime> FromDateTime { get; set; }
        public Nullable<DateTime> ToDateTime { get; set; }
        public string FromDateTimeStr { get; set; }
        public string ToDateTimeStr { get; set; }
        public decimal? Price { get; set; }
        public string Comments { get; set; }
        public Nullable<DateTime> CreatedOn { get; set; }
        public Nullable<DateTime> LastUpdatedon { get; set; }
        public bool? IsActive { get; set; }


       
    }
  

  
}
