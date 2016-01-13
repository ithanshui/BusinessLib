namespace Business.Cache
{
    public interface ICache<K>
    {
        void Set(K key, CacheValue value, System.TimeSpan? outTime = null);

        //void SetGroup(K group, K key, CacheValue value);

        CacheValue Get(K key);

        //CacheValue[] Gets(params K[] keys);

        //CacheValue GetGroup(K group, K key);

        //CacheValue[] GetGroups(K group, params K[] keys);

        void Remove(K key);

        //void Removes(params K[] key);

        //void RemoveGroup(K group, params K[] keys);

        bool ContainsKey(K key);

        //bool ContainsKey(K group, K key);
    }

    public interface ICache : ICache<string> { }
}
