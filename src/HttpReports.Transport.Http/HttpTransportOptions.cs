using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Transport.Http
{
    public class HttpTransportOptions : IOptions<HttpTransportOptions>
    {
        public Uri CollectorAddress { get; set; }

        public int DeferSecond { get; set; }
         
        public int DeferThreshold { get; set; }  

        public HttpTransportOptions Value => this;

    }
}
