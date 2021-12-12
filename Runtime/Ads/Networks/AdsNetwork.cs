using System;
using GameKit.Ads.Units;
using UnityEngine;

namespace GameKit.Ads.Networks
{
    public abstract class AdsNetwork : UnityEngine.ScriptableObject, IAdsNetwork
    {
        public bool IsInitialized { get; protected set; }
        public abstract void Initialize(bool trackingConsent, bool purchasedDisableUnits);

        public abstract void DisableUnits();
        public abstract bool IsSupported(Type type);
        public abstract IAdUnit[] GetUnits(Type type);
    }

    public interface IAdsNetwork
    {
        bool IsInitialized { get; }
        void Initialize(bool trackingConsent, bool purchasedDisableUnits);
        void DisableUnits();
        bool IsSupported(Type type);
        IAdUnit[] GetUnits(Type type);
    }
}