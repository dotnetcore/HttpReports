using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports
{
    public interface IPerformance
    {
        string Id { get; set; } 
        string Service { get; set; }

        string Instance { get; set; } 

        int GCGen0 { get; set; }

        int GCGen1 { get; set; }

        int GCGen2 { get; set; }

        double HeapMemory { get; set; } 

        double ProcessCPU { get; set; }  

        double ProcessMemory { get; set; } 

        int ThreadCount { get; set; }

        int PendingThreadCount { get; set; }

        DateTime CreateTime { get; set; }  

    }
}
