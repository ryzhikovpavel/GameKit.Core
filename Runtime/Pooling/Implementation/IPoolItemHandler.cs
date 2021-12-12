namespace GameKit.Implementation
{
    public interface IPoolItemHandler<T>
    {
        T CreateInstance();
        void Reset(T item);
    }
}