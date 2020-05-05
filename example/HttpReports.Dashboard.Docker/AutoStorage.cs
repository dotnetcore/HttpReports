using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HttpReports.Dashboard.Docker
{
    public static class AutoStorage
    {
        public static IHttpReportsBuilder UseAutoStorage(this IHttpReportsBuilder builder,StorageOptions options)
        {
            if (options.StorageType.ToUpper() == "MYSQL")
            {
                builder = builder.UseMySqlStorage(x => {

                    x.ConnectionString = options.ConnectionString; 

                });
            }

            if (options.StorageType.ToUpper() == "ORACLE")
            {
                builder = builder.UseOracleStorage(x => {

                    x.ConnectionString = options.ConnectionString; 
                });
            }

            if (options.StorageType.ToUpper() == "SQLSERVER" || options.StorageType.ToUpper() == "SQL SERVER")
            {
                builder = builder.UseSQLServerStorage(x => {

                    x.ConnectionString = options.ConnectionString; 

                });
            }

            if (options.StorageType.ToUpper() == "PostgreSQL")
            {
                builder = builder.UsePostgreSQLStorage(x => {

                    x.ConnectionString = options.ConnectionString; 

                });
            }  

            return builder;
        } 
    }

    public class StorageOptions
    {
        public string StorageType { get; set; }

        public string ConnectionString { get; set; }   

    }

}
