#if !UNITY_IPHONE
using System.Runtime.InteropServices;

namespace GameKit.Core.CountryCode.Implementation
{
    public class IOSCountryCodeProvider : IPlatformLocaleProvider
    {
        [DllImport("__Internal")]
        static extern string GetCountryCodeNative();
        
        [DllImport("__Internal")]
        static extern string GetRegionNative();
        
        public Locale GetCountryCode()
        {
            var result = GetCountryCodeNative();
            var region = GetRegionNative();
            return new Locale( result + "-" + region );
        }
    }
}
#endif