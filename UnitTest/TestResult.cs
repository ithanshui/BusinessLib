using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;
using Business.Authentication;
using Business.Result;
using Business.Attributes;
using System.Collections.Generic;
using ProtoBuf;
using Business;
using Business.Extensions;
using NLog;
using LinqToDB.Mapping;
using LinqToDB;
using LinqToDB.DataProvider;
using System.Linq;
using Newtonsoft.Json;

namespace UnitTest
{
    #region Data

    public class Data : Business.Data.DataBase<DataConnection>
    {
        public override DataConnection GetConnection()
        {
            return new DataConnection();
        }
    }

    public class DataConnection : Business.Data.LinqToDBConnection<Entitys>
    {
        readonly static IDataProvider provider = new LinqToDB.DataProvider.MySql.MySqlDataProvider();
        readonly static string conString = "server=192.168.0.110;uid=test;pwd=123456;database=test";

        public DataConnection()
            : base(provider, conString) { }

        public override Entitys Entity
        {
            get { return new Entitys(this); }
        }
    }

    public class Entitys : Business.Data.Entitys
    {
        readonly IDataContext con;
        public Entitys(IDataContext con)
        { this.con = con; }

        public System.Linq.IQueryable<songs> songs { get { return Get<songs>(); } }

        public override IQueryable<T> Get<T>()
        {
            return this.con.GetTable<T>();
        }
    }

    #region Entitys

    [Table(Name = "songs")]
    public class songs : Business.Entity.EntityBase
    {
        [Column(Name = "songs_name"), NotNull]
        public string songs_name { get; set; }

        [Column(Name = "songs_passwd"), NotNull]
        public string songs_passwd { get; set; }
    }

    #endregion

    #endregion

    #region Common

    public static class Common
    {
        public static Data OnlyData = new Data();

        public static Business.Cache.CacheBase OnlyCache = new Business.Cache.CacheBase();

        static ConfigurationOptions configurationOptions = new ConfigurationOptions { EndPoints = { { "192.168.0.110", 6379 } }, KeepAlive = 180, Password = "", AllowAdmin = true };

        public static Business.Cache.Redis OnlyRedis = new Business.Cache.Redis(configurationOptions);

        public static Business.Log.NLogAdapter OnlyLog = new Business.Log.NLogAdapter(LogManager.GetCurrentClassLogger());

        static Business.Authentication.Interceptor<ResultProtoBuf<string>, Session<List<string>>> Authentication = new Interceptor<ResultProtoBuf<string>, Session<List<string>>>();
        public static Business.Extensions.InterceptorBind<BusinessMember> Interceptor = new InterceptorBind<BusinessMember>(Authentication);

        static Business.Authentication.Interceptor Authentication1 = new Interceptor();
        public static Business.Extensions.InterceptorBind<BusinessMember1> Interceptor1 = new InterceptorBind<BusinessMember1>(Authentication1);

        public static IResult<DataType> GetResult<DataType>(this DataType data)
        {
            return new ResultProtoBuf<DataType>();
        }
    }

    [ProtoBuf.ProtoContract(SkipConstructor = true)]
    public class ResultProtoBuf<DataType> : ResultBase<DataType>
    {
        public static implicit operator ResultProtoBuf<DataType>(string value)
        {
            return Help.JsonDeserialize<ResultProtoBuf<DataType>>(value);
        }
        public static implicit operator ResultProtoBuf<DataType>(byte[] value)
        {
            return Help.ProtoBufDeserialize<ResultProtoBuf<DataType>>(value);
        }

        public ResultProtoBuf()
        {
            var type = typeof(DataType);
            if (type.IsValueType)
            {
                this.Data = default(DataType);
            }
        }

        [ProtoBuf.ProtoMember(1, Name = "S")]
        [Newtonsoft.Json.JsonProperty(PropertyName = "S")]
        public override System.Int32 State { get; set; }

        [ProtoBuf.ProtoMember(2, Name = "M")]
        [Newtonsoft.Json.JsonProperty(PropertyName = "M")]
        public override System.String Message { get; set; }

        DataType data;
        [ProtoBuf.ProtoMember(3, Name = "D")]
        [Newtonsoft.Json.JsonProperty(PropertyName = "D")]
        public override DataType Data { get; set; }

        public override byte[] ToBytes()
        {
            using (var stream = new System.IO.MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(stream, this);
                return stream.ToArray();
            }
        }
    }

    #endregion

    public class Parameters
    {
        [ProtoBuf.ProtoContract(SkipConstructor = true)]
        //[Deserialize(TrimAllChar = true)]
        //[ProtoBuf(TrimAllChar = true)]
        [Json(TrimAllChar = true)]
        public struct Register
        {
            [ProtoBuf.ProtoMember(1)]
            [CanNotNull(-11)]
            [Size(12, "\"account\" length verification failed. min 4, max 8", Min = 4, Max = "8")]
            [CheckChar(-13, Mode = CheckCharAttribute.CheckCharMode.All)]
            public string account;

            [ProtoBuf.ProtoMember(2)]
            [CanNotNull(-14)]
            [Size(15, Min = 4, Max = 8)]
            public string password;

            [ProtoBuf.ProtoMember(3)]
            [CheckEmail(16)]
            [Size(-17, Min = 4, Max = 32)]
            public string email;

            [ProtoBuf.ProtoMember(4)]
            public List<string> d1;

            [ProtoBuf.ProtoMember(5)]
            [Size(-19, Min = 4, Max = 8)]
            public int[] d2;

            public override string ToString()
            {
                return JsonConvert.SerializeObject(this);
            }

            public byte[] ToBytes()
            {
                return Help.ProtoBufSerialize(this);
            }
        }
    }

    [ProtoBuf.ProtoContract(SkipConstructor = true)]
    public struct Result
    {
        [ProtoBuf.ProtoMember(1)]
        public string account { get; set; }
        [ProtoBuf.ProtoMember(2)]
        public string email { get; set; }
    }

    //[BusinessLog(true)]
    public class BusinessMember : BusinessBase<Data, Business.Log.NLogAdapter, Business.Cache.ICache>
    {
        public BusinessMember()
            : base(Common.OnlyData, Common.OnlyLog, Common.OnlyCache, () =>
            { })
        {
            this.WriteLogAsync = x =>
            {
                System.Console.WriteLine(x);
            };
        }

        #region Optional override Login Session Token Competence

        public override string Login(string value, out string error, string cmdId = null)
        {
            error = System.String.Empty;

            try
            {
                if (System.String.IsNullOrEmpty(value)) { return System.String.Empty; }

                Session<List<string>> session = value;
                session.Data = new List<string> { "111", "222" };

                var time = System.DateTime.Now;

                //-----Start writing business logic-----//

                //Add permissions
                //session.Competences = new List<string>();

                //-----Writing business logic end-----//

                //send value-key
                var key = System.String.IsNullOrEmpty(cmdId) ? System.Guid.NewGuid().ToString("N") : cmdId;
                session.Key = string.Format("Session_{0}", key);

                session.Time = time;
                this.Cache.Set(session.Key, session.ToBytes(), TimeSpan.FromMinutes(1440));
                //Relation key
                this.Cache.Set(session.Account, session.Key, TimeSpan.FromMinutes(1440));

                return key;
            }
            catch (System.Exception ex)
            {
                this.WriteLogAsync.BeginInvoke(new BusinessLogData(BusinessLogType.Exception, "Login", "Sys", value, ex, 0, "Login", null), null, null);

                error = JsonConvert.SerializeObject(ex);//allow hide or show!
                return System.String.Empty;//hide to log!
            }
        }

        public override IToken GetToken(object token)
        {
            if (null == token) { return null; }

            var _token = Help.JsonDeserialize<Token>(System.Convert.ToString(token));
            if (null == _token) { return null; }

            return _token;
        }

        public override Session GetSession<Session>(Business.Authentication.IToken token)
        {
            if (null == token) { return null; }

            var cacheValue = this.Cache.Get(string.Format("Session_{0}", token.Key));
            if (!cacheValue.HasValue) { return null; }

            var session = Help.ProtoBufDeserialize<Session>(cacheValue);

            if (!System.Object.Equals(session.IP, token.IP))
            {
                session.IP = token.IP; this.Cache.Set(session.Key, session.ToBytes());
            }

            return session;
        }

        public override bool CheckCompetence(ISession session, string competence)
        {
            return !(null != session.Competences && !session.Competences.Contains(competence));
        }

        #endregion

        public virtual void Test1(object token, object arguments = null) { }
        public virtual IResult Test2(object token, object arguments = null, ISession<List<string>> session = null) { return ResultFactory.Create(); }

        //[Command("H2", ResultDataType = CommandAttribute.DataType.ProtoBuf)]
        [ProtoBufCommand("H2", ResultDataType = CommandAttribute.DataType.ProtoBuf)]
        //[JsonCommand("H2", ResultDataType = CommandAttribute.DataType.ProtoBuf)]
        public virtual IResult Test3(object token, object arguments = null, ISession<List<string>> session = null, Parameters.Register ags = default(Parameters.Register))
        {
            //data
            /*
            using (var con = this.DB.GetConnection())
            {
                con.BeginTransaction();

                var songs = con.Entity.songs.Where(c => c.songs_name == "eeee");
                foreach (var item in songs)
                {
                    System.Console.WriteLine(string.Format("{0}|{1}", item.songs_name, item.songs_passwd));
                }

                con.Commit();
            }
            */
            //cache
            this.Cache.Set("222", "333");
            this.Cache.Set("2qw", "333");
            this.Cache.Set("3qw", "333");

            this.Cache.Remove("222");
            this.Cache.Remove("2qw");
            this.Cache.Remove("3qw");

            this.Cache.Remove(session.Key);
            this.Cache.Remove(session.Account);

            //log
            this.Log.Debug(ags.account);

            //result
            var resultData = new List<Result> { new Result { account = "aaa", email = "bbb" }, new Result { account = "ccc", email = "ddd" } }.Select(c => new { c.account, c.email });

            var result1 = ResultFactory.Create(resultData);

            var result2 = ResultFactory.Create(resultData, resultData.GetResult());
            //var result2 = new ResultProtoBuf { State = 1, Data = resultData };

            return result2;
        }
    }

    public class BusinessMember1 : BusinessBase<Data, Business.Log.NLogAdapter, Business.Cache.ICache>
    {
        public BusinessMember1()
            : base(Common.OnlyData, Common.OnlyLog, Common.OnlyCache, () =>
            { })
        {
            this.WriteLogAsync = x =>
            {
                System.Console.WriteLine(x);
            };
        }

        #region Optional override Login Session Token Competence

        public override string Login(string value, out string error, string cmdId = null)
        {
            error = System.String.Empty;

            try
            {
                if (System.String.IsNullOrEmpty(value)) { return System.String.Empty; }

                Session session = value;

                var time = System.DateTime.Now;

                //-----Start writing business logic-----//

                //Add permissions
                //session.Competences = new List<string>();

                //-----Writing business logic end-----//

                //send value-key
                var key = System.String.IsNullOrEmpty(cmdId) ? System.Guid.NewGuid().ToString("N") : cmdId;
                session.Key = string.Format("Session_{0}", key);

                session.Time = time;
                this.Cache.Set(session.Key, session.ToBytes(), TimeSpan.FromMinutes(1440));
                //Relation key
                this.Cache.Set(session.Account, session.Key, TimeSpan.FromMinutes(1440));

                return key;
            }
            catch (System.Exception ex)
            {
                this.WriteLogAsync.BeginInvoke(new BusinessLogData(BusinessLogType.Exception, "Login", "Sys", value, ex, 0, "Login", null), null, null);

                error = JsonConvert.SerializeObject(ex);//allow hide or show!
                return System.String.Empty;//hide to log!
            }
        }

        public override IToken GetToken(object token)
        {
            if (null == token) { return null; }

            var _token = Help.JsonDeserialize<Token>(System.Convert.ToString(token));
            if (null == _token) { return null; }

            return _token;
        }

        public override Session GetSession<Session>(Business.Authentication.IToken token)
        {
            if (null == token) { return null; }

            var cacheValue = this.Cache.Get(string.Format("Session_{0}", token.Key));
            if (!cacheValue.HasValue) { return null; }

            var session = Help.ProtoBufDeserialize<Session>(cacheValue);

            if (!System.Object.Equals(session.IP, token.IP))
            {
                session.IP = token.IP; this.Cache.Set(session.Key, session.ToBytes());
            }

            return session;
        }

        public override bool CheckCompetence(ISession session, string competence)
        {
            return !(null != session.Competences && !session.Competences.Contains(competence));
        }

        #endregion

        [ProtoBufCommand("H2", ResultDataType = CommandAttribute.DataType.ProtoBuf)]
        public virtual IResult Test3(object token, object arguments = null, Session session = null, Parameters.Register ags = default(Parameters.Register))
        {
            //data
            /*
            using (var con = this.DB.GetConnection())
            {
                con.BeginTransaction();

                var songs = con.Entity.songs.Where(c => c.songs_name == "eeee");
                foreach (var item in songs)
                {
                    System.Console.WriteLine(string.Format("{0}|{1}", item.songs_name, item.songs_passwd));
                }

                con.Commit();
            }
            */
            //cache
            this.Cache.Set("222", "333");
            this.Cache.Set("2qw", "333");
            this.Cache.Set("3qw", "333");

            this.Cache.Remove("222");
            this.Cache.Remove("2qw");
            this.Cache.Remove("3qw");

            this.Cache.Remove(session.Key);
            this.Cache.Remove(session.Account);

            //log
            this.Log.Debug(ags.account);

            //result
            var resultData = new List<Result> { new Result { account = "aaa", email = "bbb" }, new Result { account = "ccc", email = "ddd" } };

            var result1 = ResultFactory.Create(resultData);

            return result1;
        }
    }

    [TestClass]
    public class TestResult
    {
        static string GetToken()
        {
            var session = new Session<List<string>> { Account = "admin", Password = "test", IP = "192.168.1.111", Site = "Site1" }.ToString();
            var error = System.String.Empty;
            var key = Common.Interceptor.Instance.Login(session, out error);
            if (!System.String.IsNullOrEmpty(error)) { throw new System.Exception(error); }
            var token = new Business.Authentication.Token { Key = key, IP = "192.168" }.ToString();
            return token;
        }

        static string GetToken1()
        {
            var session = new Session { Account = "admin", Password = "test", IP = "192.168.1.111", Site = "Site1" }.ToString();
            var error = System.String.Empty;
            var key = Common.Interceptor1.Instance.Login(session, out error);
            if (!System.String.IsNullOrEmpty(error)) { throw new System.Exception(error); }
            var token = new Business.Authentication.Token { Key = key, IP = "192.168" }.ToString();
            return token;
        }

        static Parameters.Register Parameter()
        {
            return new Parameters.Register() { account = "hello    ", password = " 1234 69", d1 = new List<string>() { "aaa", "bbb" }, d2 = new int[] { 1, 2, 3, 4, 5 } };
        }

        static void Asserts(string json, byte[] bytes)
        {
            var ddd1 = (ResultProtoBuf<List<System.Tuple<string, string>>>)Help.ProtoBufDeserialize(bytes, typeof(ResultProtoBuf<List<System.Tuple<string, string>>>));
            Assert.IsNotNull(ddd1);

            if (null != bytes)
            {
                ResultProtoBuf<List<System.Tuple<string, string>>> ddd5 = bytes;
                Assert.IsNotNull(ddd5);

                var rrr2 = (ResultProtoBuf<List<Result>>)bytes;
                Assert.IsNotNull(rrr2);
            }

            ResultBase<List<Result>> rrr1 = json;
            Assert.IsNotNull(rrr1);
        }

        static void Asserts1(string json, byte[] bytes)
        {
            var ddd1 = (ResultBase<List<System.Tuple<string, string>>>)Help.ProtoBufDeserialize(bytes, typeof(ResultBase<List<System.Tuple<string, string>>>));
            Assert.IsNotNull(ddd1);

            if (null != bytes)
            {
                ResultBase<List<System.Tuple<string, string>>> ddd5 = bytes;
                Assert.IsNotNull(ddd5);

                var rrr2 = (ResultBase<List<Result>>)bytes;
                Assert.IsNotNull(rrr2);
            }

            ResultBase<List<Result>> rrr1 = json;
            Assert.IsNotNull(rrr1);
        }

        [TestMethod]
        public void TestResult1()
        {
            var token = GetToken();

            var ps = Parameter();

            var startTime = new System.Diagnostics.Stopwatch();
            startTime.Start();

            var rrr = Common.Interceptor.Instance.Test3(token, ps.ToString());

            var json = rrr.ToString();

            var bytes = rrr.ToBytes();

            startTime.Stop();

            var time = startTime.Elapsed.TotalMilliseconds;
            System.Console.WriteLine(time);

            Asserts(json, bytes);
        }

        [TestMethod]
        public void TestResult2()
        {
            var token = GetToken();

            var ps = Parameter();

            var startTime = new System.Diagnostics.Stopwatch();
            startTime.Start();

            var rrr = Common.Interceptor.Instance.Test3(token, ps.ToBytes());

            var json = rrr.ToString();

            var bytes = rrr.ToBytes();

            startTime.Stop();

            var time = startTime.Elapsed.TotalMilliseconds;
            System.Console.WriteLine(time);

            Asserts(json, bytes);
        }

        [TestMethod]
        public void TestResult3()
        {
            var token = GetToken();

            var ps = Parameter();

            var startTime = new System.Diagnostics.Stopwatch();
            startTime.Start();

            var rrr = Common.Interceptor.Command["H2"].Method(token, ps.ToBytes());

            var json = rrr.ToString();

            var bytes = rrr.ToBytes();

            startTime.Stop();

            var time = startTime.Elapsed.TotalMilliseconds;
            System.Console.WriteLine(time);

            Asserts(json, bytes);
        }

        [TestMethod]
        public void TestResult4()
        {
            var token = GetToken();

            var ps = Parameter();

            var startTime = new System.Diagnostics.Stopwatch();
            startTime.Start();

            var rrr = Common.Interceptor.Command["H2"].Method(token, ps.ToString());

            var json = rrr.ToString();

            var bytes = rrr.ToBytes();

            startTime.Stop();

            var time = startTime.Elapsed.TotalMilliseconds;
            System.Console.WriteLine(time);

            Asserts(json, bytes);
        }

        [TestMethod]
        public void TestResult5()
        {
            var token = GetToken1();

            var ps = Parameter();

            var startTime = new System.Diagnostics.Stopwatch();
            startTime.Start();

            var rrr = Common.Interceptor1.Instance.Test3(token, ps.ToString());

            var json = rrr.ToString();

            var bytes = rrr.ToBytes();

            startTime.Stop();

            var time = startTime.Elapsed.TotalMilliseconds;
            System.Console.WriteLine(time);

            Asserts1(json, bytes);
        }

        [TestMethod]
        public void TestResult6()
        {
            var token = GetToken1();

            var ps = Parameter();

            var startTime = new System.Diagnostics.Stopwatch();
            startTime.Start();

            var rrr = Common.Interceptor1.Instance.Test3(token, ps.ToBytes());

            var json = rrr.ToString();

            var bytes = rrr.ToBytes();

            startTime.Stop();

            var time = startTime.Elapsed.TotalMilliseconds;
            System.Console.WriteLine(time);

            Asserts1(json, bytes);
        }

        [TestMethod]
        public void TestResult7()
        {
            var token = GetToken1();

            var ps = Parameter();

            var startTime = new System.Diagnostics.Stopwatch();
            startTime.Start();

            var rrr = Common.Interceptor1.Command["H2"].Method(token, ps.ToBytes());

            var json = rrr.ToString();

            var bytes = rrr.ToBytes();

            startTime.Stop();

            var time = startTime.Elapsed.TotalMilliseconds;
            System.Console.WriteLine(time);

            Asserts1(json, bytes);
        }

        [TestMethod]
        public void TestResult8()
        {
            var token = GetToken1();

            var ps = Parameter();

            var startTime = new System.Diagnostics.Stopwatch();
            startTime.Start();

            var rrr = Common.Interceptor1.Command["H2"].Method(token, ps.ToString());

            var json = rrr.ToString();

            var bytes = rrr.ToBytes();

            startTime.Stop();

            var time = startTime.Elapsed.TotalMilliseconds;
            System.Console.WriteLine(time);

            Asserts1(json, bytes);
        }
    }
}
