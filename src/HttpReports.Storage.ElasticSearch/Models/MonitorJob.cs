using HttpReports.Monitor;
using Nest;
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Storage.ElasticSearch.Models
{
    [ElasticsearchType(RelationName = "monitorjob")]
    public class MonitorJob : IMonitorJob
    {
        [Nest.Keyword]
        public string Id { get; set; }

        [Nest.Text]
        public string Title { get; set; }

        [Nest.Text]
        public string Description { get; set; }

        [Nest.Text]
        public string CronLike { get; set; }

        [Nest.Text]
        public string WebHook { get; set; }

        [Nest.Text]
        public string Emails { get; set; }

        [Nest.Text]
        public string Mobiles { get; set; }

        [Nest.Text]
        public int Status { get; set; }

        [Nest.Text]
        public string Nodes { get; set; }

        [Nest.Text]
        public string Payload { get; set; }

        [Nest.Keyword]
        public DateTime CreateTime { get; set; }

    }
}
