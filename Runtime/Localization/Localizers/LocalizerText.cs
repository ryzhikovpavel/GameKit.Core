#pragma warning disable 649
using TMPro;
using UnityEngine;

namespace GameKit.Localizers
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class LocalizerText: Localizer
    {
        [SerializeField] private TMP_Text label;
        public bool upperFirstChar;

        private void Awake()
        {
            if (label is null) label = GetComponent<TMP_Text>();
        }
        
        protected override void OnLocalize()
        {
            Bind(Service<Localization>.Instance.Translate(Key));
        }
        
        public virtual void Bind(string text)
        {
            if (upperFirstChar) text = text.ToFirstUpper();
            label.text = text;
#if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                label.enabled = false;
                label.enabled = true;
            }
#endif
        }

        protected virtual void Reset()
        {
            label = GetComponent<TMP_Text>();
        }
    }
}