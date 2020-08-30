using System; 
namespace HttpReports
{
    public class RequestInfo 
    {  
        public string Id { get; set; }

        public string ParentId { get; set; }

        public string Service { get; set; }

        public string Instance { get; set; }

        public string Route { get; set; }

        public string Url { get; set; }

        public string RequestType { get; set; }

        public string Method { get; set; }

        public int Milliseconds { get; set; }

        public int StatusCode { get; set; }

        public string RemoteIP { get; set; }


        public string LoginUser { get; set; }

        public DateTime CreateTime { get; set; } 
    }
}