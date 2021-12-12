using System.IO;
using GameKit.Ads.Requests;
using GameKit.Ads.Units;
using JetBrains.Annotations;

namespace GameKit.Ads.Placements
{
    [PublicAPI]
    public class AdsAnchoredBannerPlacement: AdsPlacement<IAnchoredBannerAdUnit>
    {
        private float _updateFrequency;
        public AdAnchor CurrentPosition { get; private set; }

        public AdsAnchoredBannerPlacement(string name, float updateFrequency) : base(name) { }
        
        public void Show(AdAnchor anchor)
        {
            Service<AdsMediator>.Instance.Show(this, new AdBannerRequestData(anchor, _updateFrequency));
        }

        public void Hide()
        {
            Service<AdsMediator>.Instance.Hide(this);   
        }
    }
}