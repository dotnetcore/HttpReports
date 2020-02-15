using System;

using Microsoft.Extensions.Options;

namespace HttpReports
{
    public class HttpReportsOptions : IOptions<HttpReportsOptions>
    {
       
        public string ApiPoint { get; set; } = "api";

      
        public string Node { get; set; } = "default"; 
        

        public bool UseHome { get; set; } = true;

        public bool FilterStaticFiles { get; set; } = true;


        public HttpReportsOptions Value => this;
    } 
}