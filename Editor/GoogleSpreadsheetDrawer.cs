#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GameKit.Editor
{
    [CustomPropertyDrawer(typeof(GoogleSpreadsheet))]
    public class GoogleSpreadsheetDrawer : PropertyDrawer
    { 
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);
        
            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        
            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            const int buttonWidth = 50;
            // Calculate rects
            var contentRect = new Rect(position.x, position.y, position.width - buttonWidth, position.height);
            var buttonRect = new Rect(position.x + position.width - buttonWidth, position.y, buttonWidth, position.height);

            string tableValue = property.FindPropertyRelative("_table").stringValue;
            string gridValue = property.FindPropertyRelative("_grid").stringValue;
                
            if (string.IsNullOrEmpty(gridValue))
            {
                tableValue = EditorGUI.TextField(contentRect, tableValue);
                if (GUI.Button(buttonRect, "Save"))
                {
                    string table = null;
                    string grid = null;
                    if (string.IsNullOrEmpty(tableValue) == false)
                    {
                        tableValue = tableValue.Remove(0, GoogleSpreadsheet.Url.Length);
                        table = tableValue.Substring(0, tableValue.IndexOf("/"));
                        grid = tableValue.Substring(tableValue.IndexOf("gid=") + 4, tableValue.Length - tableValue.IndexOf("gid=") - 4);
    
                    }
                    property.FindPropertyRelative("_table").stringValue = table;
                    property.FindPropertyRelative("_grid").stringValue = grid;
                }
                else
                {
                    property.FindPropertyRelative("_table").stringValue = tableValue;
                }
            }
            else
            {
                EditorGUI.LabelField(contentRect,  "Gid: " + gridValue + " (..." + tableValue.Substring(tableValue.Length - 4, 4) + ")");
                if (GUI.Button(buttonRect, "Clear"))
                {
                    property.FindPropertyRelative("_table").stringValue = string.Empty;
                    property.FindPropertyRelative("_grid").stringValue = string.Empty;
                }
            }

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;
        
            EditorGUI.EndProperty();
        }
    }
}
#endif