using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace CommunicationApp.Models
{
    public partial class JsonReturnModel
    {
        public string Status { get; set; }
        public object Message { get; set; }
    }
}
