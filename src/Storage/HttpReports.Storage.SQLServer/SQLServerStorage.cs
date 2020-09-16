 
using HttpReports.Core;
using HttpReports.Core.Config;
using HttpReports.Core.Models;
using HttpReports.Core.Storage.FilterOptions;
using HttpReports.Models;
using HttpReports.Monitor;
using HttpReports.Storage.Abstractions;
using HttpReports.Storage.FilterOptions;
using HttpReports.Storage.Abstractions.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HttpReports.Storage.SQLServer
{
    public class SQLServerStorage : BaseStorage
    {
        public SQLServerStorageOptions _options; 

        public ILogger<SQLServerStorage> Logger { get; }  
      
        private string Prefix { get; set; } = string.Empty; 

        public SQLServerStorage(IOptions<SQLServerStorageOptions> options,ILogger<SQLServerStorage> logger) 

            : base(new BaseStorageOptions
            {
                DeferSecond = options.Value.DeferSecond,
                DeferThreshold = options.Value.DeferThreshold,
                ConnectionString = options.Value.ConnectionString,
                DataType = FreeSql.DataType.SqlServer

            }) 

        {
            _options = options.Value;   
          
            Logger = logger; 
          
        }     



     
    }
}