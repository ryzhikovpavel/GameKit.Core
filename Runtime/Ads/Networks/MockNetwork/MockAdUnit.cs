using System.Collections;
using GameKit.Ads.Networks.MockBehaviour;
using GameKit.Ads.Units;
using UnityEngine;

namespace GameKit.Ads.Networks
{
    internal abstract class MockAdUnit : IAdUnit
    {
        protected MockCanvas Canvas;

        public AdUnitState State { get; protected set; }
        public string Error { get; protected set;}
        public IAdInfo Info { get; } = new DefaultAdInfo();

        public abstract void Show();
        
        public void Release()
        {
            Loop.StartCoroutine(Load());
        }

        public MockAdUnit(MockCanvas canvas)
        {
            Canvas = canvas;
            State = AdUnitState.Loaded;
        }

        private IEnumerator Load()
        {
            yield return new WaitForSecondsRealtime(1f);
            State = AdUnitState.Loaded;
        }
    }
}