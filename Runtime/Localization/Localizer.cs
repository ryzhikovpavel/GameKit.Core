using UnityEngine;

namespace GameKit
{
    // ReSharper disable once IdentifierTypo
    public abstract class Localizer : MonoBehaviour
    {
        [SerializeField, HideInInspector] private string key;
        protected string Key => key;

        private void OnEnable()
        {
            if (enabled == false) return;
            Service<Localization>.Instance.OnLocalize += OnLocalize;
            OnLocalize();
        }

        private void OnDisable()
        {
            Service<Localization>.Instance.OnLocalize -= OnLocalize;
        }

        protected abstract void OnLocalize();
    }
}