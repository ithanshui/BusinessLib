using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLib.Extensions
{
    using BusinessLib.Data.Entity;

    public static class DataEx
    {
        public static System.Data.Common.DbConnection GetConnection(Common.Data.ADO.DataBaseType dataBaseType, System.String connectionString) { return Common.Data.ADO.SqlHelp.GetConnection(dataBaseType, connectionString); }

        public static List<T> GetData<T>(this System.Data.Common.DbConnection connection, string tblName, string getFields, string filter, string group = "", string order = "", System.Data.CommandBehavior commandBehavior = System.Data.CommandBehavior.Default, System.Data.Common.DbTransaction t = null, params System.Data.Common.DbParameter[] parameter)
        {
            if (null == tblName || System.String.IsNullOrEmpty(tblName.Trim())) { return null; }

            using (var ds = Common.Data.ADO.SqlHelp.ExecuteReadOnlyDataSet(connection, string.Format("SELECT {4} FROM {0}{1}{2}{3}",
                        tblName,
                        (null == filter || System.String.IsNullOrEmpty(filter.Trim())) ? System.String.Empty : string.Format(" WHERE {0}", filter),
                        (null == group || System.String.IsNullOrEmpty(group.Trim())) ? System.String.Empty : string.Format(" GROUP BY {0}", group),
                        (null == order || System.String.IsNullOrEmpty(order.Trim())) ? System.String.Empty : string.Format(" ORDER BY {0}", order),
                        System.String.IsNullOrEmpty(getFields) ? "*" : getFields),
                        commandBehavior, t, null, parameter))
            {
                return ds.Tables[0].ToList<T>();
            }
        }
        public static object[] GetDataPaging<T>(this System.Data.Common.DbConnection connection, string tblName, string key, string order, string getFields = "*", string filter = "", int pageSize = 100, int currentPage = 1, params System.Data.Common.DbParameter[] parameter)
        {
            if (null == tblName || System.String.IsNullOrEmpty(tblName.Trim())) { return null; }

            using (var ds = Common.Data.ADO.SqlHelp.ExecuteReadOnlyDataSet(connection, string.Format("SELECT * FROM {0}", tblName), System.Data.CommandBehavior.SchemaOnly, null, null))
            {
                order = null == order || System.String.IsNullOrEmpty(order) ? ds.Tables[0].PrimaryKey.Length > 0 ? ds.Tables[0].PrimaryKey[0].ColumnName : ds.Tables[0].Columns[0].ColumnName : order;
                var paging = Common.Data.ADO.SqlHelp.GetPaging(connection, tblName, key, order, getFields, pageSize, currentPage, false, filter);
                //...//
                var count = System.Convert.ToInt32(Common.Data.ADO.SqlHelp.ExecuteScalar(connection, Common.Data.ADO.SqlHelp.GetPaging(connection, tblName, order, getCount: true, where: filter), null, parameter));

                var _parameter = new System.Data.Common.DbParameter[parameter.Length];
                for (int i = 0; i < parameter.Length; i++) { _parameter.SetValue(connection.GetParameter(parameter[i].ParameterName, parameter[i].Value), i); }

                var countPage = System.Convert.ToInt32(System.Math.Ceiling(System.Convert.ToDouble(count) / System.Convert.ToDouble(pageSize)));
                if (currentPage < 0) { currentPage = 0; }
                if (currentPage > countPage) { currentPage = countPage; }
                if (currentPage <= 0 && countPage > 0) { currentPage = 1; }
                var rString = new object[3];
                rString.SetValue(System.Convert.ToString(currentPage), 1);
                rString.SetValue(System.Convert.ToString(count), 2);
                //...此处返回正确 currentPage
                using (var ds1 = Common.Data.ADO.SqlHelp.ExecuteReadOnlyDataSet(connection, paging, null, null, null, _parameter))
                {
                    rString.SetValue(ds1.Tables[0].ToList<T>(), 0);
                    return rString;
                }
            }
        }

        public static int SaveOrUpdate<T>(this System.Data.Common.DbConnection connection, IEnumerable<T> entity, System.Data.Common.DbTransaction t = null, bool isDelete = false)
        {
            if (null == entity || 0 == entity.Count()) { return 0; }

            using (var dt = entity.ToDataTable<T>(connection, t))
            {
                if (isDelete)
                {
                    dt.AcceptChanges();
                    foreach (var row in dt.Rows) { row.Delete(); }
                }
                return Common.Data.ADO.SqlHelp.SaveOrUpdate(connection, dt, dt.TableName, false, t);
            }
        }
        public static int SaveOrUpdate<T>(this System.Data.Common.DbConnection connection, T entity, System.Data.Common.DbTransaction t = null, bool isDelete = false)
        {
            if (null == entity) { return 0; }

            using (var dt = entity.ToDataTable<T>(connection, t))
            {
                if (isDelete)
                {
                    dt.AcceptChanges();
                    foreach (var row in dt.Rows) { row.Delete(); }
                }
                return Common.Data.ADO.SqlHelp.SaveOrUpdate(connection, dt, dt.TableName, false, t);
            }
        }

        public static int ExecuteNonQuery(this System.Data.Common.DbConnection connection, string commandText, System.Data.Common.DbTransaction t = null, System.Data.CommandType commandType = System.Data.CommandType.Text, params System.Data.Common.DbParameter[] parameter)
        {
            using (var cmd = GetCommand(connection, commandText, t, commandType, parameter))
            {
                if (cmd.Connection.State == System.Data.ConnectionState.Closed) { cmd.Connection.Open(); }
                return cmd.ExecuteNonQuery();
            }
        }

        public static object ExecuteScalar(this System.Data.Common.DbConnection connection, string commandText, System.Data.Common.DbTransaction t = null, System.Data.CommandType commandType = System.Data.CommandType.Text, params System.Data.Common.DbParameter[] parameter)
        {
            using (var cmd = GetCommand(connection, commandText, t, commandType, parameter))
            {
                if (cmd.Connection.State == System.Data.ConnectionState.Closed) { cmd.Connection.Open(); }
                return cmd.ExecuteScalar();
            }
        }

        public static System.Data.Common.DbCommand GetCommand(this System.Data.Common.DbConnection connection, string commandText, System.Data.Common.DbTransaction t = null, System.Data.CommandType commandType = System.Data.CommandType.Text, params System.Data.Common.DbParameter[] parameter)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandType = commandType;
            cmd.CommandText = commandText;
            cmd.Parameters.AddRange(parameter);
            cmd.Transaction = t;
            return cmd;
        }

        public static System.Data.Common.DbParameter GetParameter(this System.Data.Common.DbConnection connection, string parameterName, object value)
        {
            using (var cmd = connection.CreateCommand())
            {
                var parameter = cmd.CreateParameter();
                parameter.ParameterName = parameterName;
                parameter.Value = value ?? System.DBNull.Value;
                return parameter;
            }
        }
    }
}
