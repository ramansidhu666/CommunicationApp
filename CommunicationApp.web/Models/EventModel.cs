using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace CommunicationApp.Models
{
    public class EventModel
    {
        public int EventId { get; set; }
        public int? CustomerId { get; set; }
        [Display(Name = "Event Description")]
        [Required(ErrorMessage = "EventDescription is Required")]
        public string EventDescription { get; set; }
        [Display(Name = "Event Date")]
        public DateTime EventDate { get; set; }
        [Display(Name = "Event Name")]
        [Required(ErrorMessage = "Event Name is Required")]
        public string EventName { get; set; }
        public string EventImage { get; set; }
        [Display(Name = "Event Time")]
        [Required(ErrorMessage = "Event Time is Required")]
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public Nullable<DateTime> CreatedOn { get; set; }
        public Nullable<DateTime> LastUpdatedon { get; set; }
        public bool IsActive { get; set; }
        [Display(Name = "All Users")]
        public bool All { get; set; }
        [Display(Name = "Event For")]
        //[Required(ErrorMessage = "Please select user.")]
        public IEnumerable<string> SelectedCustomer { get; set; }
        public IEnumerable<SelectListItem> CustomersList { get; set; }
    }


}