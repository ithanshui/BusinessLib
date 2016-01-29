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
using LinqToDB;

namespace Business.Data
{
    /// <summary>
    /// the a LinqToDBConnection
    /// </summary>
    /// <typeparam name="IEntity"></typeparam>
    public abstract class LinqToDBConnection<IEntity> : LinqToDB.Data.DataConnection, Data.IConnection
        where IEntity : class, Data.IEntity
    {
        public LinqToDBConnection(LinqToDB.DataProvider.IDataProvider provider, string conString)
            : base(provider, conString) { }

        public abstract IEntity Entity { get; }

        Data.IEntity IConnection.Entity
        {
            get { return Entity; }
        }

        public new void BeginTransaction()
        {
            base.BeginTransaction();
        }

        public new void BeginTransaction(System.Data.IsolationLevel isolationLevel)
        {
            base.BeginTransaction(isolationLevel);
        }

        public void Commit()
        {
            base.CommitTransaction();
        }

        public void Rollback()
        {
            base.RollbackTransaction();
        }

        public int Save<T>(System.Collections.IEnumerable obj)
        {
            return this.Execute((obj1) => { return DataExtensions.Insert<T>(this, (T)obj1); }, obj);
        }

        public int Save<T>(T obj)
        {
            return this.Execute((obj1) => { return DataExtensions.Insert<T>(this, (T)obj); });
        }

        public int SaveOrUpdate<T>(System.Collections.IEnumerable obj)
        {
            return this.Execute((obj1) => { return DataExtensions.InsertOrReplace<T>(this, (T)obj1); }, obj);
        }

        public int SaveOrUpdate<T>(T obj)
        {
            return this.Execute((obj1) => { return DataExtensions.InsertOrReplace<T>(this, (T)obj); });
        }

        public int Update<T>(System.Collections.IEnumerable obj)
        {
            return this.Execute((obj1) => { return DataExtensions.Update<T>(this, (T)obj1); }, obj);
        }

        public int Update<T>(T obj)
        {
            return this.Execute((obj1) => { return DataExtensions.Update<T>(this, (T)obj); });
        }

        public int Delete<T>(System.Collections.IEnumerable obj)
        {
            return this.Execute((obj1) => { return DataExtensions.Delete<T>(this, (T)obj1); }, obj);
        }

        public int Delete<T>(T obj)
        {
            return this.Execute((obj1) => { return DataExtensions.Delete<T>(this, (T)obj); });
        }

        public void BulkCopy<T>(System.Collections.Generic.IEnumerable<T> source)
        {
            LinqToDB.Data.DataConnectionExtensions.BulkCopy<T>(this, source);
        }

        public T Execute<T>(string sql, params DataParameter[] parameters)
        {
            return LinqToDB.Data.DataConnectionExtensions.Execute<T>(this, sql, parameters.Select(c => new LinqToDB.Data.DataParameter(c.Name, c.Value)).ToArray());
        }

        public int Execute(string sql, params DataParameter[] parameters)
        {
            return LinqToDB.Data.DataConnectionExtensions.Execute(this, sql, parameters.Select(c => new LinqToDB.Data.DataParameter(c.Name, c.Value)).ToArray());
        }

        public int ExecuteNonQuery(string commandText, System.Data.CommandType commandType = System.Data.CommandType.Text, params DataParameter[] parameter)
        {
            return DataConnectionEx.ExecuteNonQuery(this, commandText, commandType, parameter);
        }

        public T ExecuteScalar<T>(string commandText, System.Data.CommandType commandType = System.Data.CommandType.Text, params DataParameter[] parameter)
        {
            return DataConnectionEx.ExecuteScalar<T>(this, commandText, commandType, parameter);
        }

        public new void Dispose()
        {
            base.DisposeCommand();
            base.Dispose();
            if (null != base.Transaction) { base.Transaction.Dispose(); }
            if (null != base.Connection) { base.Connection.Dispose(); }
        }
    }
}
