namespace HttpReports
{
    /// <summary>
    /// model创建器，用于创建储存库特有的有特性标签的对象
    /// </summary>
    public interface IModelCreator
    {
        IRequestInfo NewRequestInfo();
    }
}