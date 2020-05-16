using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Dashboard.Handle
{
    public interface IDashboardHandle
    {
        DashboardContext Context { get; set; }
    }
}
