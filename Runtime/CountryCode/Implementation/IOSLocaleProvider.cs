#if UNITY_IPHONE
using System.Runtime.InteropServices;

namespace GameKit.Implementation
{
    internal class IOSLocaleProvider : IPlatformLocaleProvider
    {
        [DllImport("__Internal")]
        static extern string GetCountryCodeNative();
        
        [DllImport("__Internal")]
        static extern string GetRegionNative();
        
        public Locale GetPlatformLocale()
        {
            var result = GetCountryCodeNative();
            var region = GetRegionNative();
            return new Locale( result + "-" + region );
        }
    }
}
#endif