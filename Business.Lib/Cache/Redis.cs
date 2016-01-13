using StackExchange.Redis;
using System.Linq;

namespace Business.Cache
{
    //using Business.Extensions;

    //private static ConnectionMultiplexer _redis;
    //private static IDatabase _db;
    //private static IServer _server;
    //private static bool needSave = false;
    //private void Init(string host, int port, string pwd, int database)
    //{
    //var options = ConfigurationOptions.Parse(host + ":" + port);
    //options.SyncTimeout = int.MaxValue;
    //options.AllowAdmin = true;
    //if (!string.IsNullOrEmpty(pwd))
    //{
    //    options.Password = pwd;
    //}
    //if (_redis == null)
    //    _redis = ConnectionMultiplexer.Connect(options);
    //if (_server == null)
    //    _server = _redis.GetServer(host + ":" + port);
    //if (_db == null)
    //    _db = _redis.GetDatabase(database);
    //needSave = false;
    //}

    public class Redis : ICache
    {
        //static string connString;
        static ConfigurationOptions cfgOpt;
        static ConnectionMultiplexer connection;

        static ConnectionMultiplexer Connection
        {
            get
            {
                lock (cfgOpt)
                {
                    if (null == connection || !connection.IsConnected)
                    {
                        connection = ConnectionMultiplexer.Connect(cfgOpt);
                    }

                    return connection;
                }
            }
        }

        readonly int db;

        public Redis(ConfigurationOptions cfgOpt, int dbNum = 0)
        {
            Redis.cfgOpt = cfgOpt;
            db = dbNum;
        }

        static CacheValue Result(RedisValue value)
        {
            return !value.HasValue ? default(CacheValue) : new CacheValue(value);
        }

        static CacheValue[] Result(RedisValue[] values)
        {
            return values.Select(c => Result(c)).ToArray();
        }

        static RedisKey[] Keys(string[] key)
        {
            return key.Select<string, RedisKey>(c => c).ToArray();
        }

        static RedisValue[] Values(string[] key)
        {
            return key.Select<string, RedisValue>(c => c).ToArray();
        }

        //public long Count(int db = 0)
        //{
        //    return Connection.GetServer(Connection.GetEndPoints()[0]).Keys(db).Count();
        //}

        public System.Collections.Generic.IEnumerable<RedisKey> Keys(int endPoint = 0, string pattern = null)
        {
            return Connection.GetServer(Connection.GetEndPoints()[endPoint]).Keys(db, pattern);
        }

        public void Set(string key, CacheValue value, System.TimeSpan? outTime = null)
        {
            if (!value.HasValue) { return; }

            var dataBase = Connection.GetDatabase(db);
            dataBase.StringSet(key, value.Value, outTime);
        }

        public void SetGroup(string group, string key, CacheValue value)
        {
            if (!value.HasValue) { return; }

            var dataBase = Connection.GetDatabase(db);
            dataBase.HashSet(group, key, value.Value);
        }

        public CacheValue Get(string key)
        {
            var dataBase = Connection.GetDatabase(db);
            var value = dataBase.StringGet(key);
            return Result(value);
        }

        public CacheValue[] Gets(params string[] keys)
        {
            var dataBase = Connection.GetDatabase(db);
            var redisKeys = Keys(keys);
            var values = dataBase.StringGet(redisKeys);
            return Result(values);
        }

        public CacheValue GetGroup(string group, string key)
        {
            var dataBase = Connection.GetDatabase(db);
            var value = dataBase.HashGet(group, key);
            return Result(value);
        }

        public CacheValue[] GetGroups(string group, params string[] keys)
        {
            var dataBase = Connection.GetDatabase(db);

            RedisValue[] values;

            if (null != keys && 0 < keys.Length)
            {
                var fields = keys.Select<string, RedisValue>(c => c).ToArray();
                values = dataBase.HashGet(group, fields);
            }
            else
            {
                values = dataBase.HashValues(group);
            }

            return Result(values);
        }

        public void Remove(string key)
        {
            Removes(key);
        }

        public void Removes(params string[] key)
        {
            var dataBase = Connection.GetDatabase(db);
            dataBase.KeyDelete(Keys(key));
        }

        public void RemoveGroup(string group, params string[] key)
        {
            var dataBase = Connection.GetDatabase(db);
            dataBase.HashDelete(group, Values(key));
        }

        public bool ContainsKey(string key)
        {
            var dataBase = Connection.GetDatabase(db);
            return dataBase.KeyExists(key);
        }

        public bool ContainsKey(string group, string key)
        {
            var dataBase = Connection.GetDatabase(db);
            return dataBase.HashExists(group, key);
        }
    }
}
