#if UNITY_ANDROID && !UNITY_EDITOR
using PlatformLocaleProvider = GameKit.Implementation.AndroidLocaleProvider;
#elif UNITY_IPHONE && !UNITY_EDITOR
using PlatformLocaleProvider = GameKit.Implementation.IOSLocaleProvider;
#else
using PlatformLocaleProvider = GameKit.Implementation.DefaultLocaleProvider;
#endif
using GameKit.Implementation;

namespace GameKit
{
    public class Locale
    {
        private static readonly IPlatformLocaleProvider Provider = new PlatformLocaleProvider();
        
        public static Locale Default => Provider.GetPlatformLocale();
        
        /// <summary>
        /// System language code (ISO 639-1)
        /// </summary>
        public readonly string Language;

        /// <summary>
        /// System set country code (ISO 3166-1)
        /// </summary>
        public readonly string Country;
        
        public Locale(string language, string country)
        {
            Language = language.ToLower();
            Country = country.ToLower();
        }

        public Locale(string countryCode)
        {
            // ReSharper disable once PossiblyMistakenUseOfParamsMethod
            var result = countryCode.Split('-', '_');

            switch ( result.Length )
            {
                case 1:
                    Language = result[0].ToLower();
                    break;
                case 2:
                case 3:
                    Language = result[0].ToLower();
                    Country = result[1].ToLower();
                    break;
                default:
                    Language = "en";
                    Country = string.Empty;
                    break;
            }
        }

        /// <summary>
        /// Gets formatted country code.
        /// </summary>
        /// <returns>Country code formatted as $"{<see cref="Language"/>}-{<see cref="Country"/>}".</returns>
        public string GetCountryCode()
        {
            return $"{Language}-{Country}";
        }

        /// <summary>
        /// Converts localization details to a dash-separated format.
        /// </summary>
        /// <returns>Country code formatted as $"{<see cref="Language"/>}-{<see cref="Country"/>}".</returns>
        public override string ToString()
        {
            return GetCountryCode().ToLower().TrimEnd( '-' );
        }
    }
}