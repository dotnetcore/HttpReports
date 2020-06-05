using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Storage.SQLite
{
    public class SQLiteStorageOptions : IOptions<SQLiteStorageOptions>
    { 
        public string ConnectionString { get; set; }

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

        public SQLiteStorageOptions Value => this; 
    }
}
