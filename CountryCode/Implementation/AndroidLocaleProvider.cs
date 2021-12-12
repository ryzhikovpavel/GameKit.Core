#if UNITY_ANDROID
using UnityEngine;

namespace GameKit.Core.CountryCode.Implementation
{
    public class AndroidPlatformLocaleProvider : IPlatformLocaleProvider
    {
        public Locale GetCountryCode()
        {
            return ReadCountryCodeFromLocale();
        }

        Locale ReadCountryCodeFromLocale()
        {
            using (var localeObject = new AndroidJavaClass("java.util.Locale"))
            {
                using(var localeInstance = localeObject.CallStatic<AndroidJavaObject>("getDefault"))
                {
                    if (localeInstance != null)
                    {
                        var language = localeInstance.Call<string>("getLanguage");
                        var country = localeInstance.Call<string>("getCountry");
                        return new Locale(language, country);
                    }
                }
            }

            return null;
        }
    }
}
#endif