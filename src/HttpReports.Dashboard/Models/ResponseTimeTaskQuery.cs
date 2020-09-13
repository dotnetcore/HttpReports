using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Dashboard.Models
{
    public class ResponseTimeTaskQuery:QueryRequest
    {
        public int TimeoutMS { get; set; }
    }
}
