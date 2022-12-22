using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Net.Http;
using LC.Newtonsoft.Json;
using TapTap.AntiAddiction.Model;
using TapTap.Common.Internal.Json;

namespace TapTap.AntiAddiction.Internal 
{
    internal class StandaloneChina 
    {
        static readonly string ANTI_ADDICTION_SETTINGS_URL = "https://tds-public-config-sh.oss-cn-shanghai.aliyuncs.com/antiaddiction-settings.json";

        static StandaloneResponse current;

        static HttpClient client;

        private static HttpClient HttpClient 
        {
            get 
            {
                if (client == null) 
                {
                    client = new HttpClient();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                }
                return client;
            }
        }

        internal static async Task<bool> Enabled() 
        {
            if (current != null) 
            {
                return current.Enabled;
            }
            
            HttpResponseMessage response = await HttpClient.GetAsync(ANTI_ADDICTION_SETTINGS_URL);
            string resultString = await response.Content.ReadAsStringAsync();
            response.Dispose();

            current = JsonConvert.DeserializeObject<StandaloneResponse>(resultString, TapJsonConverter.Default);
            return current != null && current.Enabled;
        }
    }
}
