using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommunicationApp.Web.Models
{
    public class ChattelsTypesModel
    {
        public int Id { get; set; }
        public string ChattelsName { get; set; }
        public string ChattelsCount { get; set; }
        public bool IsChecked { get; set; }
        public ChattelsTypesModel()
        {
            IsChecked = false;
        }
    }
}