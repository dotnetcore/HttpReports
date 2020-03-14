using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports
{
    public interface IRequestDetail
    {
        string Id { get; set; }

        string RequestId { get; set; }

        string Scheme { get; set; }

        string QueryString { get; set; }

        string Header { get; set; }

        string Cookie { get; set; }

        string RequestBody { get; set; }

        string ResponseBody { get; set; }

        string ErrorMessage { get; set; }

        string ErrorStack { get; set; } 

        DateTime CreateTime { get; set; }

    }
}
