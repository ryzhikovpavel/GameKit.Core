#if UNITY_ANDROID && !UNITY_EDITOR
using PlatformCountryCodeProvider = HUFEXT.CountryCode.Runtime.Implementation.AndroidCountryCodeProvider;
#elif UNITY_IPHONE && !UNITY_EDITOR
using PlatformCountryCodeProvider = HUFEXT.CountryCode.Runtime.Implementation.iOSCountryCodeProvider;
#else
using PlatformCountryCodeProvider = GameKit.Core.CountryCode.Implementation.DefaultCountryCodeProvider;
#endif
using GameKit.Core.CountryCode.Implementation;
using JetBrains.Annotations;

namespace GameKit.Core.CountryCode
{
    public class CountryCode
    {
        private static readonly ICountryCodeProvider Provider = new PlatformCountryCodeProvider();
        
        public static CountryCode System => Provider.GetCountryCode();
        
        /// <summary>
        /// System language code (ISO 639-1)
        /// </summary>
        [PublicAPI]
        public readonly string Language;

        /// <summary>
        /// System set country code (ISO 3166-1)
        /// </summary>
        [PublicAPI]
        public readonly string Country;

        /// <summary>
        /// iOS-only region descriptor.
        /// </summary>
        /// <example>
        /// <para>Assume iOS returns zh-Hans_HK as the used language.</para>
        /// <para>It will be split to: Language: zh, Country: Hans, Region: HK.</para>
        /// </example>
        [PublicAPI]
        public readonly string Region;

        public CountryCode(string language, string country, string region = "")
        {
            Language = language.ToLower();
            Country = country.ToLower();
            Region = region.ToLower();
        }

        public CountryCode(string countryCode)
        {
            // ReSharper disable once PossiblyMistakenUseOfParamsMethod
            var result = countryCode.Split('-', '_');

            switch ( result.Length )
            {
                case 1:
                    Language = result[0].ToLower();
                    break;

                case 2:
                    Language = result[0].ToLower();
                    Country = result[1].ToLower();
                    break;

                case 3:
                    Language = result[0].ToLower();
                    Country = result[1].ToLower();
                    Region = result[2].ToLower();
                    break;

                default:
                    Language = "en";
                    Country = string.Empty;
                    Region = string.Empty;
                    break;
            }
        }

        /// <summary>
        /// Gets formatted country code.
        /// </summary>
        /// <returns>Country code formatted as $"{<see cref="Language"/>}-{<see cref="Country"/>}".</returns>
        [PublicAPI]
        public string GetCountryCode()
        {
            return $"{Language}-{Country}";
        }

        /// <summary>
        /// Converts localization details to a dash-separated format.
        /// </summary>
        /// <returns>Country code formatted as $"{<see cref="Language"/>}-{<see cref="Country"/>}".</returns>
        [PublicAPI]
        public override string ToString()
        {
            return GetCountryCode().ToLower().TrimEnd( '-' );
        }
    }
}