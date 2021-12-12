using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GameKit.Ads.Placements;
using GameKit.Ads.Requests;
using GameKit.Ads.Units;
using JetBrains.Annotations;

namespace GameKit.Ads.Processors
{
    [PublicAPI]
    public class DisplayProcessor
    {
        public virtual void Show(AdsPlacement placement, IAdRequestData request, IEnumerable<IAdUnit> units)
        {
            Loop.StartCoroutine(ShowProcess(placement, request, units));
        }
        
        private IEnumerator ShowProcess(AdsPlacement placement, IAdRequestData request, IEnumerable<IAdUnit> units)
        {
            if (FindLoaded(units, out var unit) == false)
            {
                if (Service<AdsMediator>.Logger.IsErrorAllowed) 
                    Service<AdsMediator>.Logger.Error($"{placement.Name}|Ad units not loaded. Request show break");
                placement.DispatchFailed("Ad units not loaded. Request show break");
                yield break;
            }

            if (Service<AdsMediator>.Logger.IsDebugAllowed) 
                Service<AdsMediator>.Logger.Debug($"{placement.Name}|Ad unit found. Setup unit with request data and show");
            
            if (SetupUnit(unit, request) == false) yield break;
            unit.Show();
            
            while (unit.State == AdUnitState.Loaded) yield return null;
            if (unit.State == AdUnitState.Error)
            {
                if (Service<AdsMediator>.Logger.IsErrorAllowed) 
                    Service<AdsMediator>.Logger.Error($"{placement.Name}|Ad unit error: {unit.Error}");
                placement.DispatchFailed(unit.Error);
                yield break;
            }

            if (unit.State != AdUnitState.Displayed)
            {
                if (Service<AdsMediator>.Logger.IsWarningAllowed) 
                    Service<AdsMediator>.Logger.Warning($"{placement.Name}|Ad unit state not displayed. Current state: {unit.State}");
            }
            
            placement.DispatchDisplayed(unit.Info);
            if (Service<AdsMediator>.Logger.IsDebugAllowed) 
                Service<AdsMediator>.Logger.Debug($"{placement.Name}|Ad unit displayed");
            
            while (unit.State == AdUnitState.Displayed) yield return null;
            
            if (unit.State == AdUnitState.Error)
            {
                if (Service<AdsMediator>.Logger.IsErrorAllowed) 
                    Service<AdsMediator>.Logger.Error($"{placement.Name}|Ad unit close with error: {unit.Error}");
                placement.DispatchFailed(unit.Error);
                yield break;
            }

            if (unit.State == AdUnitState.Clicked)
            {
                if (Service<AdsMediator>.Logger.IsDebugAllowed) 
                    Service<AdsMediator>.Logger.Debug($"{placement.Name}|Ad unit clicked");
                placement.DispatchClicked(unit.Info);
            }

            if (unit.State is AdUnitState.Closed or AdUnitState.Clicked == false)
            {
                if (Service<AdsMediator>.Logger.IsWarningAllowed) 
                    Service<AdsMediator>.Logger.Warning($"{placement.Name}|Ad unit state not closed or clicked. Current state: {unit.State}");
            }
            DoCloseAd(unit, placement);
            placement.DispatchClosed();
            if (Service<AdsMediator>.Logger.IsDebugAllowed) 
                Service<AdsMediator>.Logger.Debug($"{placement.Name}|Ad unit closed");
            
            unit.Release();
        }

        protected virtual bool SetupUnit(IAdUnit unit, IAdRequestData request)
        {
            if (unit is IAnchoredBannerAdUnit anchorUnit)
            {
                if (request is IAdAnchorableRequestData anchorRequest)
                {
                    anchorUnit.SetAnchor(anchorRequest.Anchor);
                }
                else
                {
                    if (Service<AdsMediator>.Logger.IsErrorAllowed) 
                        Service<AdsMediator>.Logger.Error($"RequestData|Anchor request data not found");
                    return false;
                }
            }
            return true;
        }

        protected virtual void DoCloseAd(IAdUnit unit, AdsPlacement placement)
        {
            if (unit is IRewardedVideoAdUnit rewardedUnit && placement is AdsRewardedVideoPlacement rewardedPlacement)
            {
                if (rewardedUnit.IsEarned)
                {
                    if (Service<AdsMediator>.Logger.IsDebugAllowed) 
                        Service<AdsMediator>.Logger.Debug($"{placement.Name}|Ad rewarded unit earned");
                    rewardedPlacement.DispatchEarned(rewardedUnit.Reward);
                }
                else
                {
                    if (Service<AdsMediator>.Logger.IsDebugAllowed) 
                        Service<AdsMediator>.Logger.Debug($"{placement.Name}|Ad rewarded unit skipped");
                    rewardedPlacement.DispatchSkipped();
                }
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool FindLoaded(IEnumerable<IAdUnit> units, out IAdUnit foundUnit)
        {
            foreach (var unit in units)
            {
                if (unit.IsLoaded)
                {
                    foundUnit = unit;
                    return true;
                }
            }

            foundUnit = null;
            return false;
        }
    }
}