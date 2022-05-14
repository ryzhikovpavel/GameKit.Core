using System;
using System.Collections.Generic;
using GameKit.Ads.Networks.MockBehaviour;
using GameKit.Ads.Units;
using UnityEngine;

namespace GameKit.Ads.Networks
{
    internal class MockAdsNetwork: IAdsNetwork
    {
        private readonly Dictionary<Type, IAdUnit[]> _units = new Dictionary<Type,  IAdUnit[]>();

        public TaskRoutine Initialize(bool trackingConsent, bool intrusiveAdUnits)
        {
            MockCanvas canvas = UnityEngine.Object.Instantiate(Resources.Load<MockCanvas>("MockAdsNetworkCanvas"));

            if (intrusiveAdUnits)
            {
                _units.Add(typeof(ITopSmartBannerAdUnit),
                    new IAdUnit[] { new MockAnchoredAdUnit(canvas, AdAnchor.Top) });
                _units.Add(typeof(IBottomSmartBannerAdUnit),
                    new IAdUnit[] { new MockAnchoredAdUnit(canvas, AdAnchor.Bottom) });
                _units.Add(typeof(IInterstitialAdUnit), new IAdUnit[] { new MockInterstitial(canvas) });
            }
            
            _units.Add(typeof(IRewardedVideoAdUnit), new IAdUnit[] { new MockRewardedVideo(canvas) });

            canvas.HideFullscreen();
            canvas.HideAnchored();
            
            return TaskRoutine.FromCompleted();
        }
        
        public bool IsSupported(Type type) => _units.ContainsKey(type);
        public IAdUnit[] GetUnits(Type type) => _units[type];
    }
}