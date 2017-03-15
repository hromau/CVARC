using System;
using System.Collections.Generic;
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
        private static Dictionary<LoadingData, string[]> LevelToControllerIds;

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

                await GetResultAndSendToWeb(mainConnection, settings);
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
            var controllerList = LevelToControllerIds[levelName];
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

        private static async Task GetResultAndSendToWeb(TcpClient mainConnection, GameSettings settings)
        {
            var result = await mainConnection.ReadJsonAsync<GameResult>();
            var webResult = new WebCommonResults();
            webResult.GameName = settings.LoadingData.ToString();
            webResult.PathToLog = result.PathToLogFile;
            webResult.RoleToCvarcTag = settings.ActorSettings
                .Where(s => !s.IsBot)
                .ToDictionary(s => s.ControllerId, s => s.PlayerSettings.CvarcTag);
            webResult.Scores = result.ScoresByPlayer;
            WebServer.SendResult(webResult);
            Pool.CheckGame();
        }

        public static Dictionary<LoadingData, string[]> GetControllersIdList()
        {
            if (LevelToControllerIds != null)
                return LevelToControllerIds;
            try
            {
                var service = new TcpClient();
                service.Connect(MultiplayerProxyConfigurations.ServiceEndPoint);
                service.WriteJson(ServiceUnityCommand.GetCompetitionsList);
                LevelToControllerIds = 
                    service.ReadJson<Dictionary<string, string[]>>()
                    .ToDictionary(kvp => LoadingData.Parse(kvp.Key), kvp => kvp.Value);
                return LevelToControllerIds;
            }
            catch (Exception e)
            {
                log.Error("cant load list of controllers id", e);
                throw;
            }
            
        }
    }
}
