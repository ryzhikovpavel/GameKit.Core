namespace GameKit.Core.CountryCode.Implementation
{
    public class DefaultPlatformLocaleProvider : IPlatformLocaleProvider
    {
        public GameKit.Core.CountryCode.Locale GetCountryCode()
        {
            return new Locale("en", "US");
        }
    }
}