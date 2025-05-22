    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace FRONT_END.Service
    {
    public static class ConfigService
    {
        public static string ApiBaseUrl
        {
            get
            {
#if ANDROID
                return "https://172.25.192.1:7261/api/v1/country";
#else
                throw new NotImplementedException("ApiBaseUrl is not implemented in this platform");
#endif
            }
        }
    }
}
