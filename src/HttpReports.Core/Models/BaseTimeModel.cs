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

        public decimal GcGen0 { get; set; }

        public decimal GcGen1 { get; set; }

        public decimal GcGen2 { get; set; }

        public decimal HeapMemory { get; set; }

        public decimal ThreadCount { get; set; } 

    }  

}
