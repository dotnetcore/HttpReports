using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Storage.ElasticSearch
{
    public class ElasticSearchStorageOptions:IOptions<ElasticSearchStorageOptions>
    { 
        public string ConnectionString { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; } 

        /// <summary>
        /// 是否启用延时写入
        /// </summary>
        public bool EnableDefer { get; set; }

        /// <summary>
        /// 延时时间
        /// </summary>
        public int DeferSecond { get; set; }

        /// <summary>
        /// 延时阈值
        /// </summary>
        public int DeferThreshold { get; set; }

        public ElasticSearchStorageOptions Value => this;
    }
}
