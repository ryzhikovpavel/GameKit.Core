using GameKit.Pooling.Core;

namespace GameKit.Pooling.Core
{
    public interface IPoolEntity
    {
        int Id { get; internal set; }
        IPoolContainer Owner { get; internal set; }
        void Reset();
    }
}