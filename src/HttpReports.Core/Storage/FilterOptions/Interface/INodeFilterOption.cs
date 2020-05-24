namespace HttpReports.Storage.FilterOptions
{
    /// <summary>
    /// 节点过滤选项
    /// </summary>
    public interface INodeFilterOption : IFilterOption
    {

        string Service { get; set; }
        string LocalIP { get; set; }

        int LocalPort { get; set; }
    }
}