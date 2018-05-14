using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace CommunicationApp.Models
{
    public partial class EventCustomerModel
    {
        public int EventCustomerId { get; set; }
        public int EventId { get; set; }
        public int CustomerId { get; set; }
    }
}
