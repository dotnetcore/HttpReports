using HttpReports.Storage.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options; 

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
