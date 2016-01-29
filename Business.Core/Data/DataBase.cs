/*==================================
             ########   
            ##########           
                                              
             ########             
            ##########            
          ##############         
         #######  #######        
        ######      ######       
        #####        #####       
        ####          ####       
        ####   ####   ####       
        #####  ####  #####       
         ################        
          ##############                                                 
==================================*/

using System.Linq;

namespace Business.Data
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

        readonly string name;
        readonly object value;

        public string Name { get { return name; } }

        public object Value { get { return value; } }
    }

    [ProtoBuf.ProtoContract(SkipConstructor = true)]
    public struct Paging<T> : Auth.ISerialize
    {
        public static implicit operator Paging<T>(string value)
        {
            return Extensions.Help.JsonDeserialize<Paging<T>>(value);
        }
        public static implicit operator Paging<T>(byte[] value)
        {
            return Extensions.Help.ProtoBufDeserialize<Paging<T>>(value);
        }

        [ProtoBuf.ProtoMember(1, Name = "D")]
        public System.Collections.Generic.List<T> Data { get; set; }

        [ProtoBuf.ProtoMember(2, Name = "P")]
        public int CurrentPage { get; set; }

        [ProtoBuf.ProtoMember(3, Name = "C")]
        public int Count { get; set; }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

        public byte[] ToBytes()
        {
            return Extensions.Help.ProtoBufSerialize(this);
        }
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
            var count = query.Count();
            if (0 == count) { return new Paging<T> { Data = new System.Collections.Generic.List<T>() }; }

            var _pageSize = System.Math.Min(pageSize, pageSizeMax);

            var countPage = System.Convert.ToInt32(System.Math.Ceiling(System.Convert.ToDouble(count) / System.Convert.ToDouble(_pageSize)));

            currentPage = currentPage < 0 ? 0 : currentPage > countPage ? countPage : currentPage;
            if (currentPage <= 0 && countPage > 0) { currentPage = 1; }

            return new Paging<T> { Data = query.Skip(_pageSize * (currentPage - 1)).Take(_pageSize).ToList(), CurrentPage = currentPage, Count = count };
        }

        public static Paging<T> ToPaging<T>(this System.Collections.Generic.List<T> data, int currentPage, int count)
        {
            return new Paging<T> { Data = data, CurrentPage = currentPage, Count = count };
        }

        public static IQueryable<T> SkipRandom<T>(this IQueryable<T> query, int take = 0)
        {
            if (0 < take)
            {
                query = query.Skip(Extensions.Help.Random(query.Count() - take)).Take(take);
            }
            else
            {
                query = query.Skip(Extensions.Help.Random(query.Count()));
            }
            return query;
        }
    }

    public abstract class DataBase<IConnection> : IData
        where IConnection : class, Data.IConnection
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
    }
}