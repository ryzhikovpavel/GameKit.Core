namespace GameKit.Ads.Units
{
    public interface IRewardedVideoAdUnit: IAdUnit
    {
        bool IsEarned { get; }
        IRewardAdInfo Reward { get; set; }
    }

    public interface IAnchoredBannerAdUnit : IAdUnit 
    {
        void SetAnchor(AdAnchor anchor);
        void Hide();
    }

    public interface IInterstitialAdUnit : IAdUnit { }
}