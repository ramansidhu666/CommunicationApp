using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationApp.Entity
{
    public class MessageImage
    {
        public int MessageImageId { get; set; }
        public int MessageId { get; set; }
        public string ImageUrl { get; set; }

    }
}
