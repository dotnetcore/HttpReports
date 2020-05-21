using System.Collections.Generic;

namespace HttpReports.Dashboard.DTO
{
    public class RequestInfoTrace
    {
        public string Text { get; set; }

        public string Id { get; set; }

        public string Node { get; set; }

        public string Url { get; set; }

        public int Milliseconds { get; set; }

        public int StatusCode { get; set; }

        public string RequestType { get; set; }

        public List<RequestInfoTrace> Nodes { get; set; }
    }
}