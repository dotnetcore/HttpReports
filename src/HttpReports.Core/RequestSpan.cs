using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Core
{
    public class RequestSpan:IRequestSpan
    {
        public string Id { get; set; }
        public string ParentId { get; set; }
        public string RequestId { get; set; }
        public string SpanType { get; set; }
        public string Action { get; set; }
        public string Input { get; set; }
        public string Output { get; set; }
        public string Payload { get; set; }
        public string Millisecond { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
