using System.Threading.Tasks;

namespace HttpReports
{
    /// <summary>
    /// 储存库接口
    /// </summary>
    public interface IHttpReportsStorage
    {
        /// <summary>
        /// 初始化储存库
        /// </summary>
        /// <returns></returns>
        Task InitAsync();

        /// <summary>
        /// 添加一条请求记录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task AddRequestInfoAsync(IRequestInfo request);

        //TODO 定义所有数据存取接口
    }
}