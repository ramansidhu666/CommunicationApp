using CommunicationApp.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;
namespace CommunicationApp.Models
{
    public partial class AgentModel
    {
        public int AgentId { get; set; }
        public int? CompanyId { get; set; }
        public int? CustomerId { get; set; }
        public string TrebId { get; set; }
        public string AgentType { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhoto { get; set; }
        public string CustomerMobileNo { get; set; }
        public string CustomerEmail { get; set; }
        public string WebsiteUrl { get; set; }
        public string MLS { get; set; }
        public string AgentTypeStatus { get; set; }
        public string   City { get; set; }
        public string Date { get; set; }
        public string FromTime { get; set; }
        public string ToTime { get; set; }
        public string Date2 { get; set; }
        public string FromTime2 { get; set; }
        public string ToTime2 { get; set; }
        public string FromDateTimeStr { get; set; }
        public string ToDateTimeStr { get; set; }
        public decimal? Price { get; set; }
        public string Comments { get; set; }
        public string AdminName { get; set; }
        public string AdminPhoto { get; set; }
        public string AdminPhoneNo { get; set; }
        public string AdminWebSiteUrl { get; set; }
        public Nullable<DateTime> CreatedOn { get; set; }
        public Nullable<DateTime> LastUpdatedon { get; set; }
        public bool? IsActive { get; set; }
        public int? AgentStatusId { get; set; }
        public int ParentId { get; set; }
        public List<string> Imagelist { get; set; }
        public string OpenHousePropertyImage { get; set; }
        public List<PropertyImages> PropertyPhotolist { get; set; }

       
    }
    public class AgentListModel
    {
        public string UserName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public List<AgentModel> _AgentListModel { get; set; }
    }

  
}
