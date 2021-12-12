#pragma warning disable 649
using GameKit.Csv;
using UnityEngine;

namespace GameKit.Implementation
{
    [CreateAssetMenu(fileName = "Localization", menuName = "GameKit/Localization/Config")]
    public class DataLocalization : ScriptableObject
    {
        public DataLanguageTranslation Default => @default is null ? ((translations is null || translations.Length == 0) ? null : translations[0]) : @default;
        public DataLanguageTranslation[] Translations => translations;

#if UNITY_EDITOR
        public GoogleSpreadsheet[] Spreadsheets => spreadsheets;

        public void AddTranslation(DataLanguageTranslation translation)
        {
            if (translations.IndexOf( translation ) >= 0 ) return;
            ArrayTools.Add(ref translations, translation);
        }
#endif

        [SerializeField] private DataLanguageTranslation @default;
        [SerializeField] private DataLanguageTranslation[] translations;
        [SerializeField] private GoogleSpreadsheet[] spreadsheets;
    }
}