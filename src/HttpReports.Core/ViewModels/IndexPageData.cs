using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Core.ViewModels
{
    public class IndexPageData
    {
        public double AvgResponseTime { get; set; }

        public int Total { get; set; }

        public int NotFound { get; set; }

        public int ServerError { get; set; }

        public int APICount { get; set; }
        public double ErrorPercent { get; set; }

        public int Service { get; set; }

        public int Instance { get; set; }

    }
}
