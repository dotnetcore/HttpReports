 
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Core.Models
{
    public class RequestDetail
    {
        
        public long Id { get; set; } 
        
        public long RequestId { get; set; }
        public string Scheme { get; set; }
        public string QueryString { get; set; }
        public string Header { get; set; }
        public string Cookie { get; set; }
        public string RequestBody { get; set; }
        public string ResponseBody { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorStack { get; set; }
        public DateTime CreateTime { get; set; }

    }
}
