using HttpReports.Core.StorageFilters;
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Core.StorageFilters
{
    public class ResponseTimeTaskFilter:BasicFilter
    {
        public int TimeoutMS { get; set; }
    }
}
