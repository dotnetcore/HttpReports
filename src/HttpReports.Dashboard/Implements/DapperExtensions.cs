using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;

using Dapper;

namespace HttpReports.Dashboard.Implements
{
    public static class DapperExtensions
    {
        #region 获取一条对象

        /// <summary>
        /// 执行sql并返回单个对象
        /// </summary>
        /// <typeparam name="T">model类型</typeparam>
        /// <param name="con">直接调用DapperConnection类</param>
        /// <param name="sql">sql语句</param>
        /// <returns>查询到的对象，可能为空</returns>
        public static T GetModelBySql<T>(this SqlConnection con, string sql)
        {
            return con.Query<T>(GetAntiXssSql(sql)).FirstOrDefault();
        }

        /// <summary>
        /// 获取一个对象
        /// </summary>
        /// <typeparam name="T">要获取的对象</typeparam>
        /// <param name="con"></param>
        /// <param name="param"> 参数， 假设用到@a，则传入new {a='xxx'},传入对象即可</param>
        /// <returns></returns>
        public static T GetModel<T>(this SqlConnection con, object whereParam)
        {
            if (whereParam == null)
                throw new Exception("参数不能为null");
            var sql = $"select * from  [{ typeof(T).Name}]  where  ";
            var t = whereParam.GetType();
            var properties = t.GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                if (i > 0)
                    sql += $" and  ";
                sql += $" {properties[i].Name}=@{properties[i].Name}";
            }
            return con.Query<T>(GetAntiXssSql(sql), whereParam).FirstOrDefault();
        }

        /// <summary>
        /// 执行sql并返回单个对象
        /// </summary>
        /// <typeparam name="T">model类型</typeparam>
        /// <param name="con">直接调用DapperConnection类</param>
        /// <param name="sql">sql语句</param>
        /// <param name="param">sql里面用的参数，假设用到@a，则传入new {a='xxx'},传入对象即可，这里不是memberinitexpression</param>
        /// <returns>查询到的对象，可能为空</returns>
        public static T GetModelBySql<T>(this SqlConnection con, string sql, object param)
        {
            if (!sql.Contains("@"))
                throw new Exception("请使用参数查询");
            return con.Query<T>(GetAntiXssSql(sql), param).FirstOrDefault();
        }

        /// <summary>
        /// 执行sql并返回单个对象
        /// </summary>
        /// <typeparam name="T">model类型</typeparam>
        /// <param name="con">直接调用DapperConnection类</param>
        /// <param name="sql">sql语句</param>
        /// <param name="param">sql里面用的参数，假设用到@a，则传入new {a='xxx'},传入对象即可，这里不是memberinitexpression</param>
        /// <returns>查询到的对象，可能为空</returns>
        public static T GetModelBySql<T>(this SqlConnection con, string sql, object param, T model)
        {
            if (!sql.Contains("@"))
                throw new Exception("请使用参数查询");
            return con.Query<T>(GetAntiXssSql(sql), param).FirstOrDefault();
        }

        #endregion 获取一条对象

        #region insert方法

        /// <summary>
        /// insert 一个实体
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="con">直接调用DapperConnection类</param>
        /// <param name="model">模型</param>
        /// <param name="fieldWithOutInsert">不需要插入的字段，主要是针对自增主键</param>
        /// <returns></returns>
        public static T AddModel<T>(this SqlConnection con, T model, string fieldWithOutInsert = "id", bool is_filter = true)
        where T : class
        {
            var insertParameterSql = GetInsertParamSql(typeof(T), fieldWithOutInsert);
            if (is_filter)
            {
                model = ReturnSecurityObject(model) as T;
            }

            var identify = GetModelBySql<int?>(con, insertParameterSql, model);
            if (!string.IsNullOrWhiteSpace(fieldWithOutInsert))
                model = SetIdentify(model, fieldWithOutInsert, identify);
            return model;
        }

        #endregion insert方法

        #region 获取列表

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
        public static List<T> GetListBySql<T>(this SqlConnection con, string sql, string orderby, int pagesize, int pageindex, out int totalCount)
        {
            var safeSql = GetAntiXssSql(sql);
            totalCount = con.Query<int>(PagingHelper.CreateCountingSql(safeSql)).First();
            var pagingSql = PagingHelper.CreatePagingSql(totalCount, pagesize, pageindex, safeSql, orderby);
            return con.Query<T>(pagingSql).ToList();
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
        public static List<T> GetListBySql<T>(this SqlConnection con, string sql, string orderby, int pagesize, int pageindex, out int totalCount, object param)
        {
            var safeSql = GetAntiXssSql(sql);
            totalCount = con.Query<int>(PagingHelper.CreateCountingSql(safeSql), param).First();
            var pagingSql = PagingHelper.CreatePagingSql(totalCount, pagesize, pageindex, safeSql, orderby);
            return con.Query<T>(pagingSql, param).ToList();
        }

        /// <summary>
        /// 查询数据列表
        /// </summary>
        /// <typeparam name="T">查询对象</typeparam>
        /// <param name="con">数据库链接</param>
        /// <param name="fieldParam">查询字段,查询全部为null,查询某个字段，如：new {name="",id=0,status=0}</param>
        /// <param name="where">查询条件where sql语句</param>
        /// <param name="whereParam">查询条件参数值，如：new {name="danny",status=1}</param>
        /// <param name="orderBy">排序字段 ,正序为true，降序为false，如：new {sort_id=false,id=true}</param>
        /// <param name="pagesize">每页条数</param>
        /// <param name="pageindex">当前页</param>
        /// <param name="totalCount">返回总条数</param>
        /// <returns></returns>
        public static List<T> GetList<T>(this SqlConnection con, object fieldParam, object where, object whereParam, object orderBy, int pagesize, int pageindex, out int totalCount)
        {
            var table_name = typeof(T).Name;

            #region 查询字段

            var fields = "*";
            StringBuilder sql = new StringBuilder();
            if (fieldParam != null)
            {
                fields = "";
                var t = fieldParam.GetType();
                var properties = t.GetProperties();
                for (int i = 0; i < properties.Length; i++)
                {
                    if (i > 0)
                        fields += ",";
                    fields += properties[i].Name;
                }
            }

            sql.Append($"select {fields} from [{table_name}]  ");

            #endregion 查询字段

            #region 查询条件

            if (where != null)
            {
                sql.Append($" where {where.ToString()}");
                //var t = whereParam.GetType();
                //var properties = t.GetProperties();
                //for (int i = 0; i < properties.Length; i++)
                //{
                //    if (i > 0)
                //        sql.Append($" and  ");
                //    sql.Append($" {properties[i].Name}=@{properties[i].Name}");
                //}
            }

            #endregion 查询条件

            #region 排序字段

            var order = "";
            if (orderBy != null)
            {
                var t = orderBy.GetType();
                var properties = t.GetProperties();
                for (int i = 0; i < properties.Length; i++)
                {
                    if (i > 0)
                        order += ",";
                    order += $" {properties[i].Name} {((Utils.ObjToBool(properties[i].GetValue(orderBy), false)) ? "asc" : "desc")}";
                }
            }
            else
            {
                var t = typeof(T);
                var properties = t.GetProperties();
                order += $" {properties[0].Name}  asc";
            }

            #endregion 排序字段

            var safeSql = GetAntiXssSql(sql.ToString());

            totalCount = con.Query<int>(PagingHelper.CreateCountingSql(safeSql), whereParam).First();
            var pagingSql = PagingHelper.CreatePagingSql(totalCount, pagesize, pageindex, safeSql, order);
            return con.Query<T>(pagingSql, whereParam).ToList();
        }

        /// <summary>
        /// 查询数据列表
        /// </summary>
        /// <typeparam name="T">查询对象</typeparam>
        /// <param name="con">数据库链接</param>
        /// <param name="fieldParam">查询字段,查询全部为null,查询某个字段，如：new {name="",id=0,status=0}</param>
        /// <param name="whereParam">查询条件，如：new {name="danny",status=1}</param>
        /// <param name="orderBy">排序字段 ,正序为true，降序为false，如：new {sort_id=false,id=true}</param>
        /// <returns></returns>
        public static List<T> GetList<T>(this SqlConnection con, object fieldParam = null, object whereParam = null, object orderBy = null)
        {
            var table_name = typeof(T).Name;

            #region 查询字段

            var fields = "*";
            StringBuilder sql = new StringBuilder();
            if (fieldParam != null)
            {
                fields = "";
                var t = fieldParam.GetType();
                var properties = t.GetProperties();
                for (int i = 0; i < properties.Length; i++)
                {
                    if (i > 0)
                        fields += ",";
                    fields += properties[i].Name;
                }
            }

            sql.Append($"select {fields} from [{table_name}]  ");

            #endregion 查询字段

            #region 查询条件

            if (whereParam != null)
            {
                sql.Append(" where ");
                var t = whereParam.GetType();
                var properties = t.GetProperties();
                for (int i = 0; i < properties.Length; i++)
                {
                    if (i > 0)
                        sql.Append($" and  ");
                    sql.Append($" {properties[i].Name}=@{properties[i].Name}");
                }
            }

            #endregion 查询条件

            #region 排序字段

            if (orderBy != null)
            {
                sql.Append(" order by ");
                var t = orderBy.GetType();
                var properties = t.GetProperties();
                for (int i = 0; i < properties.Length; i++)
                {
                    if (i > 0)
                        sql.Append($" , ");
                    sql.Append($" {properties[i].Name} {((Utils.ObjToBool(properties[i].GetValue(orderBy), false)) ? "asc" : "desc")}");
                }
            }

            #endregion 排序字段

            var safeSql = GetAntiXssSql(sql.ToString());
            return con.Query<T>(safeSql, whereParam).ToList();
        }

        /// <summary>
        /// Dapper获取列表
        /// </summary>
        /// <typeparam name="T">获取的列表类型</typeparam>
        /// <param name="con">直接调用DapperConnection类</param>
        /// <param name="sql">sql语句含orderby</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public static List<T> GetListBySql<T>(this SqlConnection con, string sql, object param = null)
        {
            var safeSql = GetAntiXssSql(sql);
            return con.Query<T>(safeSql, param).ToList();
        }

        /// <summary>
        /// 獲取匿名對象列表，請注意，使用此方法時，需要考慮字段順序，因為匿名對象沒有set方法，所有set操作需要走.ctor（構造函數），構造函數有順序敏感
        /// 獲取到的數據僅用於數據提供，無法修改，請注意！
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="con"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static List<T> GetListBySql<T>(this SqlConnection con, string sql, T model, object param = null)
        {
            var safeSql = GetAntiXssSql(sql);
            return con.Query<T>(safeSql, param).ToList();
        }

        /// <summary>
        /// 獲取匿名對象列表，請注意，使用此方法時，需要考慮字段順序，因為匿名對象沒有set方法，所有set操作需要走.ctor（構造函數），構造函數有順序敏感
        /// 獲取到的數據僅用於數據提供，無法修改，請注意！
        /// 此函數傳入的T，第一個屬性一定是row_number,請賦值Int64.MaxValue
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="con"></param>
        /// <param name="sql"></param>
        /// <param name="orderby"></param>
        /// <param name="pagesize"></param>
        /// <param name="pageindex"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public static List<T> GetDynamicList<T>(this SqlConnection con, string sql, T model, string orderby, int pagesize, int pageindex, out int totalCount, object param)
        {
            var safeSql = GetAntiXssSql(sql);
            totalCount = con.Query<int>(PagingHelper.CreateCountingSql(safeSql), param).First();
            var pagingSql = PagingHelper.CreatePagingSql(totalCount, pagesize, pageindex, safeSql, orderby);
            return con.Query<T>(pagingSql, param).ToList();
        }

        /// <summary>
        /// 獲取匿名對象列表，請注意，使用此方法時，需要考慮字段順序，因為匿名對象沒有set方法，所有set操作需要走.ctor（構造函數），構造函數有順序敏感
        /// 獲取到的數據僅用於數據提供，無法修改，請注意！
        /// 此函數傳入的T，第一個屬性一定是row_number,請賦值Int64.MaxValue
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="orderby"></param>
        /// <param name="pagesize"></param>
        /// <param name="pageindex"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public static List<T> GetDynamicList<T>(this SqlConnection con, string sql, T model, string orderby, int pagesize, int pageindex, out int totalCount)
        {
            var safeSql = GetAntiXssSql(sql);
            totalCount = con.Query<int>(PagingHelper.CreateCountingSql(safeSql)).First();
            var pagingSql = PagingHelper.CreatePagingSql(totalCount, pagesize, pageindex, safeSql, orderby);
            return con.Query<T>(pagingSql).ToList();
        }

        /// <summary>
        /// 获取总数
        /// </summary>
        /// <param name="param">sql里面用的参数，假设用到@a，则传入new {a='xxx'},传入对象即可</param>
        /// <returns></returns>
        public static int GetCount<T>(this SqlConnection con, object whereParam)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append($"select count(*) from {typeof(T).Name}");
            if (whereParam != null)
            {
                sql.Append(" where ");
                var whereField = whereParam.GetType().GetProperties();
                for (int i = 0; i < whereField.Length; i++)
                {
                    if (i > 0)
                        sql.Append(" and  ");
                    sql.Append($" {whereField[i].Name}=@{whereField[i].Name}");
                }
            }

            int count = con.Query<int>(sql.ToString(), whereParam).First();
            return count;
        }

        /// <summary>
        /// 获取总数
        /// </summary>
        /// <param name="param">sql里面用的参数，假设用到@a，则传入new {a='xxx'},传入对象即可</param>
        /// <returns></returns>
        public static int GetCount(this SqlConnection con, string sql, object parm)
        {
            int count = con.Query<int>(sql, parm).First();
            return count;
        }

        /// <summary>
        /// 获取单个值数据
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="param">sql里面用的参数，假设用到@a，则传入new {a='xxx'},传入对象即可</param>
        /// <returns></returns>
        public static object GetScalarValue(this SqlConnection con, string sql, object param)
        {
            return con.ExecuteScalar(sql, param);
        }

        #endregion 获取列表

        #region update方法

        /// <summary>
        /// update一个实体,实体是先查询出来的
        /// </summary>
        /// <param name="con">直接调用DapperConnection类</param>
        /// <param name="model">需要更新的模型</param>
        /// <param name="fieldWithOutUpdate">不需要更新的字段，主要是针对自增主键</param>
        /// <returns></returns>
        public static bool UpdateModel<T>(this SqlConnection con, T model, string fieldWithOutUpdate = "id", bool is_filter = true) where T : class
        {
            var updateParameterSql = GetUpdateParamSql(typeof(T), fieldWithOutUpdate);
            try
            {
                if (is_filter)
                {
                    model = ReturnSecurityObject(model) as T;
                }
                GetModelBySql<int>(con, updateParameterSql, model);
                return true;
            }
            catch
            { return false; }
        }

        /// <summary>
        /// 更新对象
        /// </summary>
        /// <typeparam name="T">对象</typeparam>
        /// <param name="con"></param>
        /// <param name="updateParam">set字段</param>
        /// <param name="whereParam">where字段</param>
        /// <param name="is_filter"></param>
        /// <returns></returns>
        public static bool UpdateModel<T>(this SqlConnection con, object updateParam, object whereParam, bool is_filter = true) where T : class, new()
        {
            T model = new T();
            StringBuilder sql = new StringBuilder();
            var t = typeof(T);
            var properties = t.GetProperties();
            sql.Append($"update [{t.Name}] set ");
            var updateField = updateParam.GetType().GetProperties();
            for (int i = 0; i < updateField.Length; i++)
            {
                if (i > 0)
                    sql.Append(",");
                sql.Append($" {updateField[i].Name}=@{updateField[i].Name}");
                for (int j = 0; j < properties.Length; j++)
                {
                    if (properties[j].Name.ToLower() == updateField[i].Name.ToLower())
                    {
                        properties[j].SetValue(model, updateField[i].GetValue(updateParam));
                        break;
                    }
                }
            }

            sql.Append(" where ");
            var whereField = whereParam.GetType().GetProperties();
            for (int i = 0; i < whereField.Length; i++)
            {
                if (i > 0)
                    sql.Append(" and  ");
                sql.Append($" {whereField[i].Name}=@{whereField[i].Name}");

                for (int j = 0; j < properties.Length; j++)
                {
                    if (properties[j].Name.ToLower() == whereField[i].Name.ToLower())
                    {
                        properties[j].SetValue(model, whereField[i].GetValue(whereParam));
                        break;
                    }
                }
            }

            try
            {
                if (is_filter)
                {
                    model = ReturnSecurityObject(model) as T;
                }
                int num = con.Execute(sql.ToString(), model);
                return num > 0;
            }
            catch (Exception ex)
            { return false; }
        }

        #endregion update方法

        #region 增删改 方法

        /// <summary>
        /// 删除对象
        /// </summary>
        /// <typeparam name="T">要删除的对象</typeparam>
        /// <param name="con"></param>

        /// <param name="whereParam"> 参数， 假设用到@a，则传入new {a='xxx'},传入对象即可</param>
        /// <returns></returns>
        public static bool DeleteModel<T>(this SqlConnection con, object whereParam)
        {
            if (whereParam == null)
                throw new Exception("参数不能为null");
            var sql = $"delete { typeof(T).Name}  where  ";
            var t = whereParam.GetType();
            var properties = t.GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                if (i > 0)
                    sql += $" and  ";

                sql += $" {properties[i].Name}=@{properties[i].Name}";
            }

            int num = con.Execute(sql, whereParam);
            return num > 0;
        }

        /// <summary>
        /// 执行一条sql语句(增删改)
        /// </summary>
        /// <param name="con">直接调用DapperConnection类</param>
        /// <param name="sql">TSQL语句</param>
        /// <param name="param">sql里面用的参数，假设用到@a，则传入new {a='xxx'},传入对象即可</param>
        /// <returns></returns>
        public static bool ExecuteSql(this SqlConnection con, string sql, object param)
        {
            string filterSql = GetAntiXssSql(sql);
            int num = con.Execute(filterSql, param);
            return num > 0;
        }

        #endregion 增删改 方法

        /// <summary>
        /// 执行有参数有返回值存储过程
        /// </summary>
        /// <param name="con"></param>
        /// <returns></returns>
        public static string ExecuteStoredProcedureWithParms(this SqlConnection con, string SPName, DynamicParameters dp)
        {
            dp.Add("@result", "", DbType.String, ParameterDirection.Output);
            con.Execute(SPName, dp, null, null, CommandType.StoredProcedure);
            string roleName = dp.Get<string>("@result");
            return roleName;
        }

        #region 事务处理

        /// <summary>
        /// 事务处理，用于增删改,不需要返回结果的sql语句
        /// </summary>
        /// <param name="con">直接调用DapperConnection类</param>
        /// <param name="sql">需要执行语句的集合</param>
        /// <returns></returns>
        public static bool ExecuteTransaction(this SqlConnection con, List<string> sql)
        {
            int num = 0;
            bool result = false;
            bool wasClosed = con.State == ConnectionState.Closed;
            if (wasClosed) con.Open();
            SqlTransaction tran = con.BeginTransaction();
            try
            {
                for (int i = 0; i < sql.Count; i++)
                {
                    num = con.Execute(sql[i], null, tran);
                }
                tran.Commit();
                result = true;
            }
            catch (Exception)
            {
                tran.Rollback();
            }
            finally
            {
                tran.Dispose();
                if (wasClosed) con.Close();
            }

            return result;
        }

        /// <summary>
        /// 带参数的事务处理，用于增删改
        /// </summary>
        /// <param name="con">直接调用DapperConnection类</param>
        /// <param name="sql">需要执行语句的集合</param>
        /// <returns></returns>
        public static bool ExecuteTransaction(this SqlConnection con, List<string> sql, List<object> param)
        {
            int num = 0;
            bool result = false;
            bool wasClosed = con.State == ConnectionState.Closed;
            if (wasClosed) con.Open();
            SqlTransaction tran = con.BeginTransaction();
            try
            {
                for (int i = 0; i < sql.Count; i++)
                {
                    num = con.Execute(sql[i], param[i], tran);
                }
                tran.Commit();
                result = true;
            }
            catch (Exception)
            {
                tran.Rollback();
            }
            finally
            {
                tran.Dispose();
                if (wasClosed) con.Close();
            }

            return result;
        }

        #endregion 事务处理

        #region 内部方法

        /// <summary>
        /// 设置主键
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="fieldWithOutInsert"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static T SetIdentify<T>(T model, string fieldWithOutInsert, int? value)
        where T : class
        {
            fieldWithOutInsert = fieldWithOutInsert.ToLower();
            var t = typeof(T);
            var properties = t.GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                if (properties[i].Name.ToLower() == fieldWithOutInsert)
                {
                    properties[i].SetValue(model, value);
                    break;
                }
            }
            var fields = t.GetFields();
            for (int i = 0; i < fields.Length; i++)
            {
                if (fields[i].Name.ToLower() == fieldWithOutInsert)
                {
                    fields[i].SetValue(model, value);
                    break;
                }
            }
            return model;
        }

        /// <summary>
        /// 返回安全的entity对象
        /// </summary>
        /// <typeparam name="T">entity对象的类型</typeparam>
        /// <param name="model">entity对象</param>
        /// <returns>安全的entity对象</returns>
        public static object ReturnSecurityObject(object model)
        {
            Type t = model.GetType();//获取类型
            foreach (PropertyInfo mi in t.GetProperties())//遍历该类型下所有属性（非字段，字段需要另一方法，好在EF都是属性
            {
                if (mi.PropertyType == "".GetType())//如果属性为string类型
                {
                    var inputString = (mi.GetValue(model) ?? "").ToString();
                    var sx = GetSafeText(inputString);//进行字符串过滤
                    sx = System.Web.HttpUtility.HtmlDecode(sx);
                    mi.SetValue(model, sx);//将过滤后的值设置给传入的对象
                }
            }
            return model;//返回安全对象
        }

        /// <summary>
        /// 获取更新sql
        /// </summary>
        /// <param name="type">需要更新的类型</param>
        /// <param name="fieldWithOutUpdate">需要更新的主键名</param>
        /// <returns></returns>
        public static string GetUpdateParamSql(Type type, string fieldWithOutUpdate)
        {
            var properties = type.GetProperties();
            var fields = type.GetFields();
            var paramSql = "update " + type.Name + " set ";
            fieldWithOutUpdate = (fieldWithOutUpdate ?? "").ToLower();
            if (properties != null && properties.Length > 0)
            {
                for (int i = 0; i < properties.Length; i++)
                {
                    var name = properties[i].Name;
                    if (fieldWithOutUpdate != (name.ToLower()))
                        paramSql += name + "=@" + name + ",";
                    else
                        fieldWithOutUpdate = name;
                }
            }
            if (fields != null && fields.Length > 0)
            {
                for (int i = 0; i < fields.Length; i++)
                {
                    var name = fields[i].Name;
                    if (fieldWithOutUpdate != (name.ToLower()))
                        paramSql += name + "=@" + name + ",";
                    else
                        fieldWithOutUpdate = name;
                }
            }
            return paramSql.TrimEnd(',') + string.Format(" where {0}=@{0}", fieldWithOutUpdate);
        }

        /// <summary>
        /// 获取insert子句 insert into table （xxx,yyy,zzz） values (@xxx,@yyy,@zzz)
        /// </summary>
        /// <param name="type">对象类型</param>
        /// <param name="fieldWithOutInsert">主键字段</param>
        /// <param name="new_table_name">自定义新的表名，不自动取type的</param>
        /// <param name="noInsert">不需要插入的地段</param>
        /// <returns></returns>
        public static string GetInsertParamSql(Type type, string fieldWithOutInsert, string new_table_name = "", List<string> noInsert = null, bool identity = true)
        {
            var properties = type.GetProperties();
            var fields = type.GetFields();
            var paramSql = string.Format("insert into [{0}] ", (new_table_name == "" ? type.Name : new_table_name));
            var paramString = string.Empty;
            var fieldString = string.Empty;
            var allFieldsName = new List<string>();
            fieldWithOutInsert = (fieldWithOutInsert ?? "").ToLower();
            if (properties != null && properties.Length > 0)
            {
                for (int i = 0; i < properties.Length; i++)
                {
                    if (noInsert != null)
                    {
                        foreach (var item in noInsert)
                            if (item.ToUpper() != (properties[i].Name.ToLower()))
                                allFieldsName.Add(properties[i].Name);
                    }
                    if (fieldWithOutInsert != (properties[i].Name.ToLower()))
                        allFieldsName.Add(properties[i].Name);
                }
            }
            if (fields != null && fields.Length > 0)
            {
                for (int i = 0; i < fields.Length; i++)
                {
                    if (noInsert != null)
                    {
                        foreach (var item in noInsert)
                            if (item.ToUpper() != (fields[i].Name.ToLower()))
                                allFieldsName.Add(fields[i].Name);
                    }
                    if (fieldWithOutInsert != (fields[i].Name.ToLower()))
                        allFieldsName.Add(fields[i].Name);
                }
            }
            allFieldsName.ForEach(x => { fieldString += x + ","; paramString += "@" + x + ","; });
            paramSql += string.Format("({0}) values ({1});", fieldString.TrimEnd(','), paramString.TrimEnd(','));
            if (identity)
                paramSql += "select @@identity;";
            return paramSql;
        }

        /// <summary>
        /// 获取防JS攻击的sql
        /// </summary>
        /// <param name="inputString">输入的sql</param>
        /// <returns></returns>
        private static string GetAntiXssSql(string inputString)
        {
            var sx = GetSafeText(inputString);//进行字符串过滤
            return System.Web.HttpUtility.HtmlDecode(sx);
        }

        #endregion 内部方法

        /// <summary>
        /// 过滤标记
        /// </summary>
        /// <param name="NoHTML">包括HTML，脚本，数据库关键字，特殊字符的源码 </param>
        /// <returns>已经去除标记后的文字</returns>
        public static string GetSafeText(string Htmlstring)
        {
            if (Htmlstring == null)
            {
                return "";
            }
            else
            {
                return Htmlstring.Trim();
            }
        }
    }
}