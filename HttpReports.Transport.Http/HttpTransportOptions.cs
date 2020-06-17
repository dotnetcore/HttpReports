using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Transport.Http
{
    public class HttpTransportOptions
    {
        public Uri CollectorAddress { get; set; }

        public HttpTransportOptions Value => this;

    }
}
