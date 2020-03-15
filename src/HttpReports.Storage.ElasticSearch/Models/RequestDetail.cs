using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Storage.ElasticSearch.Models
{
    internal class RequestDetail
    {
        [Nest.Keyword]
        public string Id { get; set; }

        [Nest.Keyword]
        public string RequestId { get; set; }

        [Nest.Text]
        public string Scheme { get; set; }

        [Nest.Text]
        public string QueryString { get; set; }

        [Nest.Text]
        public string Header { get; set; }

        [Nest.Text]
        public string Cookie { get; set; }

        [Nest.Text]
        public string RequestBody { get; set; }

        [Nest.Text]
        public string ResponseBody { get; set; }

        [Nest.Text]
        public string ErrorMessage { get; set; }

        [Nest.Text]
        public string ErrorStack { get; set; }


        [Nest.Date]
        public DateTime CreateTime { get; set; } 

    }
}
