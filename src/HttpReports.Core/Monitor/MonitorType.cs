namespace HttpReports.Monitor
{
    /// <summary>
    /// 监控类型
    /// </summary>
    public enum MonitorType
    {
        /// <summary>
        /// 未定义
        /// </summary>
        UnDefine = 0,

        /// <summary>
        /// 响应超时
        /// </summary>
        ResponseTimeOut,

        /// <summary>
        /// 错误的响应
        /// </summary>
        ErrorResponse,

        /// <summary>
        /// 某个地址(api或页面)请求量过多
        /// </summary>
        ToManyRequestWithAddress,

        /// <summary>
        /// 单个远程地址请求量过多
        /// </summary>
        ToManyRequestBySingleRemoteAddress,
    }
}