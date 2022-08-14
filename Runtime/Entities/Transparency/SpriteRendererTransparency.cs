using System;
using UnityEngine;

namespace GameKit.Entities.Transparency
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteRendererTransparency: EntityTransparency, ITransparency
    {
        private SpriteRenderer _renderer;
        private float _initialAlpha;
        
        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
        }

        protected override void SetAlpha(float value)
        {
            Color c = _renderer.color;
            c.a = value * _initialAlpha;
            _renderer.color = c;
        }
    }
}