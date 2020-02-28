namespace HttpReports.Storage.FilterOptions
{
    /// <summary>
    /// 基于状态码的过滤选项
    /// </summary>
    public interface IStatusCodeFilterOption : IFilterOption
    {
        /// <summary>
        /// 状态码列表
        /// </summary>
        int[] StatusCodes { get; set; }
    }
}