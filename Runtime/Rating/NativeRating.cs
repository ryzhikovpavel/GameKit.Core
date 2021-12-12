using System;
using System.Collections;
using System.Diagnostics;
using Google.Play.Common;
using Google.Play.Review;
using UnityEngine;
using Debug = UnityEngine.Debug;

#if UNITY_ANDROID
#endif

namespace GameKit
{
    public abstract class NativeRating
    {
        public static event Action EventRated;

        public static NativeRating Create()
        {
#if UNITY_EDITOR
            return new EditorPlatformReview();
#elif UNITY_ANDROID
            return new AndroidPlatformReview();
#elif UNITY_IOS
            return new IosPlatformReview();
#endif
            throw new NotSupportedException();
        }
        
        protected NativeRating(){}
        protected void RatedDispatch() => EventRated?.Invoke();
        public abstract void Open(Action onComplete);
        public abstract void Dispose();
    }
    
    public class EditorPlatformReview : NativeRating
    {
        public override void Open(Action onComplete)
        {
            Application.OpenURL("https://play.google.com/store/apps/details?id="+Application.identifier);
            onComplete?.Invoke();
            RatedDispatch();
        }
        public override void Dispose() { }
    }
    
#if UNITY_IOS
    public class IosPlatformReview : NativeRating
    {
        private IEnumerator _process;

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
#endif

#if UNITY_ANDROID
    public class AndroidPlatformReview: NativeRating
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
#endif
}