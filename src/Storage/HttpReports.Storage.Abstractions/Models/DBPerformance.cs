using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Storage.Abstractions.Models
{

    [Table(Name = "Performance")]
    [Index("idx_performance_id", "Id", true)]
    public class DBPerformance
    {
        [Column(IsPrimary = true)]
        public long Id { get; set; }

        [Column(StringLength = 50)]
        public string Service { get; set; } 

        [Column(StringLength = 50)]
        public string Instance { get; set; }

        public int GCGen0 { get; set; }

        public int GCGen1 { get; set; }

        public int GCGen2 { get; set; } 

     
        public double HeapMemory { get; set; }

        
        public double ProcessCPU { get; set; }

         
        public double ProcessMemory { get; set; }

        public int ThreadCount { get; set; }

        public int PendingThreadCount { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
