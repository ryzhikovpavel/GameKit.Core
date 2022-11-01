#if UNITY_ANDROID && !GooglePlayReview
using System;
using UnityEngine;

namespace GameKit.Rating.Implementation
{
    internal class AndroidPlatformReview : NativeRating
    {
        public override void Open(Action onComplete)
        {
            Application.OpenURL("market://details?id="+Application.identifier);
            onComplete?.Invoke();
            RatedDispatch();
        }

        public override void Dispose() { }
    }
}
#endif

#if UNITY_ANDROID && GooglePlayReview
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Google.Play.Common;
using Google.Play.Review;

namespace GameKit.Rating.Implementation
{

    internal class AndroidPlatformReview: NativeRating
    {
        private IEnumerator _process;
        private ReviewManager _reviewManager;
        private PlayReviewInfo _playReviewInfo;
        
        protected internal AndroidPlatformReview()
        {
            _reviewManager = new ReviewManager();
            Loop.StartCoroutine(_process = Initialize());
        }

        public override void Open(Action onComplete)
        {
            Loop.StartCoroutine(_process = Show(onComplete));
        }

        private IEnumerator Initialize()
        {
            var requestFlowOperation = _reviewManager.RequestReviewFlow();
            yield return requestFlowOperation;
            
            if (requestFlowOperation.Error != ReviewErrorCode.NoError)
            {
                Debug.LogError(requestFlowOperation.Error.ToString());
                yield break;
            }
            
            _playReviewInfo = requestFlowOperation.GetResult();
            _process = null;
            IsReady = true;
        }

        private IEnumerator Show(Action onComplete)
        {
            PlayAsyncOperation<VoidResult, ReviewErrorCode> launchFlowOperation = null;
            bool isNativeDialog = false;
            if (_playReviewInfo != null)
            {
                launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);
                var time = Stopwatch.StartNew();
                yield return launchFlowOperation;
                time.Stop();
                isNativeDialog = launchFlowOperation.Error == ReviewErrorCode.NoError;
                if (time.ElapsedMilliseconds < 1000) isNativeDialog = false;
            }

            if (isNativeDialog == false)
            {
                if (launchFlowOperation != null && launchFlowOperation.Error != ReviewErrorCode.NoError)
                    Debug.LogError(launchFlowOperation.Error.ToString());
                Application.OpenURL("market://details?id="+Application.identifier);
                yield return new WaitForSeconds(0.2f);
            }

            _process = null;
            onComplete?.Invoke();
            RatedDispatch();
        }

        public override void Dispose()
        {
            if (_process != null)
            {
                Loop.StopCoroutine(_process);
                _process = null;
            }

            _reviewManager = null;
            _playReviewInfo = null;
        }
    }
}
#endif