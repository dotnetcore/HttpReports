using HttpReports.Dashboard.DataContext;
using HttpReports.Dashboard.Implements;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace HttpReports.Dashboard.DataAccessors
{
    public  class BaseAccessor
    {
        private SqlConnection connection;

        public BaseAccessor(DBFactory factory)
        {
            connection = factory.GetSqlConnection();
        } 

        public T Get<T>(object where)
        { 
            return connection.GetModel<T>(where);
        }

        public T Add<T>(T t) where T : class, new()
        {
            return connection.AddModel(t);
        }

        public bool Remove<T>(object whereParam)
        {
            return connection.DeleteModel<T>(whereParam);
        }

        public int GetCount<T>(object where)
        {
            return connection.GetCount<T>(where);
        }


        public bool Update<T>(T model) where T : class, new()
        {
            return connection.UpdateModel<T>(model);
        }

        public bool UpdateWhere<T>(object model, object where) where T : class, new()
        {
            return connection.UpdateModel<T>(model, where);
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldParam">查询字段,查询全部为null,查询某个字段，如：new {name="",id=0,status=0} </param>
        /// <param name="whereParam">查询条件，如：new {name="danny",status=1}</param>
        /// <param name="orderBy">排序字段 ,正序为true，降序为false，如：new {sort_id=false,id=true}</param>
        /// <returns></returns>
        public List<T> GetList<T>(object fieldParam = null, object whereParam = null, object orderBy = null) where T : class, new()
        {
            return connection.GetList<T>(fieldParam, whereParam, orderBy);
        }

        public List<T> GetList<T>(object fieldParam, object where, object whereParam, object orderBy, int pagesize, int pageindex, out int totalCount)
        {
            return connection.GetList<T>(fieldParam, where, whereParam, orderBy, pagesize, pageindex, out totalCount);
        }


        /// <summary>
        /// Dapper获取分页列表
        /// </summary>
        /// <typeparam name="T">获取的列表类型</typeparam>
        /// <param name="sql">sql语句（不包含orderby以外的部分）</param>
        /// <param name="orderby">orderby的字段，如果多个可用,分隔，逆序可用desc</param>
        /// <param name="pagesize">页大小</param>
        /// <param name="pageindex">当前页</param>
        /// <param name="totalCount">数据总数</param>
        /// <returns></returns>
        public List<T> GetListBySql<T>(string sql, string orderby, int pagesize, int pageindex, out int totalCount)
        {
            return connection.GetListBySql<T>(sql, orderby, pagesize, pageindex, out totalCount);
        }
    }
}
