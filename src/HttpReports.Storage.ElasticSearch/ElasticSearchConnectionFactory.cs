using Elasticsearch.Net;
using HttpReports.Core.Config;
using Microsoft.Extensions.Options;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HttpReports.Storage.ElasticSearch
{
    public class ElasticSearchConnectionFactory
    { 
        public ElasticClient Client;

        public ElasticSearchStorageOptions Options { get; }

        public ElasticSearchConnectionFactory(IOptions<ElasticSearchStorageOptions> options)
        {
            Options = options.Value; 

            InitClient(); 
        } 

        private void InitClient()
        {
            if (Options.ConnectionString.IsEmpty())
            {
                throw new Exception("ElasticSearch 连接配置错误"); 
            }   
             
            List<Uri> uris = Options.ConnectionString.Split(',').Select(x => x.Trim()).Select(x=> new Uri(x)).ToList();

            var connectionSettings = new ConnectionSettings(new StaticConnectionPool(uris));

            if (!Options.UserName.IsEmpty() && !Options.Password.IsEmpty())
            {
                connectionSettings.BasicAuthentication(Options.UserName.Trim(),Options.Password.Trim());
            }

            Client = new ElasticClient(connectionSettings); 
        } 

    }
}
