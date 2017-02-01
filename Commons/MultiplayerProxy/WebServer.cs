using System;
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

        public static void SendResult(GameResult result)
        {
            log.Debug("SendResults call");
            log.Debug("Заглушка. Мы получили результат от сервера: " + result.Hehmeh);
        }

        public static bool CvarcTagExists(Guid guid) => true; //playerGuids.Contains(guid); TODO!!!

        public static async Task UpdateCvarcTagList()
        {
            while (true)
            {
                log.Debug("Time to update cvarctag list!");
                var newGuids = await WebHelper.ReadFromUrlAsync<Guid[]>(
                    $"{MultiplayerProxyConfigurations.UriToCvarcTagList}?password={MultiplayerProxyConfigurations.WebPassword}");
                if (newGuids != null)
                    playerGuids = newGuids;
                else
                    log.Warn("Cant get cvarctag list :(");
                await Task.Delay(MultiplayerProxyConfigurations.CvarcTagListTimeToLive);
                log.Debug("WAH");
            }
        }
    }
}
