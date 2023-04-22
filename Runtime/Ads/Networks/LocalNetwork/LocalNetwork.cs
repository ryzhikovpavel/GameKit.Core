using System;
using System.Collections;
using System.Collections.Generic;
using GameKit.Ads.Units;
using UnityEngine;

namespace GameKit.Ads.Networks.LocalNetwork
{
    public class LocalNetwork: IAdsNetwork
    {
        private readonly Dictionary<Type, IAdUnit[]> _units = new Dictionary<Type,  IAdUnit[]>();
        private LocalAdUnitData[] _interstitialData;
        private LocalAdUnitData[] _bannerData;

        public TaskRoutine Initialize(bool trackingConsent, bool intrusiveAdUnits)
        {
            string screen = (Screen.height > Screen.width) ? "Portrait" : "Landscape";
            
            LocalInterstitialAdUnit interstitial = UnityEngine.Object.Instantiate(
                Resources.Load<LocalInterstitialAdUnit>($"LocalInterstitialAdUnit-{screen}"));
            interstitial.SetActive(false);

            LocalBannerAdUnit topBanner = UnityEngine.Object.Instantiate(
                Resources.Load<LocalBannerAdUnit>($"LocalInterstitialAdUnit-{screen}-Top"));
            topBanner.SetActive(false);
            
            LocalBannerAdUnit bottomBanner = UnityEngine.Object.Instantiate(
                Resources.Load<LocalBannerAdUnit>($"LocalInterstitialAdUnit-{screen}-Bottom"));
            bottomBanner.SetActive(false);

            if (intrusiveAdUnits)
            {
                _units.Add(typeof(ITopSmartBannerAdUnit),
                    new IAdUnit[] { topBanner });
                _units.Add(typeof(IBottomSmartBannerAdUnit),
                    new IAdUnit[] { bottomBanner });
                _units.Add(typeof(IInterstitialAdUnit), new IAdUnit[] { interstitial });
                
                Loop.StartCoroutine(WaitAndSetupAdUnit(_interstitialData, interstitial));
                Loop.StartCoroutine(WaitAndSetupAdUnit(_bannerData, topBanner));
                Loop.StartCoroutine(WaitAndSetupAdUnit(_bannerData, bottomBanner));
            }

            return TaskRoutine.FromCompleted();
        }
        
        public bool IsSupported(Type type) => _units.ContainsKey(type);
        public IAdUnit[] GetUnits(Type type) => _units[type];

        private LocalNetwork()
        {
            
        }
        
        public static void Initialize(LocalNetworkConfig config)
        {
            var network = new LocalNetwork();
            network._interstitialData = config.interstitials;
            network._bannerData = config.banners;
            Service<AdsMediator>.Instance.RegisterNetwork(network);
        }

        private IEnumerator WaitAndSetupAdUnit(LocalAdUnitData[] ads, LocalAdUnit unit)
        {
            int index = 0;
            while (Application.isPlaying)
            {
                if (unit.State != AdUnitState.Empty)
                {
                    yield return null;
                    continue;
                }

                if (ads is null || ads.Length == 0)
                {
                    yield return null;
                    continue;
                }

                if (index >= ads.Length) index = 0;
                LocalAdUnitData ad = ads[index++];
                unit.Setup(ad.texture, ad.url);
            }
        }
    }
}