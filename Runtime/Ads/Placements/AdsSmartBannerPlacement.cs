using System;
using System.Collections;
using GameKit.Ads.Units;
using JetBrains.Annotations;
using UnityEngine;

namespace GameKit.Ads.Placements
{
    [PublicAPI]
    public class AdsSmartBannerPlacement: AdsPlacement
    {
        private const int MinUpdateFrequency = 30;
        
        private IEnumerator _repeatedLoop;
        private float _updateFrequency;
        private bool _displayed;
        public AdAnchor Anchor { get; private set; }
        public override Type UnitType { get; }
        
        public AdsSmartBannerPlacement(AdAnchor anchor, float updateFrequency) : this(null, anchor, updateFrequency) { }

        public AdsSmartBannerPlacement(string name, AdAnchor anchor, float updateFrequency) : base(name)
        {
            UnitType = (anchor == AdAnchor.Top ? typeof(ITopSmartBannerAdUnit) : typeof(IBottomSmartBannerAdUnit));
            _updateFrequency = Mathf.Max(MinUpdateFrequency, updateFrequency);

            EventClosed += OnClosed;
            EventDisplayed += OnDisplayed;
        }

        public void Show()
        {
            if (_repeatedLoop != null) return;
            Loop.StartCoroutine(_repeatedLoop = RepeatedLoop());
        }

        public void Hide()
        {
            if (_displayed) Service<AdsMediator>.Instance.Hide(this);

            if (_repeatedLoop != null)
            {
                if (Logger<AdsMediator>.IsDebugAllowed) 
                    Logger<AdsMediator>.Debug($"{DebugName}|Stop repeating process");
                Loop.StopCoroutine(_repeatedLoop);
                _repeatedLoop = null;
            }
        }

        private void OnClosed(AdsPlacement placement)
        {
            _displayed = false;
        }

        private void OnDisplayed(AdsPlacement placement, IAdInfo info)
        {
            _displayed = true;
        }

        private IEnumerator RepeatedLoop()
        {
            if (Logger<AdsMediator>.IsDebugAllowed) 
                Logger<AdsMediator>.Debug($"{DebugName}|Start repeating process");
            
            while (Application.isPlaying)
            {
                if (IsFetched == false)
                {
                    yield return null;
                    continue;
                }
                
                if (Logger<AdsMediator>.IsDebugAllowed) 
                    Logger<AdsMediator>.Debug($"{DebugName}|Request show unit");
                
                Service<AdsMediator>.Instance.Show(this);
                while (_displayed == false)
                {
                    yield return null;
                }

                var timeRemaining = _updateFrequency;

                if (Logger<AdsMediator>.IsDebugAllowed) 
                    Logger<AdsMediator>.Debug($"{DebugName}|Wait {timeRemaining} seconds for update ad unit");
            
                while (timeRemaining > 0 && _displayed)
                {
                    yield return null;
                    timeRemaining -= Time.unscaledDeltaTime;
                }
                
                if (Logger<AdsMediator>.IsDebugAllowed) 
                    Logger<AdsMediator>.Debug($"{DebugName}|Request hide unit");
                
                if (_displayed) Service<AdsMediator>.Instance.Hide(this);
                yield return null;
            }
        }
    }
}