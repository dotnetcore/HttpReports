using System; 
using Microsoft.Extensions.Options;

namespace HttpReports
{
    public class HttpReportsOptions : IOptions<HttpReportsOptions>
    {  
        public string Service { get; set; } = "Default";

        public string Server { get; set; } = "localhost:80";


        public bool Switch { get; set; } = true; 

        public string[] FilterRequest { get; set; } = { }; 

        public HttpReportsOptions Value => this;

    } 
}