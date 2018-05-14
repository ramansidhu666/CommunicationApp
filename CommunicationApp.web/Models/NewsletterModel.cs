using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CommunicationApp.Models
{
    public class NewsletterModel
    {
        public string AdminPhoto { get; set; }
        public string AdminLogo { get; set; }
        public string PropertyPhoto { get; set; }
        public string FirstContent { get; set; }
        public string SecondContent { get; set; }
        public string ThirdContent { get; set; }
        public string TemplateType { get; set; }
        public string CustomerIds { get; set; }
        [Display(Name = "Select Users")]
        public IEnumerable<string> SelectedCustomer { get; set; }
        public IEnumerable<SelectListItem> CustomersList { get; set; }
        [Display(Name = "All Users")]
        public bool All { get; set; }
        //public SecondNewsletterModel SecondNewsletterModel { get; set; }

        public string P1 { get; set; }
        public string P2 { get; set; }
        public string P3 { get; set; }
        public string P4 { get; set; }
        public string P5 { get; set; }
        public string P6 { get; set; }
        public string P7 { get; set; }
        public string PG1 { get; set; }
        public string PG2 { get; set; }
        public string PG3 { get; set; }
        public string PG4 { get; set; }
        public string PG5 { get; set; }
        public string PG6 { get; set; }
        public string PG7 { get; set; }

        //FOR second page
        public string PP1 { get; set; }
        public string PP2 { get; set; }
        public string PP3 { get; set; }
        public string PP4 { get; set; }
        public string PP5 { get; set; }
        public string PP6 { get; set; }
        public string PPG1 { get; set; }
        public string PPG2 { get; set; }
        public string PPG3 { get; set; }
        public string PPG4 { get; set; }
        public string PPG5 { get; set; }
        public string PPG6 { get; set; }
        //End

    }
  
}