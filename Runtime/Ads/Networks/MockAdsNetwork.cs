using System;
using System.Collections;
using System.Collections.Generic;
using GameKit.Ads.Networks.MockBehaviour;
using GameKit.Ads.Units;
using UnityEngine;

namespace GameKit.Ads.Networks
{
    internal class MockAdsNetwork: IAdsNetwork
    {
        private readonly Dictionary<Type, IAdUnit[]> _units = new Dictionary<Type,  IAdUnit[]>();

        public bool IsInitialized { get; private set; }
        
        public void Initialize(bool trackingConsent, bool purchasedDisableUnits)
        {
            IsInitialized = true;
            MockCanvas canvas = UnityEngine.Object.Instantiate(Resources.Load<MockCanvas>("MockAdsNetworkCanvas"));
            _units.Add(typeof(IAnchoredBannerAdUnit), new IAdUnit[] {new MockAnchoredAdUnit(canvas)});
            _units.Add(typeof(IRewardedVideoAdUnit), new IAdUnit[] {new MockRewardedVideo(canvas)});
            _units.Add(typeof(IInterstitialAdUnit), new IAdUnit[] {new MockInterstitial(canvas)});
            
            canvas.HideFullscreen();
            canvas.HideAnchored();
        }

        public void DisableUnits()
        {
            
        }

        public bool IsSupported(Type type) => _units.ContainsKey(type);
        public IAdUnit[] GetUnits(Type type) => _units[type];
    }

    internal abstract class MockAdUnit : IAdUnit
    {
        protected MockCanvas Canvas;

        public AdUnitState State { get; protected set; }
        public string Error { get; protected set;}
        public IAdInfo Info { get; } = new DefaultAdInfo();

        public abstract void Show();
        
        public void Release()
        {
            Loop.StartCoroutine(Load());
        }

        public MockAdUnit(MockCanvas canvas)
        {
            Canvas = canvas;
            State = AdUnitState.Loaded;
        }

        private IEnumerator Load()
        {
            yield return new WaitForSecondsRealtime(1f);
            State = AdUnitState.Loaded;
        }
    }
    
    internal class MockAnchoredAdUnit : MockAdUnit, IAnchoredBannerAdUnit
    {
        private AdAnchor _anchor;
        public void SetAnchor(AdAnchor anchor)
        {
            _anchor = anchor;
        }

        public void Hide()
        {
            Canvas.HideAnchored();
        }

        public override void Show()
        {
            Canvas.ShowAnchored(_anchor);
            State = AdUnitState.Displayed;
        }

        public MockAnchoredAdUnit(MockCanvas canvas) : base(canvas)
        {
            Canvas.EventAnchoredClicked += OnEventAnchoredClicked;
        }

        private void OnEventAnchoredClicked()
        {
            State = AdUnitState.Clicked;
            Hide();
        }
    }
    
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

    internal class MockInterstitial : MockFullscreen, IInterstitialAdUnit
    {
        public MockInterstitial(MockCanvas canvas) : base(canvas) { }
        
        public override void Show()
        {
            base.Show();
            Canvas.ShowInterstitial();
        }
    }

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
        public IRewardAdInfo Reward { get; } = new DefaultRewardAdInfo();
    }
}