using System;

namespace GameKit.Ads
{
    [Serializable]
    public struct AdsSession
    {
        public bool disableIntrusiveAdUnits;
        public int relevantAdsConsent;
    }
}