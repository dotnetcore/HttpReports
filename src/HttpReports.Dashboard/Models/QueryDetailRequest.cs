using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Dashboard.Models
{
    public class QueryDetailRequest:QueryRequest
    {
        public string RequestId { get; set; }

        public string Route { get; set; }

        public int StatusCode { get; set; }

        public string RequestBody { get; set; }

        public string ResponseBody { get; set; }


    }
}
