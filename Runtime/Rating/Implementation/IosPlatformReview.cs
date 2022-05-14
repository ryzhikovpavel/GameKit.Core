#if UNITY_IOS
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace GameKit.Rating.Implementation
{
    internal class IosPlatformReview : NativeRating
    {
        private IEnumerator _process;

        public IosPlatformReview()
        {
            IsReady = true;
        }

        public override void Open(Action onComplete)
        {
            Loop.StartCoroutine(_process = Show(onComplete));
        }

        public override void Dispose()
        {
            if (_process != null)
            {
                Loop.StopCoroutine(_process);
                _process = null;
            }
        }

        private IEnumerator Show(Action onComplete)
        {
            bool isNativeDialog = false;
            var time = Stopwatch.StartNew();
            isNativeDialog = UnityEngine.iOS.Device.RequestStoreReview();
            time.Stop();
            if (isNativeDialog == false || time.ElapsedMilliseconds < 1000)
            {
                Application.OpenURL($"itms-apps://itunes.apple.com/app/id{CoreConfig.Instance.appleAppId}");
                yield return new WaitForSeconds(0.2f);
            }
            
            onComplete?.Invoke();
            RatedDispatch();
        }
    }
}
#endif