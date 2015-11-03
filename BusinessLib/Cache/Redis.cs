using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLib.Cache
{
    public class Redis : ICache, System.IDisposable
    {
        readonly ServiceStack.Redis.RedisClient redis;

        public Redis()
        {
            redis = new ServiceStack.Redis.RedisClient(Host, Port);
        }

        public virtual string Host { get { return "localhost"; } }

        public virtual int Port { get { return 6379; } }

        public void Set(string key, object value, TimeSpan outTime)
        {
            redis.Set(key, value, outTime);
        }

        public long Count
        {
            get { return redis.GetAllKeys().Count; }
        }

        public void Clear()
        {
            redis.RemoveAll(redis.GetAllKeys());
        }

        public void Set(string key, object value)
        {
            redis.Set(key, value);
        }

        public object Get(string key)
        {
            return Get<object>(key);
        }

        public Type Get<Type>(string key)
        {
            Type value;

            var time = redis.GetTimeToLive(key);

            if (time.HasValue)
            {
                value = redis.Get<Type>(key);
                Set(key, value, time.Value);
            }
            else
            {
                value = redis.Get<Type>(key);
            }

            return value;
        }

        public void Remove(string key)
        {
            redis.Remove(key);
        }

        public bool Contains(string key)
        {
            return redis.ContainsKey(key);
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            var keys = redis.GetAllKeys();

            foreach (var key in keys)
            {
                yield return new KeyValuePair<string, object>(key, Get(key));
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Dispose()
        {
            if (null != redis)
            {
                redis.Dispose();
            }
        }
    }
}
