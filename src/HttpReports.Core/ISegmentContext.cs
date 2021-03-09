using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace HttpReports.Core
{
    public interface ISegmentContext
    {
        bool Push(string activity,Segment segment);

        List<Segment> GetSegments(string Id);

        bool Release(string Id);

    }
}
