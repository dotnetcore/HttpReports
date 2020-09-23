using HttpReports.Storage.Abstractions; 
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options; 

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