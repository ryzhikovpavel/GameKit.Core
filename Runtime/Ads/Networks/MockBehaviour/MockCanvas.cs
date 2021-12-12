using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameKit.Ads.Networks.MockBehaviour
{
    public class MockCanvas : MonoBehaviour
    {
        [SerializeField] private RectTransform anchoredRect;
        [SerializeField] private Button buttonSuccess;
        [SerializeField] private Button buttonFailed;
        [SerializeField] private Button buttonCanceled;
        [SerializeField] private Button buttonFullscreen;
        [SerializeField] private Button buttonAnchored;
        [SerializeField] private Text anchoredBannerLabel;

        private int _anchoredBannerIndex;
        
        public event Action EventSuccess;
        public event Action EventFailed;
        public event Action EventCanceled;
        public event Action EventFullscreenClicked;
        public event Action EventAnchoredClicked;
        
        private void Awake()
        {
            buttonSuccess.onClick.AddListener(()=>EventSuccess?.Invoke());
            buttonFailed.onClick.AddListener(()=>EventFailed?.Invoke());
            buttonCanceled.onClick.AddListener(()=>EventCanceled?.Invoke());
            buttonFullscreen.onClick.AddListener(()=>EventFullscreenClicked?.Invoke());
            buttonAnchored.onClick.AddListener(()=>EventAnchoredClicked?.Invoke());
            
            buttonFullscreen.HideObject();
            buttonAnchored.HideObject();
            
            DontDestroyOnLoad(gameObject);
        }

        public void ShowInterstitial()
        {
            buttonFullscreen.ShowObject();
            buttonCanceled.HideObject();
        }

        public void ShowRewardedVideo()
        {
            buttonFullscreen.ShowObject();
            buttonCanceled.ShowObject();
        }

        public void ShowAnchored(AdAnchor anchor)
        {
            var aMin = anchoredRect.anchorMin;
            var aMax = anchoredRect.anchorMax;
            var pivot = anchoredRect.pivot;
            
            if (anchor == AdAnchor.Top)
            {
                aMin.y = 1;
                aMax.y = 1;
                pivot.y = 1;
            }
            else
            {
                aMin.y = 0;
                aMax.y = 0;
                pivot.y = 0;
            }

            anchoredRect.anchorMin = aMin;
            anchoredRect.anchorMax = aMax;
            anchoredRect.pivot = pivot;
            anchoredRect.anchoredPosition = Vector2.zero;
            anchoredBannerLabel.text = (_anchoredBannerIndex++).ToString(); 
            
            buttonAnchored.ShowObject();
        }
        
        public void HideAnchored()
        {
            buttonAnchored.HideObject();
        }

        public void HideFullscreen()
        {
            buttonFullscreen.HideObject();
        }
    }
}