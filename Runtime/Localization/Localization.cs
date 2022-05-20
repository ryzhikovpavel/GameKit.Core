#pragma warning disable 649
using System;
using GameKit.Implementation;
using UnityEngine;

namespace GameKit
{
    public delegate void OnLocalizeNotification();

    public class Localization
    {
        // ReSharper disable once InconsistentNaming
        private const string LANGUAGE_PREFS_KEY = "gamekit_localization_language";

        public event OnLocalizeNotification OnLocalize = delegate {};

        public DataLanguageTranslation[] Translations => _config.Translations;
        public DataLanguageTranslation Translation { get; private set; }

        private DataLocalization _config;

        public Localization()
        {
            _config = Resources.Load<DataLocalization>("Localization");
            
            var currentLang = Application.systemLanguage;
            if (currentLang == SystemLanguage.Unknown) currentLang = SystemLanguage.English;
            Debug.Log($"last key: {PlayerPrefs.GetString("application_localization_language")}");
            Debug.Log($"current: {currentLang}, saved: {PlayerPrefs.GetString(LANGUAGE_PREFS_KEY)}");
            Set(PlayerPrefs.GetString(LANGUAGE_PREFS_KEY, currentLang.ToString()));
        }

        public void Set(DataLanguageTranslation translation)
        {
            Set(translation.Language);
        }

        private void Set(string languageKey)
        {
            if (Enum.TryParse(languageKey, true, out SystemLanguage language)) Set(language);
            else
            {
                Debug.LogError("Language " + languageKey + " not found, set default " + _config.Default.Language);
                Set(_config.Default);
            }
        }

        public void Set(SystemLanguage language)
        {
            Translation = null;
            foreach (var translation in _config.Translations)
            {
                if (translation.Language == language)
                {
                    Translation = translation;
                    //Debug.Log("Set current language: " + language);
                    break;
                }

                foreach (var alias in translation.Aliases)
                {
                    if (alias == language)
                    {
                        Translation = translation;
                        //Debug.Log("Set current language: " + language + " as " + Translation.Language);
                        break;
                    }
                }
            }

            if (Translation is null)
            {
                Translation = _config.Default;

                if (Translation is null)
                {
                    throw new IndexOutOfRangeException();
                }
                
                Debug.Log("Language " + language + " not found, set default " + _config.Default.Language);
            }

            Translation.Initialize();

            Debug.Log($"SET LANGUAGE: {language}; Translation: {Translation.Language.ToString()}");
            PlayerPrefs.SetString(LANGUAGE_PREFS_KEY, Translation.Language.ToString());
            PlayerPrefs.Save();
            OnLocalize();
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