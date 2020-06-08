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
        public string Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
      
    }
}
