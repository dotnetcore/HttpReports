using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Core
{
    public interface ISegmentContext
    {
        string Component { get; set; }

        string OperateName { get; set; }
    }
}
