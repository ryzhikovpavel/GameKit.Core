using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GameKit.Ads.Placements;
using GameKit.Ads.Units;
using JetBrains.Annotations;

namespace GameKit.Ads.Processors
{
    [PublicAPI]
    public class DisplayProcessor
    {
        public IAdUnit DisplayedUnit { get; private set; }
        
        public virtual void Show(AdsPlacement placement, IEnumerable<IAdUnit> units)
        {
            Loop.StartCoroutine(ShowProcess(placement, units));
        }
        
        private IEnumerator ShowProcess(AdsPlacement placement, IEnumerable<IAdUnit> units)
        {
            if (FindLoaded(units, out var unit) == false)
            {
                if (Logger<AdsMediator>.IsErrorAllowed) 
                    Logger<AdsMediator>.Error($"{placement.DebugName}|Ad units not loaded. Request show break");
                placement.DispatchFailed("Ad units not loaded. Request show break");
                yield break;
            }

            if (Logger<AdsMediator>.IsDebugAllowed) 
                Logger<AdsMediator>.Debug($"{placement.DebugName}|Ad unit found. Setup unit with request data and show");
            
            DisplayedUnit = unit;
            if (SetupUnit(unit) == false) yield break;
            
            placement.DispatchDisplayed(unit.Info);
            if (Logger<AdsMediator>.IsDebugAllowed) 
                Logger<AdsMediator>.Debug($"{placement.DebugName}|Ad unit displayed");
            
            unit.Show();
            
            while (unit.State == AdUnitState.Loaded) yield return null;
            while (unit.State == AdUnitState.Displayed) yield return null;
            
            if (unit.State == AdUnitState.Error)
            {
                if (Logger<AdsMediator>.IsErrorAllowed) 
                    Logger<AdsMediator>.Error($"{placement.DebugName}|Ad unit error: {unit.Error}");
                placement.DispatchFailed(unit.Error);
                DoCloseAd(unit, placement);
                placement.DispatchClosed();
                yield break;
            }
            
            if (unit.State == AdUnitState.Clicked)
            {
                if (Logger<AdsMediator>.IsDebugAllowed) 
                    Logger<AdsMediator>.Debug($"{placement.DebugName}|Ad unit clicked");
                placement.DispatchClicked(unit.Info);
            }

            if (unit.State is AdUnitState.Closed or AdUnitState.Clicked == false)
            {
                if (Logger<AdsMediator>.IsWarningAllowed) 
                    Logger<AdsMediator>.Warning($"{placement.DebugName}|Ad unit state not closed or clicked. Current state: {unit.State}");
            }
            DoCloseAd(unit, placement);
            placement.DispatchClosed();
            if (Logger<AdsMediator>.IsDebugAllowed) 
                Logger<AdsMediator>.Debug($"{placement.DebugName}|Ad unit closed");
            
            unit.Release();
            DisplayedUnit = null;
        }

        protected virtual bool SetupUnit(IAdUnit unit)
        {
            return true;
        }

        protected virtual void DoCloseAd(IAdUnit unit, AdsPlacement placement)
        {
            if (unit is IRewardedVideoAdUnit rewardedUnit && placement is AdsRewardedVideoPlacement rewardedPlacement)
            {
                if (rewardedUnit.IsEarned)
                {
                    if (Logger<AdsMediator>.IsDebugAllowed) 
                        Logger<AdsMediator>.Debug($"{placement.DebugName}|Ad rewarded unit earned");
                    rewardedPlacement.DispatchEarned(rewardedUnit.Reward);
                }
                else
                {
                    if (Logger<AdsMediator>.IsDebugAllowed) 
                        Logger<AdsMediator>.Debug($"{placement.DebugName}|Ad rewarded unit skipped");
                    rewardedPlacement.DispatchSkipped();
                }
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool FindLoaded(IEnumerable<IAdUnit> units, out IAdUnit foundUnit)
        {
            foreach (var unit in units)
            {
                if (unit.State == AdUnitState.Loaded)
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