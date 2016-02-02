using System;
using StackExchange.Redis;
using Business;
using Business.Extensions;
using Business.Auth;
using Business.Result;
using Business.Attributes;
using System.Collections.Generic;
using ProtoBuf;
using NLog;
using LinqToDB.Mapping;
using LinqToDB;
using LinqToDB.DataProvider;
using System.Linq;
using Newtonsoft.Json;

namespace TestLib
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

        public IQueryable<songs> songs { get { return Get<songs>(); } }

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

        public static Business.Extensions.InterceptorBind<BusinessMember> Interceptor = new InterceptorBind<BusinessMember>(new Interceptor());

        public static Business.Extensions.InterceptorBind<BusinessMember> InterceptorNot = new InterceptorBind<BusinessMember>(new InterceptorNot());
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
                if (System.String.IsNullOrEmpty(value)) { error = Mark.Get<string>(Mark.MarkItem.Exp_UserError); return System.String.Empty; }

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

            Token _token = System.Convert.ToString(token);

            if (null == _token) { return null; }

            return _token;
        }

        public override Session GetSession<Session>(Business.Auth.IToken token)
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
        public virtual IResult Test3(string token, object arguments = null, Session session = null, Parameters.Register ags = default(Parameters.Register))
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
}
