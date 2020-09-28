using System; 
using Microsoft.Extensions.Options;

namespace HttpReports
{
    public class HttpReportsOptions : IOptions<HttpReportsOptions>
    {  
        public string Service { get; set; } = "Default";

        public string Server { get; set; } = "localhost:80"; 

        public bool Switch { get; set; } = true;

        public int MaxBytes { get; set; } = 20000;

        public bool PayloadSwitch { get; set; } = true;

        public string[] RequestFilter { get; set; } = { }; 

        public HttpReportsOptions Value => this;

    } 
}