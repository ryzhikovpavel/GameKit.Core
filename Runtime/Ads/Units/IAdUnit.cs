namespace GameKit.Ads.Units
{
    public interface IAdUnit
    {
        bool IsLoaded => State == AdUnitState.Loaded;
        AdUnitState State { get; }
        string Error { get; }
        IAdInfo Info { get; }
        
        void Show();
        void Release();
    }
}