using Newtonsoft.Json;

namespace HttpReports.Dashboard.DTO
{
    /// <summary>
    /// Http返回结果
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class HttpResult
    {
        #region 属性

        /// <summary>
        /// 消息
        /// </summary>
        [JsonProperty("msg", NullValueHandling = NullValueHandling.Ignore)]
        public string Msg { get; set; }

        /// <summary>
        /// 状态码 1成功
        /// </summary>
        [JsonProperty("code")]
        public int Code { get; set; } 
        

        #endregion 属性

        /// <summary>
        /// Http返回结果
        /// </summary>
        public HttpResult()
        {
        }

        /// <summary>
        /// Http返回结果
        /// </summary>
        /// <param name="code">1 成功 , 其他失败</param>
        /// <param name="message">消息体</param>
        public HttpResult(int code, string message)
        {
            Code = code;
            Msg = message;
        }
    }

    /// <summary>
    /// 基础Http返回结果
    /// </summary>
    public class HttpResultEntity : HttpResultEntity<object>
    {
        /// <summary>
        /// 基础Http返回结果
        /// </summary>
        /// <param name="code">1 成功 , 其他失败</param>
        /// <param name="message">消息体</param>
        /// <param name="data">数据体</param>
        public HttpResultEntity(int code, string message, object data)
        {
            Code = code;
            Msg = message;
            Data = data;
        }
    }

    /// <summary>
    /// 基础Http返回结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class HttpResultEntity<T> : HttpResult
    {
        #region 属性

        /// <summary>
        /// 数据内容
        /// </summary>
        [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
        public T Data { get; set; }

        #endregion 属性

        /// <summary>
        /// 基础Http返回结果
        /// </summary>
        /// <param name="code">1 成功 , 其他失败</param>
        /// <param name="message">消息体</param>
        /// <param name="data">数据体</param>
        public HttpResultEntity(int code, string message, T data)
        {
            Code = code;
            Msg = message;
            Data = data;
        }

        /// <summary>
        /// 基础Http返回结果
        /// </summary>
        public HttpResultEntity()
        {
        }
    }
}