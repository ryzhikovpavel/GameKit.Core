using System;
using GameKit.Ads.Units;
using JetBrains.Annotations;

namespace GameKit.Ads.Placements
{
    [PublicAPI]
    public abstract class AdsPlacement
    {
        public event AdsEventHandler EventNoFill = delegate { };
        public event AdsEventHandler EventFetched = delegate { };
        public event AdsInfoEventHandler EventDisplayed = delegate { };
        public event AdsEventHandler EventClosed = delegate { };
        public event AdsErrorEventHandler EventFailed = delegate { };
        public event AdsInfoEventHandler EventClicked = delegate { };
        
        public readonly string Name;
        public bool IsFetched { get; private set; }

        public virtual bool IsAvailable => true;

        protected AdsPlacement(string name)
        {
            Name = name;
            Service<AdsMediator>.Instance.RegisterPlacement(this);
        }

        internal void DispatchFetched()
        {
            IsFetched = true;
            EventFetched(this);
        }

        internal void DispatchNoFill()
        {
            IsFetched = false;
            EventNoFill(this);
        }

        internal void DispatchDisplayed(IAdInfo info)
        {
            EventDisplayed(this, info);
        }

        internal void DispatchClosed()
        {
            EventClosed(this);
        }

        internal void DispatchFailed(string error)
        {
            EventFailed(this, error);
        }

        internal void DispatchClicked(IAdInfo info)
        {
            EventClicked(this, info);
        }
        
        public abstract Type GetUnitType();
    }

    public abstract class AdsPlacement<TUnit>: AdsPlacement where TUnit : IAdUnit
    {
        public override Type GetUnitType() => typeof(TUnit);
        protected AdsPlacement(string name) : base(name) { }
    }
}