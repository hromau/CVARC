using System;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure;
using log4net;
using ProxyCommon;

namespace MultiplayerProxy
{
    public static class WebServer
    {
        private static readonly ILog log = LogManager.GetLogger(nameof(WebServer));
        private static Guid[] playerGuids = new Guid[0];

        public static void SendResult(WebCommonResults result)
        {
            log.Debug("SendResults call");
            WebHelper.PutAsync(result, MultiplayerProxyConfigurations.UriToPutResult + "?apiKey=" + MultiplayerProxyConfigurations.ApiKey);
        }

        public static bool CvarcTagExists(Guid guid) => playerGuids.Contains(guid);

        public static async Task UpdateCvarcTagList()
        {
            while (true)
            {
                log.Debug("Time to update cvarctag list!");
                var newGuids = await WebHelper.ReadFromUrlAsync<Guid[]>(
                    $"{MultiplayerProxyConfigurations.UriToCvarcTagList}?apiKey={MultiplayerProxyConfigurations.ApiKey}");
                if (newGuids != null)
                    playerGuids = newGuids;
                else
                    log.Warn("Cant get cvarctag list :(");
                await Task.Delay(MultiplayerProxyConfigurations.CvarcTagListTimeToLive);
            }
        }
    }
}
