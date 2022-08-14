using UnityEngine;
using UnityEngine.UI;

namespace GameKit.Entities.Transparency
{
    public abstract class EntityTransparency: MonoBehaviour, ITransparency
    {
        private float _alpha = 1;
        
        public float alpha
        {
            get => _alpha; 
            set => SetAlpha(_alpha = value);
        }
        
        protected abstract void SetAlpha(float value);
    }
}