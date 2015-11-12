namespace BusinessLib.Cache
{
    using System.Linq;
    using BusinessLib.Extensions;

    public class CacheBase : ICache
    {
        public RemovedCallback<string, object> RemovedHandler { get; set; }

        public void Set<Type>(string key, Type value, System.TimeSpan outTime)
        {
            var policy = new System.Runtime.Caching.CacheItemPolicy() { SlidingExpiration = outTime };
            if (null != RemovedHandler) { policy.RemovedCallback += (obj) => { RemovedHandler(obj.CacheItem.Key, obj.CacheItem.Value); }; }

            System.Runtime.Caching.MemoryCache.Default.Set(key, value, policy);
        }

        public void Set(string key, object value, System.TimeSpan outTime)
        {
            Set<object>(key, value, outTime);
        }

        #region IDictionary

        public void Set<Type>(string key, Type value)
        {
            System.Runtime.Caching.MemoryCache.Default.Set(key, value, System.DateTimeOffset.MaxValue);
        }

        public object Get(string key)
        {
            return System.Runtime.Caching.MemoryCache.Default.Get(key);
        }

        public Type Get<Type>(string key)
        {
            return Get(key).ChangeType<Type>();
        }

        public void Remove(string key)
        {
            System.Runtime.Caching.MemoryCache.Default.Remove(key);
        }

        public bool Contains(string key)
        {
            return System.Runtime.Caching.MemoryCache.Default.Contains(key);
        }

        public long Count
        {
            get { return System.Runtime.Caching.MemoryCache.Default.GetCount(); }
        }

        public void Clear()
        {
            foreach (var item in System.Runtime.Caching.MemoryCache.Default.Select(s => s.Key))
            {
                System.Runtime.Caching.MemoryCache.Default.Remove(item);
            }
        }

        public System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<string, object>> GetEnumerator()
        {
            foreach (var item in System.Runtime.Caching.MemoryCache.Default)
            {
                yield return item;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
