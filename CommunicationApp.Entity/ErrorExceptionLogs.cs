using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationApp.Entity
{
    public class ErrorExceptionLogs
    {
        public int EventId { get; set; }
        public Nullable<DateTime> LogDateTime { get; set; }
        public string Source { get; set; }
        public string Message { get; set; }
        public string QueryString { get; set; }
        public string TargetSite { get; set; }
        public string StackTrace { get; set; }
        public string ServerName { get; set; }
        public string RequestURL { get; set; }
        public string UserAgent { get; set; }
        public string UserIP { get; set; }
        public string UserAuthentication { get; set; }
        public string UserName { get; set; }
    }
}
