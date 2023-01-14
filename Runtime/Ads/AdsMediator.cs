using System;
using System.Collections;
using System.Collections.Generic;
using GameKit.Ads.Networks;
using GameKit.Ads.Placements;
using GameKit.Ads.Processors;
using GameKit.Ads.Units;
using JetBrains.Annotations;
using UnityEngine;


namespace GameKit.Ads
{
    [PublicAPI]
    public class AdsMediator
    {
        private Dictionary<Type, List<IAdUnit>> _typedUnits = new Dictionary<Type, List<IAdUnit>>();
        private Dictionary<Type, DisplayProcessor> _displayProcessors = new Dictionary<Type, DisplayProcessor>(3);
        private List<AvailableProcessor> _availableProcessors = new  List<AvailableProcessor>(3);
        private List<IAdsNetwork> _networks = new List<IAdsNetwork>();
        private ISession<AdsSession> _session = Session.Resolve<AdsSession>(Session.Groups.UnityPlayerPrefs);

        public event AdsEventHandler EventNoFill = delegate { };
        public event AdsEventHandler EventFetched = delegate { };
        public event AdsInfoEventHandler EventDisplayed = delegate { };
        public event AdsInfoEventHandler EventClicked = delegate { };
        public event AdsEventHandler EventClosed = delegate { };
        public event AdsErrorEventHandler EventFailed = delegate { };
        public event AdsRewardInfoEventHandler EventEarned = delegate { };
        public event AdsEventHandler EventSkipped = delegate { };
        public bool UseMockInDebugBuild { get; set; }
        public bool IsInitialized { get; set; }
        public int RelevantAdsConsent => _session.Get().relevantAdsConsent;
        
        public void RegisterPlacement(AdsPlacement placement)
        {
            if (Logger<AdsMediator>.IsDebugAllowed) 
                Logger<AdsMediator>.Debug($"{placement.DebugName}|Registered");
            Subscribe(placement);
            AddAvailableProcessor(placement);
        }

        public void RegisterNetwork(IAdsNetwork network)
        {
            _networks.Add(network);
            if (Logger<AdsMediator>.IsDebugAllowed) 
                Logger<AdsMediator>.Debug($"{network.GetType().Name}|Registered");
        }
        
        public void Show(AdsPlacement placement)
        {
            if (_displayProcessors.TryGetValue(placement.UnitType, out var processor) == false)
            {
                if (Logger<AdsMediator>.IsErrorAllowed) 
                    Logger<AdsMediator>.Error($"{placement.DebugName}|Display processor for {placement.UnitType} not found");
                return;
            }
            if (Logger<AdsMediator>.IsDebugAllowed) 
                Logger<AdsMediator>.Debug($"{placement.DebugName}|Request show placement ads");
            processor.Show(placement, _typedUnits[placement.UnitType]);
        }

        public void Hide(AdsSmartBannerPlacement placement)
        {
            if (_displayProcessors.TryGetValue(placement.UnitType, out var processor) == false)
            {
                if (Logger<AdsMediator>.IsErrorAllowed) 
                    Logger<AdsMediator>.Error($"{placement.DebugName}|Display processor for {placement.UnitType} not found");
                return;
            }

            if (processor.DisplayedUnit is IHiddenBanner banner)
            {
                banner.Hide();
            } 
            else if (Logger<AdsMediator>.IsErrorAllowed)
                Logger<AdsMediator>.Error($"{placement.DebugName}|Display processor for {placement.UnitType} not support Hide method");
        }

        public AdsMediator()
        {
            if (Logger<AdsMediator>.IsDebugAllowed) 
                Logger<AdsMediator>.Debug($"service created");
            
            BindDisplayProcessor<IRewardedVideoAdUnit>(new DisplayProcessor());
            BindDisplayProcessor<IInterstitialAdUnit>(new DisplayProcessor());
            BindDisplayProcessor<ITopSmartBannerAdUnit>(new DisplayProcessor());
            BindDisplayProcessor<IBottomSmartBannerAdUnit>(new DisplayProcessor());

            Loop.StartCoroutine(Initialize());
        }

        public void BindDisplayProcessor<TUnitType>(DisplayProcessor processor) where TUnitType : IAdUnit
        {
            _displayProcessors[typeof(TUnitType)] = processor;
        }

        public void SetupRelevantAdsConsent(bool granted)
        {
            AdsSession s = _session.Get();
            s.relevantAdsConsent = granted ? 1 : -1;
            _session.Save(s);
        }
        
        public void EnableIntrusiveAdUnits()
        {
            if (Logger<AdsMediator>.IsDebugAllowed) 
                Logger<AdsMediator>.Debug($"service enable intrusive ad units");
            var s = _session.Get();
            s.disableIntrusiveAdUnits = false;
            _session.Save(s);
        }
        
        public void DisableIntrusiveAdUnits()
        {
            if (Logger<AdsMediator>.IsDebugAllowed) 
                Logger<AdsMediator>.Debug($"service disable intrusive ad units");
            var s = _session.Get();
            s.disableIntrusiveAdUnits = true;
            _session.Save(s);

            foreach (var displayProcessor in _displayProcessors.Values)
            {
                if (displayProcessor.DisplayedUnit is IHiddenBanner banner && banner.State == AdUnitState.Displayed)
                {
                    banner.Hide();
                }
            }

            var rewardedUnitType = typeof(IRewardedVideoAdUnit);
            foreach (var processor in _availableProcessors)
            {
                if (processor.UnitType == rewardedUnitType) continue;
                processor.Disable();
            }
        }

        private void AddAvailableProcessor(AdsPlacement placement)
        {
            Type unitType = placement.UnitType;
            if (_typedUnits.TryGetValue(unitType, out var units) == false)
            {
                _typedUnits[unitType] = units = new List<IAdUnit>();
                if (IsInitialized) InitializeUnitsFoType(unitType);
            }

            var processor = new AvailableProcessor(placement, unitType, units);
            _availableProcessors.Add(processor);
            if (_session.Get().disableIntrusiveAdUnits) processor.Disable();
        }

        private void InitializeUnitsFoType(Type unitType)
        {
            if (_typedUnits.TryGetValue(unitType, out var units) == false)
            {
                _typedUnits[unitType] = units = new List<IAdUnit>();
            }
            
            foreach (var network in _networks)
            {
                if (network.IsSupported(unitType))
                    units.AddRange(network.GetUnits(unitType));
            }
        }
        
        private IEnumerator Initialize()
        {
            if (Logger<AdsMediator>.IsDebugAllowed) 
                Logger<AdsMediator>.Debug($"service initializing");
            
            if ((Logger<AdsMediator>.IsDebugAllowed || Application.isEditor) && RelevantAdsConsent == 0)
                Logger<AdsMediator>.Debug($"waiting relevant ads consent");

            while (RelevantAdsConsent == 0)
                yield return null;
            
            if (UseMockInDebugBuild && Debug.isDebugBuild)
            {
                _networks = new List<IAdsNetwork>(1) { new MockAdsNetwork() };
            }

            if (_networks.Count == 0)
            {
                if (Logger<AdsMediator>.IsWarningAllowed)
                    Logger<AdsMediator>.Warning($"Network list is empty");
                
#if UNITY_EDITOR
                _networks = new List<IAdsNetwork>(1) { new MockAdsNetwork() };
#else
                yield break;
#endif
            }

            Dictionary<TaskRoutine, IAdsNetwork> routines = new Dictionary<TaskRoutine, IAdsNetwork>();
            var intrusiveAdUnits = !_session.Get().disableIntrusiveAdUnits;
            if (Logger<AdsMediator>.IsDebugAllowed)
                Logger<AdsMediator>.Debug($"Initialize networks with args [IntrusiveAdUnits: {intrusiveAdUnits}]");
            foreach (IAdsNetwork network in _networks)
            {
                routines.Add(network.Initialize(RelevantAdsConsent == 1, intrusiveAdUnits), network);
            }

            bool initialized = false;
            while (initialized == false)
            {
                yield return null;
                initialized = true;
                foreach (var routine in routines)
                {
                    initialized = initialized && routine.Key.IsCompleted;
                }
            }

            foreach (var routine in routines)
            {
                if (routine.Key.IsCompletedSuccessfully != true) _networks.Remove(routine.Value);
            }

            IsInitialized = true;
            
            foreach (var unit in _typedUnits)
            {
                InitializeUnitsFoType(unit.Key);
            }

            if (_networks.Count == 0 && Logger<AdsMediator>.IsWarningAllowed)
                Logger<AdsMediator>.Warning($"service initialized with {_networks.Count} networks");
            else if (Logger<AdsMediator>.IsDebugAllowed) 
                Logger<AdsMediator>.Debug($"service initialized with {_networks.Count} networks");
            
            Loop.StartCoroutine(AvailableLoop());
        }

        private IEnumerator AvailableLoop()
        {
            if (Logger<AdsMediator>.IsDebugAllowed) 
                Logger<AdsMediator>.Debug($"AvailableLoop started with {_availableProcessors.Count} processors");

            var waiter = new WaitForSeconds(1);
            while (Loop.IsQuitting == false)
            {
                yield return waiter;
                
                int count = _availableProcessors.Count;
                for (var i = 0; i < count; i++)
                {
                    if (_availableProcessors[i].Enabled)
                        _availableProcessors[i].UpdateFetchedState();
                }
            }
        }
        
        #region Proxy Events

        private void Subscribe(AdsPlacement placement)
        {
            placement.EventClosed += DispatchClosed;
            placement.EventDisplayed += DispatchDisplayed;
            placement.EventFailed += DispatchFailed;
            placement.EventNoFill += DispatchNoFill;
            placement.EventFetched += DispatchFetched;
            placement.EventClicked += DispatchClicked;

            if (placement is AdsRewardedVideoPlacement reward)
            {
                reward.EventSkipped += DispatchSkipped;
                reward.EventEarned += DispatchEarned;
            }
        }
        
        private void DispatchFetched(AdsPlacement placement)
        {
            EventFetched(placement);
        }

        private void DispatchNoFill(AdsPlacement placement)
        {
            EventNoFill(placement);
        }

        private void DispatchDisplayed(AdsPlacement placement, IAdInfo info)
        {
            EventDisplayed(placement, info);
        }

        private void DispatchClosed(AdsPlacement placement)
        {
            EventClosed(placement);
        }

        private void DispatchFailed(AdsPlacement placement, string error)
        {
            EventFailed(placement, error);
        }

        private void DispatchEarned(AdsPlacement placement, IRewardAdInfo info)
        {
            EventEarned?.Invoke(placement, info);
        }

        private void DispatchSkipped(AdsPlacement placement)
        {
            EventSkipped?.Invoke(placement);
        }

        private void DispatchClicked(AdsPlacement placement, IAdInfo info)
        {
            EventClicked?.Invoke(placement, info);
        }
        
        #endregion
    }
}