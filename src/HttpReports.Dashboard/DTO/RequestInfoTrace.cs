using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Dashboard.DTO
{
    public class RequestInfoTrace 
    {
        public string Text { get; set; }

        public string Id { get; set; }

        public string Node { get; set; }

        public string Url { get; set; }

        public List<RequestInfoTrace> Nodes { get; set; }

    }
}
