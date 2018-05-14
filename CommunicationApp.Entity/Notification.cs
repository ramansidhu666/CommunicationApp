using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationApp.Entity
{
   public class Notification
    {
           public int NotificationId { get; set; }
           public string RequestMessage { get; set; }
           public int? NotificationSendTo { get; set; }
           public int? NotificationSendBy { get; set; }          
           public bool? IsRead { get; set; }
           public int? Flag { get; set; }
          
    }
}
