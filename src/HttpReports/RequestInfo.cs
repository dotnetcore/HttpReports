using System;

namespace HttpReports
{
    public class RequestInfo : IRequestInfo
    { 
        public string Id { get; set; }

        public string ParentId { get; set; }

        public string IP { get; set; }

        public int Port { get; set; }

        public string LocalIP { get; set; }

        public int LocalPort { get; set; }

        public string Node { get; set; }

        public string Route { get; set; }

        public string Method { get; set; }

        public int Milliseconds { get; set; }

        public int StatusCode { get; set; }

        public string Url { get; set; }

        public string RequestType { get; set; }

        public DateTime CreateTime { get; set; }
    }
}