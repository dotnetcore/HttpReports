using System;

using Microsoft.Extensions.Options;

namespace HttpReports.Storage.MySql
{
    internal class MySqlStorageOptions : IOptions<MySqlStorageOptions>
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

        public MySqlStorageOptions Value => this;
    }
}