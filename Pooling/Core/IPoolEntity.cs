using GameKit.Pooling.Core;

namespace GameKit.Pooling.Core
{
    public interface IPoolEntity
    {
        int Id { get; set; }
        IPoolContainer Owner { get; set; }
        void Reset();
    }
}