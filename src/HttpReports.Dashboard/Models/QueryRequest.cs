using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Dashboard.Models
{
    public class QueryRequest
    {
        public string Service { get; set; }

        public string Instance { get; set; }


        public string Start { get; set; }


        public string End { get; set; }

        public string LocalIP { get; set; }

        public int LocalPort { get; set; }


    }
}
