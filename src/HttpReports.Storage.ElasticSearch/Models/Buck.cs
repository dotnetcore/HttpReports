using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Storage.ElasticSearch.Models
{
    public class KeyedBucked
    {
        public string Key { get; set; }

        public long? DocCount { get; set; }
    }
}
