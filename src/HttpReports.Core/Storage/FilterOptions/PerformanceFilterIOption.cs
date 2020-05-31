using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Core.Storage.FilterOptions
{
    public class PerformanceFilterIOption
    {
        public string Service { get; set; }

        public string Instance { get; set; }

        public int TimeFormat { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; } 

    }
}
