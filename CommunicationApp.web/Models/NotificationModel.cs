using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommunicationApp.Models
{
    public class NotificationModel
    {
        public int NotificationId { get; set; }
        public string RequestMessage { get; set; }
        public int? NotificationSendTo { get; set; }
        public int? NotificationSendBy { get; set; }
        public int? Flag { get; set; }
        public bool? IsRead { get; set; }
    }

    public class NotificationCountModel
    {
        public int ExclusiveResidential { get; set; }
        public int ExclusiveCommercial { get; set; }
        public int ExclusiveCondo { get; set; }
        public int NewHotCommercial { get; set; }
        public int NewHotResidential { get; set; }
        public int NewHotCondo { get; set; }
        public int LookingForCommercial { get; set; }
        public int LookingForResidential { get; set; }
        public int LookingForCondo { get; set; }

    }

    public class NotificationCount
    {
        public int? Flag{get;set;}
        public int Count{get;set;}
        public int NotificationSendTo { get; set; }
    }
}