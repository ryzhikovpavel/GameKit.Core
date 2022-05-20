using GameKit.Ads.Networks.MockBehaviour;
using GameKit.Ads.Units;

namespace GameKit.Ads.Networks
{
    internal class MockAnchoredAdUnit : MockAdUnit,ITopSmartBannerAdUnit, IBottomSmartBannerAdUnit
    {
        private AdAnchor _anchor;

        public void Hide()
        {
            Canvas.HideAnchored();
            State = AdUnitState.Closed;
        }

        public override void Show()
        {
            Canvas.ShowAnchored(_anchor);
            State = AdUnitState.Displayed;
        }
        
        public MockAnchoredAdUnit(MockCanvas canvas, AdAnchor anchor) : base(canvas)
        {
            Canvas.EventAnchoredClicked += OnEventAnchoredClicked;
            _anchor = anchor;
        }

        private void OnEventAnchoredClicked()
        {
            Hide();
            State = AdUnitState.Clicked;
        }
    }
}