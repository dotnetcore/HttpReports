namespace HttpReports.Storage.FilterOptions
{
    /// <summary>
    /// 节点过滤选项
    /// </summary>
    public interface INodeFilterOption : IFilterOption
    {
        /// <summary>
        /// 节点列表
        /// </summary>
        string[] Nodes { get; set; }
    }
}