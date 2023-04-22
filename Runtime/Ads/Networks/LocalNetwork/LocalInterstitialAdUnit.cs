using GameKit.Ads.Units;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameKit.Ads.Networks.LocalNetwork
{
    public class LocalInterstitialAdUnit: LocalAdUnit, IInterstitialAdUnit
    {
        [SerializeField] private Button buttonClose;
        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            buttonClose.onClick.AddListener(()=>State = AdUnitState.Closed);
        }
    }
}