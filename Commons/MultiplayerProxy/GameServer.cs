using System;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using Infrastructure;
using log4net;
using ProxyCommon;
using CVARC.V2;
using Newtonsoft.Json;

namespace MultiplayerProxy
{
    public static class GameServer
    {
        private static readonly ILog log = LogManager.GetLogger(nameof(GameServer));

        public static async Task StartGame(ClientWithSettings[] clientsWithSettings, LoadingData levelName)
        {
            log.Debug("StartGame call");

            
            var settings = CreateGameSettings(clientsWithSettings, levelName);

            try
            {
                var mainConnection = ConnnectToServer();
                log.Info(JsonConvert.SerializeObject(settings));
                mainConnection.WriteJson(settings);
                mainConnection.WriteJson(WorldState.MakeUndefined());

                foreach (var client in clientsWithSettings.Select(c => c.Client))
                    CreateConnectionBetweenPlayerAndServer(client);

                await GetResultAndSendToWeb(mainConnection);
            }
            catch (SocketException e)
            {
                log.Error("Socket error", e);
            }
            catch (Exception e)
            {
                log.Fatal("UNKNOWN ERROR!", e);
                Console.WriteLine("fatal error");
            }
            finally
            {
                foreach (var clientsWithSetting in clientsWithSettings)
                    clientsWithSetting.Client.Close();
            }
        }

        private static GameSettings CreateGameSettings(ClientWithSettings[] settings, LoadingData levelName)
        {
            var controllerList = MultiplayerProxyConfigurations.LevelToControllerIds[levelName];
            var defaultSettings = MultiplayerProxyConfigurations.DefaultGameSettings;
            defaultSettings.LoadingData = levelName;
            defaultSettings.ActorSettings = controllerList.Select((t, i) => new ActorSettings
            {
                ControllerId = t,
                PlayerSettings = settings[i].Settings
            }).ToList();
            return defaultSettings;
        }

        private static TcpClient ConnnectToServer()
        {
            var tcpClient = new TcpClient();
            try
            {
                tcpClient.Connect(MultiplayerProxyConfigurations.UnityEndPoint);
            }
            catch (SocketException)
            {
                log.Error("Cant connect to unity server!");
                throw;
            }
            
            return tcpClient;
        }

        private static void CreateConnectionBetweenPlayerAndServer(TcpClient client)
        {
            var server = ConnnectToServer();
            Proxy.CreateChainAndStart(client, server);
        }

        private static async Task GetResultAndSendToWeb(TcpClient mainConnection)
        {
            var result = await mainConnection.ReadJsonAsync<GameResult>();
            WebServer.SendResult(result);
            Pool.CheckGame();
        }
    }
}
