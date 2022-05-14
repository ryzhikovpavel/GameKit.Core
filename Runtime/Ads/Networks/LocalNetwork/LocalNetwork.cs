using System;
using GameKit.Ads.Units;

namespace GameKit.Ads.Networks.LocalNetwork
{
    public class LocalNetwork: IAdsNetwork
    {
        public TaskRoutine Initialize(bool trackingConsent, bool intrusiveAdUnits)
        {
            return TaskRoutine.FromCompleted();
        }

        public bool IsSupported(Type type)
        {
            return false;
        }

        public IAdUnit[] GetUnits(Type type)
        {
            return null;
        }
    }
}