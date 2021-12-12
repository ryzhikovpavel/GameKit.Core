using System.Collections.Generic;

namespace GameKit.Implementation
{
    public interface IPoolContainer
    {
        void Push(IPoolEntity entity);
        void ReleaseAll();
    }
    
    public interface IPool<T>: IPoolContainer, IEnumerable<T>
    {
        T Pull();
        void PushInstance(T instance);
        void Initialize(int capacity);
    }
}