using System.IO;
using UnityEditor;
using UnityEngine;

namespace GameKit.Editor.Tools
{
    public static class LanguageTool
    {
        [MenuItem("GameKit/Localization/Set Language/English")]
        private static void SetEnglish()
        {
            Service<Localization>.Instance.Set(SystemLanguage.English);
        }
        
        [MenuItem("GameKit/Localization/Set Language/Russian")]
        private static void SetRussian()
        {
            Service<Localization>.Instance.Set(SystemLanguage.Russian);
        }
    }
}