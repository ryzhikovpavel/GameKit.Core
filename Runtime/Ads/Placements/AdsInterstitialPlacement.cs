using System;
using System.IO;
using GameKit.Ads.Units;
using JetBrains.Annotations;
using UnityEngine;

namespace GameKit.Ads.Placements
{
    [PublicAPI]
    public class AdsInterstitialPlacement: AdsPlacement
    {
        private float _frequencyCapping;
        private DateTime _lastDisplayedTime;
        private Action _completed; 
        
        public override bool IsAvailable => (DateTime.Now - _lastDisplayedTime).TotalSeconds >= _frequencyCapping;
        public override Type UnitType { get; } = typeof(IInterstitialAdUnit);

        public AdsInterstitialPlacement(float frequencyCapping) : this(null, frequencyCapping) { }
        public AdsInterstitialPlacement(string name, float frequencyCapping) : base(name)
        {
            _frequencyCapping = frequencyCapping;
            EventClosed += OnClosed;
            EventFailed += OnFailed;
        }
        
        public virtual void Show()
        {
            if (IsReady == false) return;
            Service<AdsMediator>.Instance.Show(this);
        }

        public virtual void Show(Action completed)
        {
            _completed = completed;
            if (IsReady) Service<AdsMediator>.Instance.Show(this);
            else
            {
                _completed?.Invoke();
                _completed = null;
            }
        }

        private void OnClosed(AdsPlacement placement)
        {
            _completed?.Invoke();
            _completed = null;
            _lastDisplayedTime = DateTime.Now;
        }
        
        private void OnFailed(AdsPlacement placement, string error)
        {
            _completed?.Invoke();
            _completed = null;
        }
    }
}