using CommunicationApp.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace CommunicationApp.Entity
{
    public partial class SubCategory
    {
        public int SubCategoryId { get; set; }
        public string SubCategoryName { get; set; }
        public int CategoryId { get; set; }        
        public bool IsActive { get; set; }

        public virtual Category Categories { get; set; }
        public virtual ICollection<Supplier> Suppliers { get; set; }
       
    }
}
