using HttpReports.Storage.Abstractions; 
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options; 

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