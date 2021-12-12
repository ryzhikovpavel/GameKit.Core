﻿using System;
using System.IO;
using GameKit.Ads.Units;
using JetBrains.Annotations;

namespace GameKit.Ads.Placements
{
    [PublicAPI]
    public class AdsInterstitialPlacement: AdsPlacement<IInterstitialAdUnit>
    {
        private float _frequencyCapping;
        private DateTime _lastDisplayedTime;
        private Action _completed; 
        
        public override bool IsAvailable => (DateTime.Now - _lastDisplayedTime).TotalSeconds >= _frequencyCapping;
        
        public AdsInterstitialPlacement(string name, float frequencyCapping) : base(name)
        {
            _frequencyCapping = frequencyCapping;
            EventClosed += OnClosed;
            EventFailed += OnFailed;
        }
        
        public virtual void Show()
        {
            if (IsAvailable == false) return;
            if (IsFetched == false) throw new Exception("Ad units not loaded");
            Service<AdsMediator>.Instance.Show(this, null);
        }

        public virtual void Show(Action completed)
        {
            _completed = completed;
            Show();
        }

        private void OnClosed(AdsPlacement placement)
        {
            _completed?.Invoke();
            _completed = null;
        }
        
        private void OnFailed(AdsPlacement placement, string error)
        {
            _completed?.Invoke();
            _completed = null;
        }
    }
}