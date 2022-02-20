using System;
using GameKit.Ads.Units;

namespace GameKit.Ads.Networks
{
    public interface IAdsNetwork
    {
        bool IsInitialized { get; }
        bool IsValid { get; }
        void Initialize(bool trackingConsent, bool purchasedDisableUnits);
        void DisableUnits();
        bool IsSupported(Type type);
        IAdUnit[] GetUnits(Type type);
    }
}