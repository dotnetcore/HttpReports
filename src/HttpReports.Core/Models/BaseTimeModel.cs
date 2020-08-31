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

        public decimal GcGen0_Raw { get; set; }

        public decimal GcGen1_Raw { get; set; }

        public decimal GcGen2_Raw { get; set; }

        public decimal HeapMemory_Raw { get; set; }

        public decimal ProcessMemory_Raw { get; set; }

        public decimal ThreadCount_Raw { get; set; } 


        public int  GcGen0 { get; set; }

        public int GcGen1 { get; set; }

        public int GcGen2 { get; set; }

        public double HeapMemory { get; set; }

        public double ProcessMemory { get; set; }

        public int ThreadCount { get; set; }


    }  

}
