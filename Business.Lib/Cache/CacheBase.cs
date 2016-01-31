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

namespace Business.Cache
{
    public class CacheBase : ICache, System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, CacheValue>>, System.Collections.IEnumerable
    {
        public void Set(string key, CacheValue value, System.TimeSpan? outTime = null)
        {
            if (!value.HasValue) { return; }

            if (outTime.HasValue)
            {
                System.Runtime.Caching.MemoryCache.Default.Set(key, value.Value, new System.Runtime.Caching.CacheItemPolicy() { SlidingExpiration = outTime.Value });
            }
            else
            {
                System.Runtime.Caching.MemoryCache.Default.Set(key, value.Value, System.DateTimeOffset.MaxValue);
            }
        }

        public CacheValue Get(string key)
        {
            var value = System.Runtime.Caching.MemoryCache.Default.Get(key);
            return null == value ? default(CacheValue) : new CacheValue(value as byte[]);
        }

        public void Remove(string key)
        {
            System.Runtime.Caching.MemoryCache.Default.Remove(key);
        }

        public bool ContainsKey(string key)
        {
            return System.Runtime.Caching.MemoryCache.Default.Contains(key);
        }

        public long Count()
        {
            return System.Runtime.Caching.MemoryCache.Default.GetCount();
        }

        public void Clear()
        {
            foreach (var item in System.Runtime.Caching.MemoryCache.Default.Select(s => s.Key))
            {
                System.Runtime.Caching.MemoryCache.Default.Remove(item);
            }
        }

        public System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<string, CacheValue>> GetEnumerator()
        {
            foreach (var item in System.Runtime.Caching.MemoryCache.Default)
            {
                yield return new System.Collections.Generic.KeyValuePair<string, CacheValue>(item.Key, Extensions.Help.ChangeType<CacheValue>(item.Value));
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
