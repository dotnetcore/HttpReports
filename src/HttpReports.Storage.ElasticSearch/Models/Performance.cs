using HttpReports.Core.Interface;
using Nest;
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Storage.ElasticSearch.Models
{
    [ElasticsearchType(RelationName = "performance")]
    public class Performance : IPerformance
    {
        [Nest.Keyword]
        public string Id { get; set; }

        [Nest.Keyword]
        public string Service { get; set; }

        [Nest.Keyword] 
        public string Instance { get; set; }

        [Nest.Number] 
        public int GCGen0 { get; set; }


        [Nest.Number]
        public int GCGen1 { get; set; }


        [Nest.Number]
        public int GCGen2 { get; set; }

        [Nest.Number]
        public double HeapMemory { get; set; }


        [Nest.Number]
        public double ProcessCPU { get; set; }


        [Nest.Number]
        public double ProcessMemory { get; set; }


        [Nest.Number]
        public int ThreadCount { get; set; }


        [Nest.Number]
        public int PendingThreadCount { get; set; }


        [Nest.Date]
        public DateTime CreateTime { get; set; }

    }
}
