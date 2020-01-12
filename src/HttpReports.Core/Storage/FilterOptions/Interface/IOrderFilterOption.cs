namespace HttpReports.Storage.FilterOptions
{
    /// <summary>
    /// 排序过滤选项
    /// </summary>
    public interface IOrderFilterOption : IFilterOption
    {
        /// <summary>
        /// 是否排序字段
        /// </summary>
        bool IsOrderByField { get; set; }

        /// <summary>
        /// 是否升序
        /// </summary>
        bool IsAscend { get; set; }

        /// <summary>
        /// 获取排序字段的字符串
        /// </summary>
        /// <returns></returns>
        string GetOrderField();
    }

    /// <summary>
    /// 排序过滤选项
    /// </summary>
    public interface IOrderFilterOption<T> : IOrderFilterOption
    {
        /// <summary>
        /// 排序依据的字段
        /// </summary>
        T Field { get; set; }
    }
}