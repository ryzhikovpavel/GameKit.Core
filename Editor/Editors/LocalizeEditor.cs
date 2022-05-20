using GameKit.Editor.Tools;
using UnityEditor;
using UnityEngine;

namespace GameKit.Editor.Editors
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(LocalizerText), true)]
    public class LocalizeEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();

            GUILayout.Space(6f);
            EditorGUIUtility.labelWidth = 80f;

            //((LocalizerText)target).format = (Localize.Format)EditorGUILayout.EnumPopup("Format", ((Localize)target).format);

            // SerializedProperty iterator = serializedObject.GetIterator();
            // for (bool enterChildren = true; iterator.NextVisible(enterChildren); enterChildren = false)
            // {
            //     using (new EditorGUI.DisabledScope("m_Script" == iterator.propertyPath))
            //         EditorGUILayout.PropertyField(iterator, true);
            // }

            DrawDefaultInspector ();
            
            GUILayout.BeginHorizontal();            

            // Key not found in the localization file -- draw it as a text field            
            SerializedProperty sp = EditorTools.DrawProperty("Key", serializedObject, "key");

            string myKey = sp.stringValue;
            bool isPresent = Service<Localization>.Instance.Contains(myKey);
            GUI.color = isPresent ? Color.green : Color.red;
            GUILayout.BeginVertical(GUILayout.Width(22f));
            GUILayout.Space(2f);
            //GUILayout.Label(isPresent ? "\u2714" : "\u2718", "TL SelectionButtonNew", GUILayout.Height(20f));
            GUILayout.Label(isPresent ? "\u2714" : "\u2718", GUILayout.Height(20f));
            GUILayout.EndVertical();
            GUI.color = Color.white;
            GUILayout.EndHorizontal();

            if (isPresent)
            {
                if (EditorTools.DrawHeader("Preview"))
                {
                    EditorTools.BeginContents();

                    //if (Localization.dictionary.TryGetValue(myKey, out values))
                    foreach (var lang in Service<Localization>.Instance.Translations)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(lang.Language.ToString(), GUILayout.Width(87f));
                        
                        lang.Translate(myKey, out string translation);
                        if (GUILayout.Button(translation, "textArea", GUILayout.MinWidth(80f), GUILayout.MaxWidth(Screen.width - 110f)))
                        {
                            (target as LocalizerText)?.Bind(translation, lang.rtl);
                            GUIUtility.hotControl = 0;
                            GUIUtility.keyboardControl = 0;
                        }

                        GUILayout.EndHorizontal();
                    }

                    EditorTools.EndContents();
                }
            }
            else if (Service<Localization>.Instance.Translation is null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(80f);
                GUILayout.BeginVertical();
                GUI.backgroundColor = new Color(1f, 1f, 1f, 0.35f);
            
                //int matches = 0;
                // for (int i = 0, imax = mKeys.Count; i < imax; ++i)
                // {
                //     if (mKeys[i].StartsWith(myKey, System.StringComparison.OrdinalIgnoreCase) ||
                //         mKeys[i].Contains(myKey))
                //     {
                //         if (GUILayout.Button(mKeys[i] + " \u25B2", "CN CountBadge"))
                //         {
                //             sp.stringValue = mKeys[i];
                //             GUIUtility.hotControl = 0;
                //             GUIUtility.keyboardControl = 0;
                //         }
                //
                //         if (++matches == 8)
                //         {
                //             GUILayout.Label("...and more");
                //             break;
                //         }
                //     }
                // }
            
                GUI.backgroundColor = Color.white;
                GUILayout.EndVertical();
                GUILayout.Space(22f);
                GUILayout.EndHorizontal();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}