using HttpReports.Core.Interface;
using Nest;
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Storage.ElasticSearch.Models
{ 
    [ElasticsearchType(RelationName = "sysuser")]
    public class SysUser : ISysUser
    { 
        [Nest.Keyword]
        public string Id { get; set; }

        [Nest.Keyword]
        public string UserName { get; set; }

        [Nest.Keyword]
        public string Password { get; set; }
    }
}
