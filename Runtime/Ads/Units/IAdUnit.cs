namespace GameKit.Ads.Units
{
    public interface IAdUnit
    {
        AdUnitState State { get; }
        string Error { get; }
        IAdInfo Info { get; }
        
        void Show();
        void Release();
    }
}