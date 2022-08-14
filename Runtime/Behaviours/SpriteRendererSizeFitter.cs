using System;
using UnityEngine;

namespace GameKit
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteRendererSizeFitter : MonoBehaviour
    {
        private enum Type
        {
            Width,
            Height,
            Minimum,
            Maximum
        }

        [SerializeField] private Type type;
        private SpriteRenderer _renderer;
        private WorldScreenRectangle _worldScreen;
        private bool _ready;

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
        }
        
        private void OnEnable()
        {
#if UNITY_EDITOR
            if (Screen.width == 0 || Screen.height == 0) return;
            _width = Screen.width;
            _height = Screen.height;
#endif
           Fit();
        }

#if UNITY_EDITOR
        private int _width;
        private int _height;
        private void Update()
        {
            if (_width != Screen.width || _height != Screen.height)
            {
                _width = Screen.width;
                _height = Screen.height;
                Fit();
            }
        }
#endif

        private void Fit()
        {
            var mainCamera = Camera.main;
            if (mainCamera is null)
            {
                Debug.LogError("Camera not found");
                return;
            }
            
            _worldScreen = new WorldScreenRectangle(mainCamera.orthographicSize);
            
            var sprite = _renderer.sprite;
            var rect = sprite.rect;
            var ppu = sprite.pixelsPerUnit;
            
            float wScale = _worldScreen.Rect.width / (rect.width / ppu);
            float hScale = _worldScreen.Rect.height / (rect.height / ppu);
            float scale;

            switch (type)
            {
                case Type.Width:
                    scale = wScale;
                    break;
                case Type.Height:
                    scale = hScale;
                    break;
                case Type.Minimum:
                    scale = Mathf.Min(wScale, hScale);
                    break;
                case Type.Maximum:
                    scale = Mathf.Max(wScale, hScale);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            transform.localScale = new Vector3(scale, scale);
        }
    }
}