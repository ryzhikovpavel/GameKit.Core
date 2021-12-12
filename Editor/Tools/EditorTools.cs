using UnityEditor;
using UnityEngine;

namespace GameKit.Editor.Tools
{
    public static class EditorTools
    {
        public const bool MinimalisticLook = false;

        static public void SetLabelWidth(float width)
        {
            EditorGUIUtility.labelWidth = width;
        }

        /// <summary>
        /// Helper function that draws a serialized property.
        /// </summary>

        static public SerializedProperty DrawProperty(SerializedObject serializedObject, string property,
            params GUILayoutOption[] options)
        {
            return DrawProperty(null, serializedObject, property, false, options);
        }

        /// <summary>
        /// Helper function that draws a serialized property.
        /// </summary>

        static public SerializedProperty DrawProperty(string label, SerializedObject serializedObject, string property,
            params GUILayoutOption[] options)
        {
            return DrawProperty(label, serializedObject, property, false, options);
        }

        /// <summary>
        /// Helper function that draws a serialized property.
        /// </summary>

        static public SerializedProperty DrawPaddedProperty(SerializedObject serializedObject, string property,
            params GUILayoutOption[] options)
        {
            return DrawProperty(null, serializedObject, property, true, options);
        }

        /// <summary>
        /// Helper function that draws a serialized property.
        /// </summary>

        static public SerializedProperty DrawPaddedProperty(string label, SerializedObject serializedObject,
            string property, params GUILayoutOption[] options)
        {
            return DrawProperty(label, serializedObject, property, true, options);
        }

        /// <summary>
        /// Helper function that draws a serialized property.
        /// </summary>

        static public SerializedProperty DrawProperty(string label, SerializedObject serializedObject, string property,
            bool padding, params GUILayoutOption[] options)
        {
            SerializedProperty sp = serializedObject.FindProperty(property);

            if (sp != null)
            {
                //if (NGUISettings.minimalisticLook) padding = false;

                if (padding) EditorGUILayout.BeginHorizontal();

                if (label != null) EditorGUILayout.PropertyField(sp, new GUIContent(label), options);
                else EditorGUILayout.PropertyField(sp, options);

                if (padding)
                {
                    DrawPadding();
                    EditorGUILayout.EndHorizontal();
                }
            }

            return sp;
        }

        /// <summary>
        /// Helper function that draws a serialized property.
        /// </summary>

        static public void DrawProperty(string label, SerializedProperty sp, params GUILayoutOption[] options)
        {
            DrawProperty(label, sp, true, options);
        }

        /// <summary>
        /// Helper function that draws a serialized property.
        /// </summary>

        static public void DrawProperty(string label, SerializedProperty sp, bool padding,
            params GUILayoutOption[] options)
        {
            if (sp != null)
            {
                if (padding) EditorGUILayout.BeginHorizontal();

                if (label != null) EditorGUILayout.PropertyField(sp, new GUIContent(label), options);
                else EditorGUILayout.PropertyField(sp, options);

                if (padding)
                {
                    DrawPadding();
                    EditorGUILayout.EndHorizontal();
                }
            }
        }

        static public int DrawPopupProperty(int index, string[] items, GUIContent label, SerializedObject serializedObject, string property,
            bool padding, bool firstEmptyValue, params GUILayoutOption[] options)
        {
            SerializedProperty sp = serializedObject.FindProperty(property);     
            if (sp != null) return DrawPopupProperty(index, items, label, sp, padding, firstEmptyValue, options);
            return index;
        }

        static public int DrawPopupProperty(int index, string[] items, GUIContent label, SerializedProperty sp, 
            bool padding, bool firstEmptyValue, params GUILayoutOption[] options)
        {
            if (sp != null)
            {
                if (padding) EditorGUILayout.BeginHorizontal();

                if (index >= items.Length && index < 0) index = 0;
                
                if (label != null) index = EditorGUILayout.Popup(label, index, items, options);
                else index = EditorGUILayout.Popup(index, items, options);

                if (sp.propertyType == SerializedPropertyType.String)
                {
                    if (firstEmptyValue && index == 0) sp.stringValue = "";
                    else sp.stringValue = items[index];
                }

                if (padding)
                {
                    DrawPadding();
                    EditorGUILayout.EndHorizontal();
                }
            }

            return index;
        }

        /// <summary>
        /// Helper function that draws a compact Vector4.
        /// </summary>

        static public void DrawBorderProperty(string name, SerializedObject serializedObject, string field)
        {
            if (serializedObject.FindProperty(field) != null)
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label(name, GUILayout.Width(75f));

                    SetLabelWidth(50f);
                    GUILayout.BeginVertical();
                    DrawProperty("Left", serializedObject, field + ".x", GUILayout.MinWidth(80f));
                    DrawProperty("Bottom", serializedObject, field + ".y", GUILayout.MinWidth(80f));
                    GUILayout.EndVertical();

                    GUILayout.BeginVertical();
                    DrawProperty("Right", serializedObject, field + ".z", GUILayout.MinWidth(80f));
                    DrawProperty("Top", serializedObject, field + ".w", GUILayout.MinWidth(80f));
                    GUILayout.EndVertical();

                    SetLabelWidth(80f);
                }
                GUILayout.EndHorizontal();
            }
        }

        static public void DrawPadding()
        {
            if (!MinimalisticLook)
                GUILayout.Space(18f);
        }

        /// <summary>
        /// Begin drawing the content area.
        /// </summary>

        static public void BeginContents()
        {
            BeginContents(false);
        }

        static bool mEndHorizontal = false;

        /// <summary>
        /// Begin drawing the content area.
        /// </summary>

        static public void BeginContents(bool minimalistic)
        {
            if (!minimalistic)
            {
                mEndHorizontal = true;
                GUILayout.BeginHorizontal();
                //EditorGUILayout.BeginHorizontal("AS TextArea", GUILayout.MinHeight(10f));
                EditorGUILayout.BeginHorizontal("TextArea", GUILayout.MinHeight(10f));
            }
            else
            {
                mEndHorizontal = false;
                EditorGUILayout.BeginHorizontal(GUILayout.MinHeight(10f));
                GUILayout.Space(10f);
            }

            GUILayout.BeginVertical();
            GUILayout.Space(2f);
        }

        /// <summary>
        /// End drawing the content area.
        /// </summary>

        static public void EndContents()
        {
            GUILayout.Space(3f);
            GUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            if (mEndHorizontal)
            {
                GUILayout.Space(3f);
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(3f);
        }

        static public bool DrawHeader(string text)
        {
            return DrawHeader(text, text, false, MinimalisticLook);
        }

        /// <summary>
        /// Draw a distinctly different looking header label
        /// </summary>

        static public bool DrawHeader(string text, string key)
        {
            return DrawHeader(text, key, false, MinimalisticLook);
        }

        /// <summary>
        /// Draw a distinctly different looking header label
        /// </summary>

        static public bool DrawHeader(string text, bool detailed)
        {
            return DrawHeader(text, text, detailed, !detailed);
        }

        /// <summary>
        /// Draw a distinctly different looking header label
        /// </summary>

        static public bool DrawHeader(string text, string key, bool forceOn, bool minimalistic)
        {
            bool state = EditorPrefs.GetBool(key, true);

            if (!minimalistic) GUILayout.Space(3f);
            if (!forceOn && !state) GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);
            GUILayout.BeginHorizontal();
            GUI.changed = false;

            if (minimalistic)
            {
                if (state) text = "\u25BC" + (char)0x200a + text;
                else text = "\u25BA" + (char)0x200a + text;

                GUILayout.BeginHorizontal();
                GUI.contentColor = EditorGUIUtility.isProSkin
                    ? new Color(1f, 1f, 1f, 0.7f)
                    : new Color(0f, 0f, 0f, 0.7f);
                if (!GUILayout.Toggle(true, text, "PreToolbar2", GUILayout.MinWidth(20f))) state = !state;
                GUI.contentColor = Color.white;
                GUILayout.EndHorizontal();
            }
            else
            {
                text = "<b><size=11>" + text + "</size></b>";
                if (state) text = "\u25BC " + text;
                else text = "\u25BA " + text;
                if (!GUILayout.Toggle(true, text, "dragtab", GUILayout.MinWidth(20f))) state = !state;
            }

            if (GUI.changed) EditorPrefs.SetBool(key, state);

            if (!minimalistic) GUILayout.Space(2f);
            GUILayout.EndHorizontal();
            GUI.backgroundColor = Color.white;
            if (!forceOn && !state) GUILayout.Space(3f);
            return state;
        }
    }
}