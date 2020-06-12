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


        [Nest.Text]
        public string IP { get; set; }


        [Nest.Number]
        public int Port { get; set; }


        [Nest.Text]
        public string LocalIP { get; set; }


        [Nest.Number]
        public int LocalPort { get; set; } 



        [Nest.Keyword]
        public string Node { get; set; }


        [Nest.Text]
        public string Route { get; set; }


        [Nest.Text]
        public string Method { get; set; }


        [Nest.Number]
        public int Milliseconds { get; set; }


        [Nest.Text]
        public int StatusCode { get; set; }


        [Nest.Text]
        public string Url { get; set; }


        [Nest.Text]
        public string RequestType { get; set; }


        [Nest.Date]
        public DateTime CreateTime { get; set; }  
    }
}
