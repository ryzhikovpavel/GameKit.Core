using System;
using System.IO;
using GameKit.Ads.Units;
using JetBrains.Annotations;

namespace GameKit.Ads.Placements
{
    [PublicAPI]
    public class AdsRewardedVideoPlacement: AdsPlacement
    {
        private Action _earned;
        private Action _skipped;

        public event AdsRewardInfoEventHandler EventEarned = delegate { };
        public event AdsEventHandler EventSkipped = delegate { };
        public override Type UnitType { get; } = typeof(IRewardedVideoAdUnit);
        
        public AdsRewardedVideoPlacement() : this(null) {}

        public AdsRewardedVideoPlacement(string name) : base(name)
        {
            
        }

        public void Show() => Show(null, null);

        public void Show(Action earned, Action skipped)
        {
            EventFailed += OnEventFailed;
            _earned = earned;
            _skipped = skipped;
            if (IsFetched == false) throw new Exception("Ad units not loaded");
            Service<AdsMediator>.Instance.Show(this);
        }

        private void OnEventFailed(AdsPlacement placement, string error)
        {
            _skipped?.Invoke();
            Clear();
        }

        internal void DispatchEarned(IRewardAdInfo info)
        {
            EventEarned?.Invoke(this, info);
            _earned?.Invoke();
            Clear();
        }

        internal void DispatchSkipped()
        {
            EventSkipped?.Invoke(this);
            _skipped?.Invoke();
            Clear();
        }

        private void Clear()
        {
            _earned = null;
            _skipped = null;
        }
    }
}