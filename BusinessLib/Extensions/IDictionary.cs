namespace BusinessLib.Extensions
{
    public delegate void RemovedCallback<T, T1>(T key, T1 value);

    public interface IDictionary<T, T1, T2> : System.Collections.Generic.IEnumerable<T2>, System.Collections.IEnumerable
    {
        RemovedCallback<T, T1> RemovedHandler { get; set; }

        long Count { get; }

        void Clear();

        void Set<Type>(T key, Type value);

        object Get(T key);

        Type Get<Type>(T key);

        void Remove(T key);

        bool Contains(T key);
    }
}
