using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Storage.PostgreSQL
{
    public class PostgreStorageOptions : IOptions<PostgreStorageOptions>
    {
        public string ConnectionString { get; set; }

        public string TablePrefix { get; set; }  
      
        public int DeferSecond { get; set; }
 
        public int DeferThreshold { get; set; }

        public PostgreStorageOptions Value => this;
    }
}
