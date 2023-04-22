using GameKit.Ads.Units;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameKit.Ads.Networks.LocalNetwork
{
    public class LocalBannerAdUnit: LocalAdUnit, ITopSmartBannerAdUnit, IBottomSmartBannerAdUnit
    {
        public void Hide()
        {
            State = AdUnitState.Closed;
            gameObject.SetActive(true);
        }
    }
}