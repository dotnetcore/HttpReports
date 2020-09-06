using System.Collections.Generic;

using HttpReports.Storage.FilterOptions;

namespace HttpReports.Models
{ 
    public class RequestInfoSearchResult
    { 
        public List<RequestInfo> List { get; set; } = new List<RequestInfo>();

 
        public int Total { get; set; } 
       
    }
}