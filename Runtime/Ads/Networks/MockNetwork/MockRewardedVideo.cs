using GameKit.Ads.Networks.MockBehaviour;
using GameKit.Ads.Units;

namespace GameKit.Ads.Networks
{
    internal class MockRewardedVideo : MockFullscreen, IRewardedVideoAdUnit
    {
        public MockRewardedVideo(MockCanvas canvas) : base(canvas) { }

        public override void Show()
        {
            base.Show();
            IsEarned = false;
            Canvas.ShowRewardedVideo();
        }

        protected override void OnSuccess()
        {
            IsEarned = true;
            base.OnSuccess();
        }

        public bool IsEarned { get; private set; }
        public IRewardAdInfo Reward { get; set; } = new DefaultRewardAdInfo(0, "");
    }
}