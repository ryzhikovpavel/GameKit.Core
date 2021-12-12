namespace GameKit.Implementation
{
    public interface IPoolEntity
    {
        int Id { get; set; }
        IPoolContainer Owner { get; set; }
        void Reset();
    }
}