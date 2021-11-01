namespace GameKit.Pooling.Core
{
    public interface IPoolItemHandler<T>
    {
        T CreateInstance();
        void Reset(T item);
    }
}