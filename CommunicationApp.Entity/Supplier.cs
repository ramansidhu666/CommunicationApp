using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace CommunicationApp.Entity
{
    public partial class Supplier
    {
        public int SupplierId { get; set; }        
        public int? AdminId { get; set; }
        public int? CategoryId { get; set; }
        public int? SubCategoryId { get; set; } 
        public string FirstName { get; set; }
        public string Lastname { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public string MobileNo { get; set; }
        public string Skill { get; set; }
        public string Photopath { get; set; }
        public string EmailID { get; set; }
        public string Region { get; set; }
        public string SubRegion { get; set; }
        public bool? IsActive { get; set; }


        public virtual Category Categories { get; set; }
        public virtual SubCategory SubCategories { get; set; }
        public virtual Customer Customers { get; set; }
       
      
    }
}
