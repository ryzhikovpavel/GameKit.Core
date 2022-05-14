// using System.Collections;
// using System.Collections.Generic;
// using GameKit.Ads.Placements;
// using GameKit.Ads.Requests;
// using GameKit.Ads.Units;
// using UnityEngine;

namespace GameKit.Ads.Processors
{
    // internal class AnchoredRepeatedDisplayProcessor : DisplayProcessor
    // {
    //     private const int MinUpdateFrequency = 30;
    //     private ITopSmartBannerAdUnit _unit;
    //     private float _timeRemaining;
    //     
    //     public bool IsActive { get; private set; }
    //     
    //     public void Hide()
    //     {
    //         IsActive = false;
    //         if (_unit is null == false) _unit.Hide();
    //     }
    //     
    //     public override void Show(AdsPlacement placement, IAdRequestData request, IEnumerable<IAdUnit> units)
    //     {
    //         IsActive = true;
    //         Loop.StartCoroutine(RepeatProcess(placement, request, units));
    //     }
    //     
    //     private IEnumerator RepeatProcess(AdsPlacement placement, IAdRequestData request, IEnumerable<IAdUnit> units)
    //     {
    //         if (Logger<AdsMediator>.IsDebugAllowed) 
    //             Logger<AdsMediator>.Debug($"{placement.Name}|Start repeating process");
    //         
    //         while (IsActive)
    //         {
    //             if (placement.IsFetched == false)
    //             {
    //                 yield return null;
    //                 continue;
    //             }
    //             
    //             if (Logger<AdsMediator>.IsDebugAllowed) 
    //                 Logger<AdsMediator>.Debug($"{placement.Name}|Request show unit");
    //             
    //             // ReSharper disable once PossibleMultipleEnumeration
    //             base.Show(placement, request, units);
    //             _timeRemaining = MinUpdateFrequency;
    //             if (request is IAdUpdateFrequencyRequestData data && data.UpdateFrequency > _timeRemaining)
    //                 _timeRemaining = data.UpdateFrequency;
    //
    //             if (Logger<AdsMediator>.IsDebugAllowed) 
    //                 Logger<AdsMediator>.Debug($"{placement.Name}|Wait {_timeRemaining} seconds for update ad unit");
    //
    //             while (_timeRemaining > 0 && _unit is null == false)
    //             {
    //                 yield return null;
    //                 _timeRemaining -= Time.deltaTime;
    //             }
    //             
    //             if (Logger<AdsMediator>.IsDebugAllowed) 
    //                 Logger<AdsMediator>.Debug($"{placement.Name}|Request hide unit");
    //             if (_unit is null == false) _unit.Hide();
    //             if (_unit is null == false) yield return null;
    //         }
    //         
    //         if (Logger<AdsMediator>.IsDebugAllowed) 
    //             Logger<AdsMediator>.Debug($"{placement.Name}|Stop repeating process");
    //     }
    //
    //     protected override bool SetupUnit(IAdUnit unit, IAdRequestData request)
    //     {
    //         _unit = unit as ITopSmartBannerAdUnit;
    //         return base.SetupUnit(unit, request);
    //     }
    //
    //     protected override void DoCloseAd(IAdUnit unit, AdsPlacement placement)
    //     {
    //         base.DoCloseAd(unit, placement);
    //         _unit = null;
    //     }
    // }
}