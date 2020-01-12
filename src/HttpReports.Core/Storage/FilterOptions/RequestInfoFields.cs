namespace HttpReports.Storage.FilterOptions
{
    /// <summary>
    /// 请求信息字段枚举
    /// </summary>
    public enum RequestInfoFields
    {
        /// <summary>
        /// Id
        /// </summary>
        Id,

        /// <summary>
        /// 服务节点
        /// </summary>
        Node,

        /// <summary>
        /// 路由
        /// </summary>
        Route,

        /// <summary>
        /// 请求地址
        /// </summary>
        Url,

        /// <summary>
        /// GET Or Post
        /// </summary>
        Method,

        /// <summary>
        /// 请求毫秒数
        /// </summary>
        Milliseconds,

        /// <summary>
        /// HTTP 状态码
        /// </summary>
        StatusCode,

        /// <summary>
        /// IP
        /// </summary>
        IP,

        /// <summary>
        /// 创建时间
        /// </summary>
        CreateTime,
    }
}