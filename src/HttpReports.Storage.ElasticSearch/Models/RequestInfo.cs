using Nest;
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Storage.ElasticSearch.Models
{
    [ElasticsearchType(RelationName = "requestinfo")]
    internal class RequestInfo : IRequestInfo
    {
        [Nest.Keyword]
        public string Id { get; set; }


        [Nest.Keyword]
        public string ParentId { get; set; }


        [Nest.Keyword]
        public string IP { get; set; }


        [Nest.Keyword]
        public int Port { get; set; }


        [Nest.Keyword]
        public string LocalIP { get; set; }


        [Nest.Keyword]
        public int LocalPort { get; set; }


        [Nest.Keyword]
        public string Node { get; set; }


        [Nest.Keyword]
        public string Route { get; set; }


        [Nest.Keyword]
        public string Method { get; set; }


        [Nest.Number]
        public int Milliseconds { get; set; }


        [Nest.Keyword]
        public int StatusCode { get; set; }


        [Nest.Keyword]
        public string Url { get; set; }


        [Nest.Keyword]
        public string RequestType { get; set; }


        [Nest.Date]
        public DateTime CreateTime { get; set; }  
    }
}
