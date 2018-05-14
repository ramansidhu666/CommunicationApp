using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CommunicationApp.Models
{
    public class FormModel
    {
        public int FormId { get; set; }
        [Display(Name = "Form Name")]
        public string FormName { get; set; }
        [Display(Name = "Controller Name")]
        public string ControllerName { get; set; }
    }
}