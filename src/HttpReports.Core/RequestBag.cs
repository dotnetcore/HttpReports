using HttpReports.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Core
{
    public class RequestBag
    {
        public RequestInfo RequestInfo { get; set; }

        public RequestDetail RequestDetail { get; set; }

        public RequestBag(RequestInfo info,RequestDetail detail)
        {
            this.RequestInfo = info;
            this.RequestDetail = detail;
        } 
    }
    public class RequestBagJson
    {
        public RequestInfo RequestInfo { get; set; }

        public RequestDetail RequestDetail { get; set; }

    } 

}
