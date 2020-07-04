using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HttpReports.Storage.SQLServer
{
    public static class DapperExtensions
    {

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
        public static async Task<IEnumerable<T>> GetListBySqlAsync<T>(this IDbConnection con, string sql, string orderby, int pagesize, int pageindex, int totalCount)
        {
            var pagingSql = CreatePagingSql(totalCount, pagesize, pageindex, sql, orderby);
            return await con.QueryAsync<T>(pagingSql);
        }


        /// <summary>
        /// 获取分页SQL语句，默认row_number为关健字，所有表不允许使用该字段名
        /// </summary>
        /// <param name="_recordCount">记录总数</param>
        /// <param name="_pageSize">每页记录数</param>
        /// <param name="_pageIndex">当前页数</param>
        /// <param name="_safeSql">SQL查询语句</param>
        /// <param name="_orderField">排序字段，多个则用“,”隔开</param>
        /// <returns>分页SQL语句</returns>
        public static string CreatePagingSql(int _recordCount, int _pageSize, int _pageIndex, string _safeSql, string _orderField)
        {
            //计算总页数
            _pageSize = _pageSize == 0 ? _recordCount : _pageSize;
            int pageCount = (_recordCount + _pageSize - 1) / _pageSize;

            //检查当前页数
            if (_pageIndex < 1)
            {
                _pageIndex = 1;
            }
            else if (_pageIndex > pageCount)
            {
                _pageIndex = pageCount;
            }
            //拼接SQL字符串，加上ROW_NUMBER函数进行分页
            StringBuilder newSafeSql = new StringBuilder();
            newSafeSql.AppendFormat("SELECT ROW_NUMBER() OVER(ORDER BY {0}) as row_number,", _orderField);
            newSafeSql.Append(_safeSql.Substring(_safeSql.ToUpper().IndexOf("SELECT") + 6));

            //拼接成最终的SQL语句
            StringBuilder sbSql = new StringBuilder();
            sbSql.Append("SELECT * FROM (");
            sbSql.Append(newSafeSql.ToString());
            sbSql.Append(") AS T");
            sbSql.AppendFormat(" WHERE row_number between {0} and {1}", ((_pageIndex - 1) * _pageSize) + 1, _pageIndex * _pageSize);
            sbSql.Append(" order by  row_number asc");
            return sbSql.ToString();
        }


        /// <summary>
        /// 获取记录总数SQL语句
        /// </summary>
        /// <param name="_safeSql">SQL查询语句</param>
        /// <returns>记录总数SQL语句</returns>
        public static string CreateCountingSql(string _safeSql, string count_filed = "1")
        {
            return string.Format(" SELECT COUNT(" + count_filed + ") AS RecordCount FROM ({0}) AS T ", _safeSql);
        }
    }
}
