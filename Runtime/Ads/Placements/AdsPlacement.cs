using System;
using GameKit.Ads.Units;
using JetBrains.Annotations;

namespace GameKit.Ads.Placements
{
    [PublicAPI]
    public abstract class AdsPlacement
    {
        protected ILogger Logger = Logger<AdsMediator>.Instance;

        public event AdsEventHandler EventNoFill = delegate { };
        public event AdsEventHandler EventFetched = delegate { };
        public event AdsInfoEventHandler EventDisplayed = delegate { };
        public event AdsEventHandler EventClosed = delegate { };
        public event AdsErrorEventHandler EventFailed = delegate { };
        public event AdsInfoEventHandler EventClicked = delegate { };
        public event AdsEventHandler EventReady = delegate { };

        public readonly string Name;
        public string DebugName { get; private set; }

        /// <summary>
        /// Advertising is loaded and ready to displayed
        /// </summary>
        public bool IsFetched { get; private set; }

        /// <summary>
        /// Advertising meets the conditions of display
        /// </summary>
        public virtual bool IsAvailable => true;

        /// <summary>
        /// IsFetched && IsAvailable
        /// </summary>
        public bool IsReady => IsFetched && IsAvailable;

        /// <summary>
        /// True on ad unit displayed
        /// </summary>
        public bool IsDisplayed { get; private set; }

        protected AdsPlacement(string name)
        {
            Name = name;
            DebugName = name;
            Loop.InvokeDelayed(0, Initialize);
        }

        private void Initialize()
        {
            if (DebugName.IsNullOrEmpty()) DebugName = GetType().Name;
            Service<AdsMediator>.Instance.RegisterPlacement(this);
        }

        protected void DispatchReady()
        {
            if (Logger.IsDebugAllowed) Logger.Debug($"{DebugName} is ready");
            EventReady(this);
        }

        internal void DispatchFetched()
        {
            IsFetched = true;
            if (Logger.IsDebugAllowed) Logger.Debug($"{DebugName} is fetched");
            EventFetched(this);
            
            if (IsReady) DispatchReady();
        }

        internal void DispatchNoFill()
        {
            IsFetched = false;
            if (Logger.IsDebugAllowed) Logger.Debug($"{DebugName} is no fill");
            EventNoFill(this);
        }

        internal void DispatchDisplayed(IAdInfo info)
        {
            if (Logger.IsDebugAllowed) Logger.Debug($"{DebugName} is displayed");
            IsDisplayed = true;
            EventDisplayed(this, info);
        }

        internal void DispatchClosed()
        {
            if (Logger.IsDebugAllowed) Logger.Debug($"{DebugName} is closed");
            IsDisplayed = false;
            EventClosed(this);
        }

        internal void DispatchFailed(string error)
        {
            if (Logger.IsDebugAllowed) Logger.Debug($"{DebugName} is failed");
            EventFailed(this, error);
        }

        internal void DispatchClicked(IAdInfo info)
        {
            if (Logger.IsDebugAllowed) Logger.Debug($"{DebugName} is clocked");
            EventClicked(this, info);
        }
        
        [NotNull] public abstract Type UnitType { get; }
    }
}