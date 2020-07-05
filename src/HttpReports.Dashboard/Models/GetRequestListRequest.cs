namespace HttpReports.Dashboard.Models
{
    public class GetRequestListRequest
    {
        public string Start { get; set; }

        public string End { get; set; }

        public string Url { get; set; }

        public string IP { get; set; }

        public string Service { get; set; }

        public string Instance { get; set; }

        public string LocalIP { get; set; }

        public int LocalPort { get; set; }

        public int pageNumber { get; set; }

        public int pageSize { get; set; }

        public string TraceId { get; set; }

        public string StatusCode { get; set; }

        public string Request { get; set; }

        public string Response { get; set; } 

    }
}