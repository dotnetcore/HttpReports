using HttpReports.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Dashboard.Models
{
    public class RequestTraceTree
    {
        public RequestInfo Info { get; set; }

        public List<RequestTraceTree> Nodes { get; set; }

    }
}
