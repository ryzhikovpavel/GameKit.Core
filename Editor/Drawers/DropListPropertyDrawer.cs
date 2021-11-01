using System;
using UnityEditor;
using UnityEngine;

namespace GameKit.Editor.Drawers
{
    public abstract class DropListPropertyDrawer : PropertyDrawer
    {
        private bool needInit = true;        
        private GUIContent[] items;
        private int index;
        private int offset;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (needInit)
            {
                GetItems(out var list, out var needEmpty);

                offset = needEmpty ? 1 : 0;
                if (list == null)
                {
                    items = new GUIContent[1];
                    items[0] = new GUIContent("--EMPTY--");
                }
                else
                {
                    if (needEmpty)
                        items = new GUIContent[list.Length + 1];
                    else
                        items = new GUIContent[list.Length];
                    
                    for (int i = 0; i < list.Length; i++)
                    {
                        items[i+offset] = new GUIContent(list[i]);
                    }
                }
                
                index = 0;
                string val = property.stringValue;
                for (int i = 0; i < items.Length; i++)
                {
                    if (items[i].text == val)
                    {
                        index = i;
                        break;
                    }
                }

                needInit = false;
            }

            index = EditorGUI.Popup(position, label, index, items);
            if (index == offset - 1) property.stringValue = "";
            else property.stringValue = items[index].text;
        }

        protected abstract void GetItems(out string[] items, out bool needEmpty);
    }
}