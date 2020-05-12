using System;
using Microsoft.AspNetCore.Mvc.Formatters.Internal;
using Microsoft.Extensions.Options;

namespace HttpReports
{
    public class HttpReportsOptions : IOptions<HttpReportsOptions>
    {  
        public string Node { get; set; } = "Default";

        public bool Switch { get; set; } = true;

        public bool FilterStaticFile { get; set; } = true;

        public string[] FilterRequest { get; set; } = new string[] { };

        public HttpReportsOptions Value => this;
    } 
}