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

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Business.Data
{
    [BsonIgnoreExtraElements]
    public class MongoDBEntity : Business.Data.EntityBase
    {
        ObjectId id = ObjectId.GenerateNewId();
        public ObjectId _id { get { return id; } set { id = value; } }
    }

    public abstract class MongoDBConnection<IEntity> : Data.IConnection
        where IEntity : class, Data.IEntity
    {
        const string IdName = "_id";

        static ObjectId GetId<T>(T obj)
        {
            var name = typeof(T).FullName;

            var accessor = EntityBase.MetaData[name].Accessor;
            if (!accessor.ContainsKey(IdName))
            {
                throw new System.MissingMemberException(IdName);
            }

            var idMember = accessor[IdName];
            if (!typeof(ObjectId).Equals(idMember.Item1))
            {
                throw new System.MissingMemberException(IdName);
            }

            return (ObjectId)idMember.Item2(obj);
        }

        IMongoClient server;
        string dbName;

        static IMongoCollection<T> GetCollection<T>(IMongoClient server, string dbName)
        {
            return server.GetDatabase(dbName).GetCollection<T>(typeof(T).Name);
        }

        public MongoDBConnection(IMongoClient server, string dbName)
        {
            this.server = server;
            this.dbName = dbName;
        }

        public System.Data.IDbCommand CreateCommand()
        {
            throw new NotImplementedException();
        }

        public abstract IEntity Entity { get; }

        Data.IEntity IConnection.Entity
        {
            get { return Entity; }
        }

        public void Dispose()
        {
            server = null;
        }

        public System.Data.IDbTransaction Transaction
        {
            get { throw new NotImplementedException(); }
        }

        public void BeginTransaction()
        {
            throw new NotImplementedException();
        }

        public void BeginTransaction(System.Data.IsolationLevel isolationLevel)
        {
            throw new NotImplementedException();
        }

        public void Commit()
        {
            throw new NotImplementedException();
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }

        public int Save<T>(IEnumerable<T> obj)
        {
            var collection = GetCollection<T>(this.server, dbName);

            collection.InsertMany(obj);

            return obj.Count();
        }

        public int Save<T>(T obj)
        {
            var collection = GetCollection<T>(this.server, dbName);

            collection.InsertOne(obj);

            return 1;
        }

        public int SaveOrUpdate<T>(IEnumerable<T> obj)
        {
            foreach (var item in obj) { SaveOrUpdate(item); }
            return obj.Count();
        }

        public int SaveOrUpdate<T>(T obj)
        {
            var id = GetId(obj);

            var collection = GetCollection<T>(this.server, dbName);

            var result = collection.ReplaceOne(new BsonDocument(IdName, id), obj);

            if (0 == result.MatchedCount)
            {
                collection.InsertOne(obj);
            }

            return 1;
        }

        public int Update<T>(IEnumerable<T> obj)
        {
            var id = GetId(obj);

            var collection = GetCollection<T>(this.server, dbName);

            var count = 0;

            foreach (var item in obj)
            {
                count += (int)collection.ReplaceOne(new BsonDocument(IdName, id), item).MatchedCount;
            }

            return count;
        }

        public int Update<T>(T obj)
        {
            var id = GetId(obj);

            var collection = GetCollection<T>(this.server, dbName);

            return (int)collection.ReplaceOne(new BsonDocument(IdName, id), obj).MatchedCount;
        }

        public int Delete<T>(IEnumerable<T> obj)
        {
            var count = 0;
            foreach (var item in obj) { count += Delete(item); }
            return count;
        }

        public int Delete<T>(T obj)
        {
            var id = GetId(obj);

            var collection = GetCollection<T>(this.server, dbName);
            return (int)collection.DeleteOne(new BsonDocument(IdName, id)).DeletedCount;
        }
    }
}
