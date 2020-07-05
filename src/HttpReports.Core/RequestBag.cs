using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Core
{
    public class RequestBag
    {
        public IRequestInfo RequestInfo { get; set; }

        public IRequestDetail RequestDetail { get; set; }

        public RequestBag(IRequestInfo info,IRequestDetail detail)
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
