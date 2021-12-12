using UnityEngine;

namespace GameKit.Implementation
{
    internal class DefaultLocaleProvider : IPlatformLocaleProvider
    {
        public Locale GetPlatformLocale()
        {
            switch (Application.systemLanguage)
            {
                case SystemLanguage.Afrikaans:
                    return new Locale("af");
                case SystemLanguage.Arabic:
                    return new Locale("ar");
                case SystemLanguage.Basque:
                    return new Locale("eu");
                case SystemLanguage.Belarusian:
                    return new Locale("be");
                case SystemLanguage.Bulgarian:
                    return new Locale("bg");
                case SystemLanguage.Catalan:
                    return new Locale("ca");
                case SystemLanguage.Chinese:
                    return new Locale("zh");
                case SystemLanguage.Czech:
                    return new Locale("cs");
                case SystemLanguage.Danish:
                    return new Locale("da");
                case SystemLanguage.Dutch:
                    return new Locale("nl");
                case SystemLanguage.English:
                    return new Locale("en");
                case SystemLanguage.Estonian:
                    return new Locale("et");
                case SystemLanguage.Faroese:
                    return new Locale("fo");
                case SystemLanguage.Finnish:
                    return new Locale("fi");
                case SystemLanguage.French:
                    return new Locale("fr");
                case SystemLanguage.German:
                    return new Locale("de");
                case SystemLanguage.Greek:
                    return new Locale("el");
                case SystemLanguage.Hebrew:
                    return new Locale("he");
                case SystemLanguage.Icelandic:
                    return new Locale("is");
                case SystemLanguage.Indonesian:
                    return new Locale("id");
                case SystemLanguage.Italian:
                    return new Locale("it");
                case SystemLanguage.Japanese:
                    return new Locale("ja");
                case SystemLanguage.Korean:
                    return new Locale("ko");
                case SystemLanguage.Latvian:
                    return new Locale("lv");
                case SystemLanguage.Lithuanian:
                    return new Locale("lt");
                case SystemLanguage.Norwegian:
                    return new Locale("no");
                case SystemLanguage.Polish:
                    return new Locale("pl");
                case SystemLanguage.Portuguese:
                    return new Locale("pt");
                case SystemLanguage.Romanian:
                    return new Locale("ro");
                case SystemLanguage.Russian:
                    return new Locale("ru");
                case SystemLanguage.SerboCroatian:
                    return new Locale("sr");
                case SystemLanguage.Slovak:
                    return new Locale("sk");
                case SystemLanguage.Slovenian:
                    return new Locale("sl");
                case SystemLanguage.Spanish:
                    return new Locale("es");
                case SystemLanguage.Swedish:
                    return new Locale("sv");
                case SystemLanguage.Thai:
                    return new Locale("th");
                case SystemLanguage.Turkish:
                    return new Locale("tr");
                case SystemLanguage.Ukrainian:
                    return new Locale("ua");
                case SystemLanguage.Vietnamese:
                    return new Locale("vn");
                case SystemLanguage.ChineseSimplified:
                    return new Locale("zh");
                case SystemLanguage.ChineseTraditional:
                    return new Locale("zh");
                case SystemLanguage.Hungarian:
                    return new Locale("hu");
                case SystemLanguage.Unknown:
                    return new Locale("en");
                default:
                    return new Locale("en");
            }
        }
    }
}