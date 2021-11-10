#pragma warning disable 649
using UnityEngine;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;

namespace GameKit
{
    public sealed class WorldRectangle
    {
        public readonly float CameraOrthographicSize;
        
        public Rect Rect { get; private set; }
        public Rect SafeArea { get; private set; }
        public float Ratio { get; private set; }
        
        public WorldRectangle(float cameraOrthographicSize)
        {
            CameraOrthographicSize = cameraOrthographicSize;
            Calculate();
        }
        
        private void Calculate()
        {
            Ratio = (float)Screen.width / (float)Screen.height;

            var height = CameraOrthographicSize * 2;
            var width = height * Ratio;

            Rect = new Rect(-width / 2f, -height / 2f, width, height);

            SafeArea = Screen.safeArea; 
            
            var anchorMin = SafeArea.position;
            var anchorMax = SafeArea.position + SafeArea.size;
            
            anchorMin.x = Rect.xMin + Rect.width * anchorMin.x / Screen.width;
            anchorMax.x = Rect.xMax - Rect.width * (1 - anchorMax.x / Screen.width);
            anchorMax.y = Rect.yMax - Rect.height * (1 - anchorMax.y / Screen.height);
            anchorMin.y = Rect.yMin + Rect.height * anchorMin.y / Screen.height;

            SafeArea = Rect.MinMaxRect(anchorMin.x, anchorMin.y, anchorMax.x, anchorMax.y);
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