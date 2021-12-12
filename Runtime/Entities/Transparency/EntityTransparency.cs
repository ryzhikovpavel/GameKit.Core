using UnityEngine;
using UnityEngine.UI;

namespace GameKit.Entities.Transparency
{
    public class EntityTransparency: MonoBehaviour, ITransparency
    {
        private SpriteRenderer[] _renderers;
        private float[] _renderersInitialAlpha;
        private Graphic[] _graphics;
        private float[] _graphicsInitialAlpha;
        private float _alpha = 1;
        
        public float alpha
        {
            get => _alpha; 
            set => UpdateAlpha(_alpha = value);
        }

        private void Initialize()
        {
            _renderers = GetComponentsInChildren<SpriteRenderer>(true);
            int count = _renderers.Length;
            _renderersInitialAlpha = new float[count];
            for (int i = 0; i < count; i++)
                _renderersInitialAlpha[i] = _renderers[i].color.a;
            
            _graphics = GetComponentsInChildren<Graphic>(true);
            count = _graphics.Length;
            _graphicsInitialAlpha = new float[count];
            for (int i = 0; i < count; i++)
                _graphicsInitialAlpha[i] = _graphics[i].color.a;
        }
        
        private void Awake()
        {
            Initialize();
            UpdateAlpha(_alpha);
        }

        private void UpdateAlpha(float value)
        {
            int count = _renderers.Length;
            for (var i = 0; i < count; i++)
            {
                var r = _renderers[i];
                Color c = r.color;
                c.a = value * _renderersInitialAlpha[i];
                r.color = c;
            }

            count = _graphics.Length;
            for (var i = 0; i < count; i++)
            {
                var g = _graphics[i];
                Color c = g.color;
                c.a = value * _graphicsInitialAlpha[i];
                g.color = c;
            }
        }
    }
}