namespace GameKit.Ads
{
    public interface IRewardAdInfo
    {
        int Reward { get; }
    }

    public struct DefaultRewardAdInfo : IRewardAdInfo
    {
        public int Reward { get; }

        public DefaultRewardAdInfo(int reward)
        {
            Reward = reward;
        }
    }
}