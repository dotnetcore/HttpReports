using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Core.Storage.FilterOptions
{
    public class IndexBasicDataFilter
    {
        public string Service { get; set; }

        public string Instance { get; set; }


        public DateTime StartTime { get; set; }


        public DateTime EndTime { get; set; } 

    }
}
