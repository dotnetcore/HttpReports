namespace HttpReports.Storage.FilterOptions
{
    public interface ITakeFilterOption : IFilterOption
    {
        /// <summary>
        /// 跳过数量
        /// </summary>
        int Skip { get; set; }

        /// <summary>
        /// 获取数量
        /// </summary>
        int Take { get; set; }
    }
}