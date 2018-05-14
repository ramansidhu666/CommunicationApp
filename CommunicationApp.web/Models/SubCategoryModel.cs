using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommunicationApp.Models
{
    public class SubCategoryModel
    {
        public int SubCategoryId { get; set; }
        public string SubCategoryName { get; set; }
        public string CategoryName { get; set; }
        public int CategoryId { get; set; }
        public string PhotoPath { get; set; }
        public string Price { get; set; }
        public bool IsActive { get; set; }
    }
}