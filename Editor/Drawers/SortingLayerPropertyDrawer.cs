using System.Reflection;
using GameKit.Attributes;
using UnityEditor;
using UnityEngine;

namespace GameKit.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(SortingLayerAttribute))]
    public class SortingLayerPropertyDrawer : PropertyDrawer
    {
        private static MethodInfo drawer;
        static SortingLayerPropertyDrawer()
        {
            drawer = typeof(EditorGUI).GetMethod("SortingLayerField", BindingFlags.Static | BindingFlags.NonPublic,
                null,
                new[]
                {
                    typeof(Rect), typeof(GUIContent), typeof(SerializedProperty), typeof(GUIStyle), typeof(GUIStyle)
                }, null);
            
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Integer)
            {
                Debug.LogError("SortedLayer property should be an integer ( the layer id )");
            }
            else
            {
                SortingLayerField(position, new GUIContent("Sorting Layer"), property, EditorStyles.popup, EditorStyles.label);
            }
        }

        public static void SortingLayerField(Rect position, GUIContent label, SerializedProperty layerID, GUIStyle style, GUIStyle labelStyle)
        {
            if (drawer != null)
            {
                object[] parameters = new object[] {position, label, layerID, style, labelStyle };
                drawer.Invoke(null, parameters);
            }
        }       
    }
}