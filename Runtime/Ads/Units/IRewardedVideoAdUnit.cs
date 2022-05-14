namespace GameKit.Ads.Units
{
    public interface IRewardedVideoAdUnit: IAdUnit
    {
        bool IsEarned { get; }
        IRewardAdInfo Reward { get; set; }
    }
}