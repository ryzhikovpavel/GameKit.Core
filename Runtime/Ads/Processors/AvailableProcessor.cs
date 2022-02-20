using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GameKit.Ads.Placements;
using GameKit.Ads.Units;
using JetBrains.Annotations;

namespace GameKit.Ads.Processors
{
    [PublicAPI]
    internal class AvailableProcessor
    {
        private AdsPlacement _placement;
        private IEnumerable<IAdUnit> _units;
        
        public AvailableProcessor(AdsPlacement placement, IEnumerable<IAdUnit> units)
        {
            _placement = placement;
            _units = units;
        }
        
        public void UpdateFetchedState()
        {
            if (FindLoaded() == false)
            {
                if (_placement.IsFetched)
                    _placement.DispatchNoFill();
            }
            else
            {
                if (_placement.IsFetched == false)
                    _placement.DispatchFetched();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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