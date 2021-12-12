namespace GameKit.Ads.Requests
{
    public readonly struct AdBannerRequestData: IAdAnchorableRequestData, IAdUpdateFrequencyRequestData
    {
        public AdAnchor Anchor { get; }
        public float UpdateFrequency { get; }

        public AdBannerRequestData(AdAnchor anchor, float updateFrequency)
        {
            Anchor = anchor;
            UpdateFrequency = updateFrequency;
        }
        
    }
}