namespace GameKit.Ads
{
    public interface IRewardAdInfo
    {
        string Type { get; }
        double Reward { get; }
    }

    public struct DefaultRewardAdInfo : IRewardAdInfo
    {
        public string Type { get; }
        public double Reward { get; }

        public DefaultRewardAdInfo(double reward, string type)
        {
            Reward = reward;
            Type = type;
        }
    }
}