using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks; 
using Google.Protobuf.WellKnownTypes;
using HttpReports.Core;
using HttpReports.Core.Config;
using HttpReports.Core.Models;
using HttpReports.Core.Storage.FilterOptions;
using HttpReports.Models;
using HttpReports.Monitor;
using HttpReports.Storage.Abstractions;
using HttpReports.Storage.Abstractions.Models;
using HttpReports.Storage.FilterOptions; 
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace HttpReports.Storage.MySql
{
    internal class MySqlStorage : BaseStorage
    {
        public MySqlStorageOptions Options { get; } 

        public ILogger<MySqlStorage> Logger { get; } 

        public MySqlStorage(IOptions<MySqlStorageOptions> options, ILogger<MySqlStorage> logger)

             : base(new BaseStorageOptions
             { 
                 DeferSecond = options.Value.DeferSecond,
                 DeferThreshold = options.Value.DeferThreshold,
                 ConnectionString = options.Value.ConnectionString,
                 DataType = FreeSql.DataType.MySql

             }) 

        {
            Options = options.Value; 
            Logger = logger; 
        }  
        
    }
}