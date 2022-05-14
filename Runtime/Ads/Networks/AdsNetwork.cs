using System;
using System.Threading.Tasks;
using GameKit.Ads.Units;

namespace GameKit.Ads.Networks
{
    public interface IAdsNetwork
    {
        TaskRoutine Initialize(bool trackingConsent, bool intrusiveAdUnits);
        bool IsSupported(Type type);
        IAdUnit[] GetUnits(Type type);
    }
}