using GameKit.Ads.Units;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameKit.Ads.Networks.LocalNetwork
{
    public abstract class LocalAdUnit: MonoBehaviour, IAdUnit, IPointerClickHandler
    {
        [SerializeField] private RawImage bannerImage;
        private string _url;
        
        public AdUnitState State { get; protected set; }
        
        public string Error { get; private set; }

        public IAdInfo Info { get; } = new DefaultAdInfo();

        public void Show()
        {
            State = AdUnitState.Displayed;
            gameObject.SetActive(true);
        }

        public void Release()
        {
            State = AdUnitState.Empty;
            gameObject.SetActive(false);
        }

        public void Setup(Texture texture, string url)
        {
            bannerImage.texture = texture;
            _url = url;
            State = AdUnitState.Closed;
            gameObject.SetActive(false);
        }
        
        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            State = AdUnitState.Clicked;
            Application.OpenURL(_url);
        }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}