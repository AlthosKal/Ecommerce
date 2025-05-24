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
#if DEBUG
#if ANDROID
  
                //return "https://192.168.95.99:7261/api/v1"; 
                return "https://192.168.1.21:7261/api/v1"; 
#elif IOS
                return "https://localhost:7261/api/v1";
#else
                // Windows, MacCatalyst, etc.
                return "https://localhost:7261/api/v1";
#endif
#else
                // Producción
                return "https://your-production-domain.com/api/v1";
#endif
            }
        }
    }
}