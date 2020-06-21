using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace HttpReports.Core
{
    public interface ISegmentContext
    {
        bool Push(string activity,Segment segment);

        Segment Get(string Id);

    }
}
