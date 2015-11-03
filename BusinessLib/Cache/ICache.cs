namespace BusinessLib.Cache
{
    public interface ICache : Extensions.IDictionary<string, object, System.Collections.Generic.KeyValuePair<string, object>>
    {
        void Set<Type>(string key, Type value, System.TimeSpan outTime);

        void Set(string key, object value, System.TimeSpan outTime);
    }
}
