 
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
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HttpReports.Storage.PostgreSQL
{
    public class PostgreSQLStorage : BaseStorage 
    {
        public PostgreStorageOptions Options { get; } 

        public ILogger<PostgreSQLStorage> Logger { get; }  
        
        public PostgreSQLStorage(IOptions<PostgreStorageOptions> options, ILogger<PostgreSQLStorage> logger)

            : base(new BaseStorageOptions {

                DeferSecond = options.Value.DeferSecond,
                DeferThreshold = options.Value.DeferThreshold,
                ConnectionString = options.Value.ConnectionString,
                DataType = FreeSql.DataType.PostgreSQL

            })

        {
            Options = options.Value;   
            Logger = logger;
            
        }  
    }
}
