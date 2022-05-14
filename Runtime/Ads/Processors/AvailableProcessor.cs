using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using GameKit.Ads.Placements;
using GameKit.Ads.Units;
using JetBrains.Annotations;
using UnityEngine;

namespace GameKit.Ads.Processors
{
    [PublicAPI]
    internal class AvailableProcessor
    {
        private List<IAdUnit> _units;

        public readonly AdsPlacement Placement;
        public readonly Type UnitType;
        
        public bool Enabled { get; private set; }
        
        public AvailableProcessor(AdsPlacement placement, Type unitType, List<IAdUnit> units)
        {
            Placement = placement;
            UnitType = unitType;
            _units = units;
            Enabled = true;
        }

        public void Disable()
        {
            Enabled = false;
            if (Placement.IsFetched)
                Placement.DispatchNoFill();
        }
        
        public void UpdateFetchedState()
        {
            if (FindLoaded() == false)
            {
                if (Placement.IsFetched)
                    Placement.DispatchNoFill();
            }
            else
            {
                if (Placement.IsFetched == false)
                    Placement.DispatchFetched();
            }
        }
        
        private bool FindLoaded()
        {
            foreach (var unit in _units)
            {
                if (unit.State == AdUnitState.Loaded)
                {
                    return true;
                }
            }
            return false;
        }
    }
}