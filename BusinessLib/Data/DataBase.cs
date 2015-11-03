﻿using System.Linq;
using BusinessLib.Entity;
using BusinessLib.Extensions;

namespace BusinessLib.Data
{
    /*
    public enum DataType
    {
        Undefined = 0,
        Char = 1,
        VarChar = 2,
        Text = 3,
        NChar = 4,
        NVarChar = 5,
        NText = 6,
        Binary = 7,
        VarBinary = 8,
        Blob = 9,
        Image = 10,
        Boolean = 11,
        Guid = 12,
        SByte = 13,
        Int16 = 14,
        Int32 = 15,
        Int64 = 16,
        Byte = 17,
        UInt16 = 18,
        UInt32 = 19,
        UInt64 = 20,
        Single = 21,
        Double = 22,
        Decimal = 23,
        Money = 24,
        SmallMoney = 25,
        Date = 26,
        Time = 27,
        DateTime = 28,
        DateTime2 = 29,
        SmallDateTime = 30,
        DateTimeOffset = 31,
        Timestamp = 32,
        Xml = 33,
        Variant = 34,
        VarNumeric = 35,
        Udt = 36,
    }
    */

    public struct DataParameter
    {
        public DataParameter(string name, object value) { this.name = name; this.value = value; }

        private string name;
        private object value;

        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        public object Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
    }

    public class Paging<T>
    {
        public Paging(System.Collections.Generic.List<T> data, int currentPage, int count) { Data = data; CurrentPage = currentPage; Count = count; }

        public System.Collections.Generic.List<T> Data { get; set; }

        public int CurrentPage { get; set; }

        public int Count { get; set; }
    }

    public static class DataConnectionEx
    {
        public static System.Data.IDbCommand GetCommand(this IConnection connection, string commandText, System.Data.IDbTransaction t = null, System.Data.CommandType commandType = System.Data.CommandType.Text, params DataParameter[] parameter)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandType = commandType;
            cmd.CommandText = commandText;
            foreach (var item in parameter)
            {
                var p = cmd.CreateParameter();
                p.ParameterName = item.Name;
                p.Value = System.DateTime.MinValue.Equals(item.Value) ? System.DBNull.Value : item.Value ?? System.DBNull.Value;
                cmd.Parameters.Add(p);
            }
            cmd.Transaction = t;
            return cmd;
        }

        public static int ExecuteNonQuery(this IConnection connection, string commandText, System.Data.CommandType commandType = System.Data.CommandType.Text, params DataParameter[] parameter)
        {
            return connection.Execute((obj) =>
            {
                using (var cmd = connection.GetCommand(commandText, connection.Transaction, commandType, parameter))
                {
                    return cmd.ExecuteNonQuery();
                }
            });
        }

        public static T ExecuteScalar<T>(this IConnection connection, string commandText, System.Data.CommandType commandType = System.Data.CommandType.Text, params DataParameter[] parameter)
        {
            return connection.Execute<T>((obj) =>
            {
                using (var cmd = connection.GetCommand(commandText, connection.Transaction, commandType, parameter))
                {
                    return (T)cmd.ExecuteScalar();
                }
            }, minusOneExcep: false);
        }

        public static T Execute<T>(this IConnection connection, System.Func<object, T> func, System.Collections.IEnumerable obj = null, bool minusOneExcep = true)
        {
            bool isCreateTransaction = false;
            if (null == connection.Transaction) { connection.BeginTransaction(); isCreateTransaction = !isCreateTransaction; }

            try
            {
                if (null != obj)
                {
                    var count = 0;

                    foreach (var item in obj)
                    {
                        var result = func.Invoke(item);
                        var i = System.Convert.ToInt32(result);
                        if (-1 == i)
                        {
                            connection.Rollback();
                            throw new System.Exception("Affected the number of records -1");
                        }
                        count += i;
                    }

                    if (isCreateTransaction) { connection.Commit(); }
                    return (T)System.Convert.ChangeType(count, typeof(T));
                }
                else
                {
                    var result = func.Invoke(obj);

                    if (minusOneExcep && typeof(T).Equals(typeof(System.Int32)))
                    {
                        var count = System.Convert.ToInt32(result);
                        if (-1 == count)
                        {
                            connection.Rollback();
                            throw new System.Exception("Affected the number of records -1");
                        }
                    }

                    if (isCreateTransaction) { connection.Commit(); }
                    return (T)System.Convert.ChangeType(result, typeof(T));
                }
            }
            catch (System.Exception ex) { if (null != connection.Transaction) { connection.Rollback(); } throw ex; }
            finally { if (isCreateTransaction && null != connection.Transaction) { connection.Transaction.Dispose(); } }
        }

        public static Paging<T> GetPaging<T>(this IQueryable<T> query, int pageSize, int currentPage, int pageSizeMax = 30)
        {
            var _pageSize = System.Math.Min(pageSize, pageSizeMax);

            var count = query.Count();
            var countPage = System.Convert.ToInt32(System.Math.Ceiling(System.Convert.ToDouble(count) / System.Convert.ToDouble(_pageSize)));

            currentPage = currentPage < 0 ? 0 : currentPage > countPage ? countPage : currentPage;
            if (currentPage <= 0 && countPage > 0) { currentPage = 1; }

            return new Paging<T>(query.Skip(_pageSize * (currentPage - 1)).Take(_pageSize).ToList(), currentPage, count);
        }

        public static Paging<T> ToPaging<T>(this System.Collections.Generic.List<T> data, int currentPage, int count)
        {
            return new Paging<T>(data, currentPage, count);
        }

        public static IQueryable<T> SkipRandom<T>(this IQueryable<T> query)
        {
            return query.Skip(query.Count().Random());
        }
        //public static int Execute(this IConnection connection, object obj, System.Func<object, int> func)
        //{
        //    bool isCreateTransaction = false;
        //    if (null == connection.Transaction) { connection.BeginTransaction(); isCreateTransaction = !isCreateTransaction; }

        //    try
        //    {
        //        var i = func(obj);
        //        if (-1 == i) { connection.Rollback(); return i; }
        //        if (isCreateTransaction) { connection.Commit(); }
        //        return i;
        //    }
        //    catch (System.Exception ex) { if (null != connection.Transaction) { connection.Rollback(); } throw ex; }
        //    finally { if (isCreateTransaction && null != connection.Transaction) { connection.Transaction.Dispose(); } }
        //}
    }

    public abstract class DataBase<IConnection> : IData
        where IConnection : class, BusinessLib.Data.IConnection
    {
        public abstract IConnection GetConnection();

        Data.IConnection IData.GetConnection()
        {
            return GetConnection();
        }

        static T UseConnection<T>(System.Func<Data.IConnection> getConnection, System.Func<Data.IConnection, T> func)
        {
            using (var con = getConnection.Invoke()) { return func.Invoke(con); }
        }

        public int Save<T>(System.Collections.IEnumerable obj)
        {
            return UseConnection<int>(GetConnection, (con) => { return con.Save<T>(obj); });
        }

        public int Save<T>(T obj)
        {
            return UseConnection<int>(GetConnection, (con) => { return con.Save<T>(obj); });
        }

        public int SaveOrUpdate<T>(System.Collections.IEnumerable obj)
        {
            return UseConnection<int>(GetConnection, (con) => { return con.SaveOrUpdate<T>(obj); });
        }

        public int SaveOrUpdate<T>(T obj)
        {
            return UseConnection<int>(GetConnection, (con) => { return con.SaveOrUpdate<T>(obj); });
        }

        public int Update<T>(System.Collections.IEnumerable obj)
        {
            return UseConnection<int>(GetConnection, (con) => { return con.Update<T>(obj); });
        }

        public int Update<T>(T obj)
        {
            return UseConnection<int>(GetConnection, (con) => { return con.Update<T>(obj); });
        }

        public int Delete<T>(System.Collections.IEnumerable obj)
        {
            return UseConnection<int>(GetConnection, (con) => { return con.Delete<T>(obj); });
        }

        public int Delete<T>(T obj)
        {
            return UseConnection<int>(GetConnection, (con) => { return con.Delete<T>(obj); });
        }

        public int ExecuteNonQuery(string commandText, System.Data.CommandType commandType = System.Data.CommandType.Text, params DataParameter[] parameter)
        {
            return UseConnection<int>(GetConnection, (con) => { return con.ExecuteNonQuery(commandText, commandType, parameter); });
        }

        public T ExecuteScalar<T>(string commandText, System.Data.CommandType commandType = System.Data.CommandType.Text, params DataParameter[] parameter)
        {
            return UseConnection<T>(GetConnection, (con) => { return con.ExecuteScalar<T>(commandText, commandType, parameter); });
        }
    }

    public abstract class Entitys : IEntity
    {
        public abstract System.Linq.IQueryable<T> Get<T>()
            where T : class;

        public System.Linq.IQueryable<SysCompetence> SysCompetence { get { return Get<SysCompetence>(); } }
        public System.Linq.IQueryable<SysLogin> SysLogin { get { return Get<SysLogin>(); } }
        public System.Linq.IQueryable<SysRole> SysRole { get { return Get<SysRole>(); } }
        public System.Linq.IQueryable<SysRole_Competence> SysRole_Competence { get { return Get<SysRole_Competence>(); } }
        public System.Linq.IQueryable<SysAccount> SysAccount { get { return Get<SysAccount>(); } }
        public System.Linq.IQueryable<SysAccount_Role> SysAccount_Role { get { return Get<SysAccount_Role>(); } }
        public System.Linq.IQueryable<SysConfig> SysConfig { get { return Get<SysConfig>(); } }
        public System.Linq.IQueryable<SysLog> SysLog { get { return Get<SysLog>(); } }
    }
}