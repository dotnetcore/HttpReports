namespace HttpReports.Dashboard.Models
{
    public class Result
    {
        /// <summary>
        /// Json 结果返回
        /// </summary>
        /// <param name="code">1 成功 , 其他失败</param>
        /// <param name="msg">消息体</param>
        /// <param name="data">数据体</param>
        public Result(int code, string msg, object data = null)
        {
            this.code = code;
            this.msg = msg;
            this.data = data ?? this.data;
        }

        /// <summary>
        /// 1 成功 其他失败
        /// </summary>
        public int code { get; set; } = 1;

        /// <summary>
        /// 错误消息
        /// </summary>
        public string msg { get; set; }

        /// <summary>
        /// Json 数据
        /// </summary>
        public object data { get; set; }
    }
}