using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using System.Linq;
using System.Collections.Generic;

namespace UnitTest
{
    public class Data1 : Business.Data.DataBase<DataConnection1>
    {
        public override DataConnection1 GetConnection()
        {
            return new DataConnection1();
        }
    }
    public class DataConnection1 : Business.Data.MongoDBConnection<Entitys1>
    {
        readonly static MongoClient server = new MongoClient("mongodb://127.0.0.1:27017");

        public DataConnection1() : base(server, "TestDb") { }

        public override Entitys1 Entity
        {
            get { return new Entitys1(server, "TestDb"); }
        }
    }
    public class Entitys1 : Business.Data.Entitys
    {
        readonly IMongoClient server;
        readonly string dbName;

        public Entitys1(IMongoClient server, string dbName)
        {
            this.server = server;
            this.dbName = dbName;
        }

        public IQueryable<songs> songs { get { return Get<songs>(); } }

        public override IQueryable<T> Get<T>()
        {
            return server.GetDatabase(dbName).GetCollection<T>(typeof(T).Name).AsQueryable();
        }
    }

    public class songs1 : Business.Data.MongoDBEntity
    {
        public string songs_name { get; set; }

        public string songs_passwd { get; set; }
    }

    [TestClass]
    public class TestMongoDB
    {
        [TestMethod]
        public void TestMethod1()
        {
            var data = new Data1();

            using (var con = data.GetConnection())
            {
                var list = new List<songs1>();
                var s = new songs1 { songs_name = "name1", songs_passwd = "passwd1" };
                var s1 = new songs1 { songs_name = "name2", songs_passwd = "passwd2" };
                list.Add(s);
                list.Add(s1);

                con.Save<songs1>(list);

                con.Delete<songs1>(list);
            }
        }
    }
}
