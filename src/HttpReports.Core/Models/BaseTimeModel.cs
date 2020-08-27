using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Core.Models
{
    public class BaseTimeModel
    { 
        public string KeyField { get; set; }

        public string TimeField { get; set; } 

        public int ValueField { get; set; }   

    }

    public class APPTimeModel
    {

        public string TimeField { get; set; }

        public double GcGen0 { get; set; }

        public double GcGen1 { get; set; }

        public double GcGen2 { get; set; }

        public double HeapMemory { get; set; }

        public int ThreadCount { get; set; } 

    } 
     


}
