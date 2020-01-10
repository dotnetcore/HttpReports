using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HttpReports.Dashboard.Models
{
    public class CheckModel
    {
        public bool Ok { get; set; } = true;

        public string Value { get; set; }

        public string Time { get; set; } 

    }
}
