using System;
using System.Collections.Generic;
using GameKit.Csv;
using GameKit.Editor.Tools;
using GameKit.Implementation;
using UnityEditor;
using UnityEngine;

namespace GameKit.Editor.Editors
{
    [CustomEditor(typeof(DataLocalization), true)]
    public class DataLocalizationEditor : UnityEditor.Editor
    {
        private int _curLang = 0;
        private string[] _languages;

        void OnEnable()
        {
            _languages = new string[Service<Localization>.Instance.Translations.Length];
            _curLang = -1;
            for (int i = 0; i < _languages.Length; i++)
            {
                var t = Service<Localization>.Instance.Translations[i];
                _languages[i] = t.Language.ToString();

                if (Service<Localization>.Instance.Translation == t) _curLang = i;
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();

            if (Application.isPlaying && _languages != null && _languages.Length > 0)
            {
                //GUILayout.Label("Language", EditorStyles.objectFieldThumb);
                GUILayout.BeginVertical(EditorStyles.textArea);
                GUILayout.Space(2);

                if (_curLang >= 0)
                {
                    var newLang = EditorGUILayout.Popup("Current", _curLang, _languages);
                    if (newLang != _curLang)
                    {
                        _curLang = newLang;
                        Service<Localization>.Instance.Set(Service<Localization>.Instance.Translations[_curLang]);
                    }
                }

                GUILayout.Space(2);
                GUILayout.EndVertical();
            }

            DrawDefaultInspector();
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}