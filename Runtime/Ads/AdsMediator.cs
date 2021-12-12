using System;
using System.Collections;
using System.Collections.Generic;
using GameKit.Ads.Networks;
using GameKit.Ads.Placements;
using GameKit.Ads.Processors;
using GameKit.Ads.Requests;
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
        private List<AvailableProcessor> _availableProcessors = new List<AvailableProcessor>(3);
        private List<IAdsNetwork> _networks;
        
        public event AdsEventHandler EventNoFill = delegate { };
        public event AdsEventHandler EventFetched = delegate { };
        public event AdsInfoEventHandler EventDisplayed = delegate { };
        public event AdsInfoEventHandler EventClicked = delegate { };
        public event AdsEventHandler EventClosed = delegate { };
        public event AdsErrorEventHandler EventFailed = delegate { };
        public event AdsRewardInfoEventHandler EventEarned = delegate { };
        public event AdsEventHandler EventSkipped = delegate { };

        public bool UseMockInDebugBuild { get; set; }
        
        public void RegisterPlacement(AdsPlacement placement)
        {
            Subscribe(placement);
            AddAvailableProcessor(placement);
        }
        
        public void Show(AdsPlacement placement, IAdRequestData requestData)
        {
            if (_displayProcessors.TryGetValue(placement.GetUnitType(), out var processor) == false)
            {
                if (Service<AdsMediator>.Logger.IsErrorAllowed) 
                    Service<AdsMediator>.Logger.Error($"{placement.Name}|Display processor for {placement.GetUnitType()} not found");
                return;
            }
            if (Service<AdsMediator>.Logger.IsDebugAllowed) 
                Service<AdsMediator>.Logger.Debug($"{placement.Name}|Request show placement ads");
            processor.Show(placement, requestData, _typedUnits[placement.GetUnitType()]);
        }

        public void Hide(AdsAnchoredBannerPlacement placement)
        {
            if (_displayProcessors.TryGetValue(placement.GetUnitType(), out var processor) == false)
            {
                if (Service<AdsMediator>.Logger.IsErrorAllowed) 
                    Service<AdsMediator>.Logger.Error($"{placement.Name}|Display processor for {placement.GetUnitType()} not found");
                return;
            }

            if (processor is AnchoredRepeatedDisplayProcessor repeatedProcessor)
            {
                repeatedProcessor.Hide();
            } 
            else 
                if (Service<AdsMediator>.Logger.IsErrorAllowed) 
                    Service<AdsMediator>.Logger.Error($"{placement.Name}|Display processor for {placement.GetUnitType()} not support Hide method");
        }

        public AdsMediator()
        {
            BindDisplayProcessor<IRewardedVideoAdUnit>(new DisplayProcessor());
            BindDisplayProcessor<IInterstitialAdUnit>(new DisplayProcessor());
            BindDisplayProcessor<IAnchoredBannerAdUnit>(new AnchoredRepeatedDisplayProcessor());

            Loop.StartCoroutine(Initialize());
        }

        public void BindDisplayProcessor<TUnitType>(DisplayProcessor processor) where TUnitType : IAdUnit
        {
            _displayProcessors[typeof(TUnitType)] = processor;
        }

        private void AddAvailableProcessor(AdsPlacement placement)
        {
            Type unitType = placement.GetUnitType();
            if (_typedUnits.TryGetValue(unitType, out var units) == false)
            {
                _typedUnits[unitType] = units = new List<IAdUnit>();
                if (_networks is null == false) InitializeUnitsFoType(unitType);
            }
            _availableProcessors.Add(new AvailableProcessor(placement, units));
        }

        private void InitializeUnitsFoType(Type unitType)
        {
            if (_typedUnits.TryGetValue(unitType, out var units) == false)
            {
                _typedUnits[unitType] = units = new List<IAdUnit>();
            }
            
            foreach (var network in _networks)
            {
                units.AddRange(network.GetUnits(unitType));                        
            }
        }
        
        private IEnumerator Initialize()
        {
            if (UseMockInDebugBuild && Debug.isDebugBuild)
            {
                _networks = new List<IAdsNetwork>(1) { new MockAdsNetwork() };
            }
            else
            {
                var resNetworks = Resources.LoadAll<AdsNetwork>("Networks");
                if (resNetworks is null || resNetworks.Length == 0)
                {
                    if (Service<AdsMediator>.Logger.IsWarningAllowed) 
                        Service<AdsMediator>.Logger.Warning($"Network list is empty");
                    yield break;
                }
                _networks = new List<IAdsNetwork>(resNetworks);
            }

            foreach (var network in _networks)
            {
                network.Initialize(false, Session<AdsSession>.Get().purchasedDisableUnits); //TODO fix tracking consent
            }

            bool initialized = false;
            while (initialized == false)
            {
                yield return null;
                initialized = true;
                foreach (var network in _networks) initialized = initialized && network.IsInitialized;
            }

            foreach (var unit in _typedUnits)
            {
                InitializeUnitsFoType(unit.Key);
            }
            
            Loop.StartCoroutine(AvailableLoop());
        }

        private IEnumerator AvailableLoop()
        {
            while (Loop.IsQuitting == false)
            {
                yield return null;
                
                int count = _availableProcessors.Count;
                for (var i = 0; i < count; i++) _availableProcessors[i].UpdateFetchedState();
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