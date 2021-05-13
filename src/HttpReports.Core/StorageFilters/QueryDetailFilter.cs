 
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Core.StorageFilters
{
    public class QueryDetailFilter:BasicFilter
    {
        public long RequestId { get; set; } 

        public string Route { get; set; }

        public int StatusCode { get; set; }

        public string Method { get; set; }

        public string RequestBody { get; set; }

        public string ResponseBody { get; set; }

        // 相应时间最小值。0时表示不判断，否则表示取超过这个时间的响应
        public int MinMs { get; set; }
    }
}
