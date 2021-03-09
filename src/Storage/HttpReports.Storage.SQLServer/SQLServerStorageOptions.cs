using Microsoft.Extensions.Options;
using System;

namespace HttpReports.Storage.SQLServer
{
    public class SQLServerStorageOptions : IOptions<SQLServerStorageOptions>
    {
        public string ConnectionString { get; set; } 
    

        public bool EnableDefer { get; set; }

       
        public int DeferSecond { get; set; }

      
        public int DeferThreshold { get; set; }

        public SQLServerStorageOptions Value => this;
    }
}