using System;

using Microsoft.Extensions.Options;

namespace HttpReports
{
    public class HttpReportsOptions : IOptions<HttpReportsOptions>
    {  
        public string Node { get; set; } = "Default";

        public bool Open { get; set; } = true;

        public bool FilterStaticFiles { get; set; } = true;


        public HttpReportsOptions Value => this;
    } 
}