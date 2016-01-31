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

namespace Business.Data
{
    public interface ITransaction : System.IDisposable
    {
        System.Data.IDbTransaction Transaction { get; }

        void BeginTransaction();

        void BeginTransaction(System.Data.IsolationLevel isolationLevel);

        void Commit();

        void Rollback();
    }

    public interface IDataConnectionEx
    {
        int ExecuteNonQuery(string commandText, System.Data.CommandType commandType = System.Data.CommandType.Text, params DataParameter[] parameter);

        T ExecuteScalar<T>(string commandText, System.Data.CommandType commandType = System.Data.CommandType.Text, params DataParameter[] parameter);

        T Execute<T>(string sql, params  DataParameter[] parameters);

        int Execute(string sql, params  DataParameter[] parameters);

        void BulkCopy<T>(System.Collections.Generic.IEnumerable<T> source);
    }

    public interface IConnection : System.IDisposable, ITransaction, IData2, IDataConnectionEx
    {
        System.Data.IDbCommand CreateCommand();

        IEntity Entity
        {
            get;
        }
    }

    public interface IData2
    {
        int Save<T>(System.Collections.IEnumerable obj);

        int Save<T>(T obj);

        int SaveOrUpdate<T>(System.Collections.IEnumerable obj);

        int SaveOrUpdate<T>(T obj);

        int Update<T>(System.Collections.IEnumerable obj);

        int Update<T>(T obj);

        int Delete<T>(System.Collections.IEnumerable obj);

        int Delete<T>(T obj);
    }

    public interface IData : IData2
    {
        IConnection GetConnection();
    }

    public interface IEntity
    {
        System.Linq.IQueryable<T> Get<T>() where T : class;
    }
}
