namespace GameKit.Ads.Requests
{
    public interface IAdRequestData { }

    public interface IAdAnchorableRequestData: IAdRequestData
    {
        AdAnchor Anchor { get; }
    }

    public interface IAdUpdateFrequencyRequestData : IAdRequestData
    {
        float UpdateFrequency { get; }
    }
}