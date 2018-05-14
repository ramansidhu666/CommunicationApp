using CommunicationApp.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace CommunicationApp.Entity
{
    public partial class PropertyImage
    {
        public int PropertyImageId { get; set; }
        public int? PropertyId { get; set; }
        public int? AgentId { get; set; }
        public string ImagePath { get; set; }

        public virtual Property Properties { get; set; }
    }
}
