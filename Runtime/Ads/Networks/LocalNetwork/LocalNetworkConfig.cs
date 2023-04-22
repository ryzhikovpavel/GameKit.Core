using System;
using UnityEngine;

namespace GameKit.Ads.Networks.LocalNetwork
{
    [CreateAssetMenu(fileName = "LocalNetwork", menuName = "GameKit/Ads/LocalNetwork", order = 0)]
    public class LocalNetworkConfig : ScriptableObject
    {
        public LocalAdUnitData[] interstitials;
        public LocalAdUnitData[] banners;
    }
    
    [Serializable]
    public class LocalAdUnitData
    {
        public Texture texture;
        public string url;
    }
}