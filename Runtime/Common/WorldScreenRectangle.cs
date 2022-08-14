#pragma warning disable 649
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameKit
{
    public sealed class WorldScreenRectangle
    {
        private Rect _worldRect;
        private float _ratio;
        
        public readonly float CameraOrthographicSize;

        public Rect Rect => _worldRect;
        public Rect SafeArea => CalculateSafeArea();
        public float Ratio => _ratio;
        
        public WorldScreenRectangle(float cameraOrthographicSize)
        {
            CameraOrthographicSize = cameraOrthographicSize;
            Initialize();
        }

        private void Initialize()
        {
            Calculate();
        }

        private void Calculate()
        {
            _ratio = (float)Screen.width / (float)Screen.height;

            var height = CameraOrthographicSize * 2;
            var width = height * _ratio;

            _worldRect = new Rect(-width / 2f, -height / 2f, width, height);
        }

        private Rect CalculateSafeArea()
        {
            var screenSafeArea = Screen.safeArea;
            var anchorMin = screenSafeArea.position;
            var anchorMax = screenSafeArea.position + screenSafeArea.size;
            
            anchorMin.x = _worldRect.xMin + _worldRect.width * anchorMin.x / Screen.width;
            anchorMax.x = _worldRect.xMax - _worldRect.width * (1 - anchorMax.x / Screen.width);
            anchorMax.y = _worldRect.yMax - _worldRect.height * (1 - anchorMax.y / Screen.height);
            anchorMin.y = _worldRect.yMin + _worldRect.height * anchorMin.y / Screen.height;
            
            return Rect.MinMaxRect(anchorMin.x, anchorMin.y, anchorMax.x, anchorMax.y);
        }

        public float GetRandomX(float offset)
        {
            return Random.Range(Rect.xMin + offset, Rect.xMax - offset);
        }

        public float GetRandomY(float offset)
        {
            return Random.Range(Rect.yMin + offset, Rect.yMax - offset);
        }
    }
}