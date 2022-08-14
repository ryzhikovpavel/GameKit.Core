#pragma warning disable 649
using System;
using GameKit.Implementation;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace GameKit
{
    public delegate void LocalizeNotificationHandler();

    public class Localization
    {
        // ReSharper disable once InconsistentNaming
        private const string LANGUAGE_PREFS_KEY = "gamekit_localization_language";
        private readonly DataLocalization _config;

        public event LocalizeNotificationHandler EventLocalize = delegate {};
        public DataLanguageTranslation[] Translations => _config.Translations;
        public DataLanguageTranslation Translation { get; private set; }
        
        public Localization()
        {
            _config = Resources.Load<DataLocalization>("Localization");
            
            var currentLang = Application.systemLanguage;
            if (currentLang == SystemLanguage.Unknown) currentLang = SystemLanguage.English;
            Set(PlayerPrefs.GetString(LANGUAGE_PREFS_KEY, currentLang.ToString()));
        }

        public void Set(DataLanguageTranslation translation)
        {
            Set(translation.Language);
        }

        private void Set(string languageKey)
        {
            if (Enum.TryParse(languageKey, true, out SystemLanguage language))
                Set(language);
            else
            {
                if (_config.Default is null)
                {
                    Debug.LogError($"Default translation not binded");
                    return;
                }
                Set(_config.Default);
            }
        }

        public void Set(SystemLanguage language)
        {
            Translation = null;
            if (_config.Translations is null) return;
            foreach (var translation in _config.Translations)
            {
                if (translation.Language == language)
                {
                    Translation = translation;
                    break;
                }

                foreach (var alias in translation.Aliases)
                {
                    if (alias == language)
                    {
                        Translation = translation;
                        break;
                    }
                }
            }

            if (Translation is null)
            {
                Translation = _config.Default;

                if (Translation is null)
                {
                    Debug.LogError($"Default translation not binded");
                    return;
                }
            }

            Translation.Initialize();

            PlayerPrefs.SetString(LANGUAGE_PREFS_KEY, Translation.Language.ToString());
            PlayerPrefs.Save();
            EventLocalize();
        }

        public string Translate(string key)
        {
            if (Translation.Translate(key, out string translation)) return translation;
            Debug.LogWarning($"Translation for key [{key}] was not found, the value of the key itself is used");
            return key;
        }

        public string Translate(string key, params object[] args)
        {
            return string.Format(Translate(key), args);
        }

        public bool Translate(string key, out Sprite translation)
        {
            return Translation.Translate(key, out translation);
        }

        public bool Contains(string key) => Translation.Contains(key);
    }
}