using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Core.Storage.FilterOptions
{
    public class ResponseTimeTaskFilter:BasicFilter
    {
        public int TimeoutMS { get; set; }
    }
}
