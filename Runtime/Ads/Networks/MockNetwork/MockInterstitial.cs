using GameKit.Ads.Networks.MockBehaviour;
using GameKit.Ads.Units;

namespace GameKit.Ads.Networks
{
    internal class MockInterstitial : MockFullscreen, IInterstitialAdUnit
    {
        public MockInterstitial(MockCanvas canvas) : base(canvas) { }
        
        public override void Show()
        {
            base.Show();
            Canvas.ShowInterstitial();
        }
    }
}