using GameKit.Ads.Networks.MockBehaviour;
using GameKit.Ads.Units;

namespace GameKit.Ads.Networks
{
    internal class MockFullscreen : MockAdUnit
    {
        public MockFullscreen(MockCanvas canvas) : base(canvas) { }

        public override void Show()
        {
            Canvas.EventSuccess += OnSuccess;
            Canvas.EventFailed += OnFailed;
            Canvas.EventCanceled += OnCanceled;
            Canvas.EventFullscreenClicked += OnClicked;
            State = AdUnitState.Displayed;
        }
        
        private void Close()
        {
            Canvas.EventSuccess -= OnSuccess;
            Canvas.EventFailed -= OnFailed;
            Canvas.EventCanceled -= OnCanceled;
            Canvas.EventFullscreenClicked -= OnClicked;
            Canvas.HideFullscreen();
            Release();
        }

        protected virtual void OnClicked()
        {
            State = AdUnitState.Clicked;
            Close();
        }

        protected virtual  void OnFailed()
        {
            Error = "Send failed";
            State = AdUnitState.Error;
            Close();
        }

        protected virtual  void OnSuccess()
        {
            State = AdUnitState.Closed;
            Close();
        }
        
        protected virtual  void OnCanceled()
        {
            State = AdUnitState.Closed;
            Close();
        }

    }
}