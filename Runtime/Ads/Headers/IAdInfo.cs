namespace GameKit.Ads
{
    public interface IAdInfo
    {
        int Floor { get; }
    }

    public struct DefaultAdInfo : IAdInfo
    {
        public int Floor => 0;
    }
}