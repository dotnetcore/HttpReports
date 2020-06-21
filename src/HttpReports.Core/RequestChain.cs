using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Core
{
    public class RequestChain : IRequestChain
    {
        public string Id { get; set; }

        public string ParentId { get; set; }

        public string Service { get; set; }

        public string Instance { get; set; }

        public string Component { get; set; }

        public string Value { get; set; }

        public string Error { get; set; }

        public string ErrorMessage { get; set; }

        public int IsSuccess { get; set; }  

        public int Milliseconds { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
