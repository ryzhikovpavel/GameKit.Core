#pragma warning disable 649
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameKit.Implementation
{
    [CreateAssetMenu(fileName = "LanguageName", menuName = "GameKit/Localization/Translation")]
    public class DataLanguageTranslation : ScriptableObject
    {
        [Serializable]
        public struct Translation
        {
            public string key;
            public string value;
        }

        public Sprite Icon => icon;
        public SystemLanguage Language => language;
        public SystemLanguage[] Aliases => aliases;
        public Translation[] Translations => translations;

        [SerializeField] private Sprite icon;
        [SerializeField] private SystemLanguage language;
        [SerializeField] private SystemLanguage[] aliases;
        [SerializeField] private Translation[] translations;
        [SerializeField] private Sprite[] sprites;

        [NonSerialized] private Dictionary<string, string> _dictionaryTranslations;
        [NonSerialized] private Dictionary<string, Sprite> _dictionarySprites;
        [NonSerialized] private bool _initialized;

        public bool rtl
        {
            get
            {
                switch (Language)
                {
                    case SystemLanguage.Arabic: return true;
                    case SystemLanguage.Hebrew: return true;
                    default: return false;
                }
            }
        }
        
        public bool Translate(string key, out string translation)
        {
            if (!_initialized) Initialize();
            return _dictionaryTranslations.TryGetValue(key, out translation);
        }

        public bool Translate(string key, out Sprite translation)
        {
            if (!_initialized) Initialize();
            return _dictionarySprites.TryGetValue(key, out translation);
        }

        public bool Contains(string key)
        {
            if (!_initialized) Initialize();
            return _dictionaryTranslations.ContainsKey(key) ||_dictionarySprites.ContainsKey(key);
        }
        
        public void Initialize()
        {
            if (_initialized) return;

            if (translations is null) _dictionaryTranslations = new Dictionary<string, string>();
            else
            {
                _dictionaryTranslations = new Dictionary<string, string>(translations.Length);
                foreach (var translation in translations)
                    _dictionaryTranslations.Add(translation.key, translation.value);
            }

            if (sprites is null) _dictionarySprites = new Dictionary<string, Sprite>();
            else
            {
                _dictionarySprites = new Dictionary<string, Sprite>(sprites.Length);
                foreach (var sprite in sprites)
                    _dictionarySprites.Add(sprite.name, sprite);
            }

            _initialized = true;
        }

#if UNITY_EDITOR
        // ReSharper disable once ParameterHidesMember
        public void Set(Dictionary<string, string> translations)
        {
            this.translations = new Translation[translations.Count];
            int index = 0;
            foreach (var translation in translations)
            {
                this.translations[index].key = translation.Key;
                this.translations[index].value = translation.Value;
                index++;
            }
        }

        // ReSharper disable once ParameterHidesMember
        public void Set(SystemLanguage language) => this.language = language;
#endif
    }
}