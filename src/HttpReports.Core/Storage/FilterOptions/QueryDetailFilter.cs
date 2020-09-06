using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Core.Storage.FilterOptions
{
    public class QueryDetailFilter:BasicFilter
    {
        public string RequestId { get; set; }

        public string Url { get; set; }

        public int StatusCode { get; set; }

        public string RequestBody { get; set; }

        public string ResponseBody { get; set; }

    }
}
