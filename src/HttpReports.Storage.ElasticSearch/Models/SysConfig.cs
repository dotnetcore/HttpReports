using HttpReports.Core.Interface;
using Nest;
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Storage.ElasticSearch.Models
{

    [ElasticsearchType(RelationName = "sysconfig")]
    public class SysConfig : ISysConfig
    {
        [Nest.Keyword]
        public string Id { get; set; }

        [Nest.Keyword]
        public string Key { get; set; }

        [Nest.Keyword]
        public string Value { get; set; }
      
    }
}
