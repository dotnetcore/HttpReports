using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Core
{
    public interface IRequestSpan
    {
        string Id { get; set; }

        string ParentId { get; set; }

        string RequestId { get; set; }

        string SpanType { get; set; }

        string Action { get; set; } 

        string Input { get; set; }

        string Output { get; set; }

        string Payload { get; set; }

        string Millisecond { get; set; }

        DateTime CreateTime { get; set; } 

    }
}
