using System;
using UnityEngine;

namespace GameKit.Rating.Implementation
{
    internal class EditorPlatformReview : NativeRating
    {
        public EditorPlatformReview()
        {
            IsReady = true;
        }
        
        public override void Open(Action onComplete)
        {
            Application.OpenURL("https://play.google.com/store/apps/details?id="+Application.identifier);
            onComplete?.Invoke();
            RatedDispatch();
        }
        public override void Dispose() { }
    }
}