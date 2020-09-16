using System;

using Microsoft.Extensions.Options;

namespace HttpReports.Storage.MySql
{
    public class MySqlStorageOptions : IOptions<MySqlStorageOptions>
    {
        public string ConnectionString { get; set; } 
 
        public int DeferSecond { get; set; } 
      
        public int DeferThreshold { get; set; } 

        public MySqlStorageOptions Value => this;
    }
}