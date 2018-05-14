using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;
namespace EventAppAmeba.Models
{
    public partial class EventImageModel
    {
        public int EventImageId { get; set; }
        public int EventID { get; set; }
        public string ImagePath { get; set; }
        public List<string> ImageList { get; set; }
    }

}
