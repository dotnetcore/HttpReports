using System;
using System.Collections.Generic;
using System.Text;
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

        //TODO 定义所有数据存取接口
    }
}
