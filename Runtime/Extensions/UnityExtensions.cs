#pragma warning disable 649
using UnityEngine;
using System.Collections.Generic;

namespace GameKit
{
    public static class UnityExtensions
    {
        public static void SetActive(this IEnumerable<GameObject> objects, bool active)
        {
            foreach (var obj in objects)
            {
                if (obj != null)
                    obj.SetActive(active);
            }
        }

        public static void SetActive<T>(this IEnumerable<T> objects, bool active) where T: Component
        {
            foreach (var obj in objects)
            {
                if (obj != null)
                    obj.SetActive(active);
            }
        }

        public static void SetActive<T>(this T obj, bool active) where T: Component
        {
            obj.gameObject.SetActive(active);
        }

        public static bool GetActive<T>(this T obj) where T: Component
        {
            return obj.gameObject.activeSelf;
        }

        public static void ShowObject(this GameObject gameObject)
        {
            if (gameObject is null) return;
            gameObject.SetActive(true);
        }

        public static void ShowObject<T>(this T component) where T: Component
        {
            if (component is null) return;
            component.gameObject.SetActive(true);
        }

        public static void HideObject(this GameObject gameObject)
        {
            if (gameObject is null) return;
            gameObject.SetActive(false);
        }

        public static void HideObject<T>(this T component) where T : Component
        {
            if (component is null) return;
            component.gameObject.SetActive(false);
        }

        public static void DestroyAllChildren(this Transform obj)
        {
            for (int i = obj.childCount - 1; i >= 0; i--)
            {
                var item = obj.GetChild(i);

                if (item != null)
                {
                    item.parent = null;
                    UnityEngine.Object.Destroy(item.gameObject);
                }
            }
        }
    }
}