using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace CommunicationApp.Entity
{
    public partial class FeedBack
    {
        public int FeedBackId { get; set; }
        public int? CustomerId { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public Nullable<DateTime> CreatedOn { get; set; }
        
        public virtual Customer Customers { get; set; }
    }
}
