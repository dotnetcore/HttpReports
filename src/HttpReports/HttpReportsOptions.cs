using System; 
using Microsoft.Extensions.Options;

namespace HttpReports
{
    public class HttpReportsOptions : IOptions<HttpReportsOptions>
    {  
        public string Service { get; set; } = "Default";

        public string Server { get; set; } = "http://localhost:5000"; 

        public bool Switch { get; set; } = true;

        public int MaxBytes { get; set; } = 20000; 

        public bool WithRequest { get; set; } = false;

        public bool WithResponse { get; set; } = false;

        public bool WithCookie { get; set; } = false;

        public bool WithHeader { get; set; } = false; 

        public string[] RequestFilter { get; set; } = { }; 

        public HttpReportsOptions Value => this;

    } 
}