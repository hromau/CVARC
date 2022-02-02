using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure;
using log4net;
using ProxyCommon;

namespace MultiplayerProxy
{
    public static class Pool
    {
        private static readonly ConcurrentDictionary<LoadingData, ConcurrentQueue<ClientWithSettings>> pool =
            new ConcurrentDictionary<LoadingData, ConcurrentQueue<ClientWithSettings>>();

        private static bool needToCheck;
        private static readonly ILog log = LogManager.GetLogger(typeof(Pool));
        private static readonly Dictionary<LoadingData, string[]> levelToControllerIds;

        static Pool()
        {
            log.Info("Pool ctor call");
            while (levelToControllerIds == null)
            {
                levelToControllerIds = GameServer.GetControllersIdList();
                if (levelToControllerIds == null)
                    Thread.Sleep(100);
            }
            log.Info("loaded keys: " + string.Join(", ", levelToControllerIds.Select(x => $"\n  {x.Key.ToString()} values: {string.Join("|", x.Value)}")));
            Task.Factory.StartNew(GameChecker, TaskCreationOptions.LongRunning);
            log.Info("Checker task started");
            log.Info("Pool ctor success");
        }

        public static async Task CreatePlayerInPool(TcpClient client)
        {
            log.Debug("CreatePlayerInPool call");
            try
            {
                var settings = await client.ReadJsonAsync<GameSettings>();
                // await client.ReadJsonAsync<JObject>(); // ignore worldstate
                var levelName = settings.LoadingData;
                levelName.AssemblyName = levelName.AssemblyName.ToLower();
                levelName.Level = levelName.Level.ToLower();
                
                var errorMessage = CheckForErrors(settings);
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    log.Warn($"Player got error: {errorMessage}");
                    PlayerMessageHelper.SendMessage(client, MessageType.Error, errorMessage);
                    return;
                }

                log.Info($"Has new player! LoadingData: {settings.LoadingData} CvarcTag: {settings.ActorSettings[0].PlayerSettings.CvarcTag}");

                var actorSettings = settings.ActorSettings.Single(x => !x.IsBot);

                pool.TryAdd(levelName, new ConcurrentQueue<ClientWithSettings>());

                pool[levelName].Enqueue(new ClientWithSettings
                {
                    Client = client,
                    Settings = actorSettings.PlayerSettings
                });

                

                PlayerMessageHelper.SendMessage(client, MessageType.Info,
                    PlayerMessageHelper.GetQueueMessage(pool[levelName].Count, GetByLoadingData(levelName).Length));

                log.Info(PlayerMessageHelper.GetQueueMessage(pool[levelName].Count, GetByLoadingData(levelName).Length));

                CheckGame();
            }
            catch (Exception e)
            {
                log.Error("error while accept player", e);
                PlayerMessageHelper.SendMessage(client, MessageType.Error,
                    "something went wrong! please, contact administrator (fokychuk47@ya.ru)");
                throw;
            }
        }

        public static void CheckGame() => needToCheck = true;

        private static async Task TryStartGame()
        {
            log.Debug("TryStartGame call");
            foreach (var levelName in pool.Keys)
            {
                var controllerIdsLength = GetByLoadingData(levelName).Length;
                if (pool[levelName].Count < controllerIdsLength)
                    continue;
                var players = new List<ClientWithSettings>();
                ClientWithSettings client;
                while (players.Count < controllerIdsLength && pool[levelName].TryDequeue(out client))
                    if (client.Client.IsAlive())
                        players.Add(client);

                if (players.Count < controllerIdsLength)
                    foreach (var player in players)
                        pool[levelName].Enqueue(player);
                else
                    await GameServer.StartGame(players.ToArray(), levelName);
            }
        }

        private static string CheckForErrors(GameSettings settings)
        {
            // это не иф, а ебучий костыль. blame юра.
            if (!levelToControllerIds.Keys.Any(k => k.ToString().Equals(settings.LoadingData.ToString(), StringComparison.CurrentCultureIgnoreCase)))
                return $"This LoadingData doesn't exists in proxy settings: {settings.LoadingData}";
            var actorSettings = settings.ActorSettings.Where(x => !x.IsBot).ToArray();
            if (actorSettings.Length != 1)
                return "All players except one must be bots.";
            var playerSettings = actorSettings[0].PlayerSettings;
            if (playerSettings == null)
                return "Player settings must be not null";
            if (!WebServer.CvarcTagExists(playerSettings.CvarcTag))
                return $"Unknown cvarctag: {playerSettings.CvarcTag}. Try to wait few minutes, if u changed it or create new account";
            return null;
        }

        private static async Task GameChecker()
        {
            while (true)
            {
                if (!needToCheck)
                {
                    await Task.Delay(500);
                    continue;
                }
                needToCheck = false;
                await TryStartGame();
            }
        }

        private static string[] GetByLoadingData(LoadingData ld)
        {
            var value = levelToControllerIds.Keys.Single(
                k => k.ToString().Equals(ld.ToString(), StringComparison.CurrentCultureIgnoreCase));
            return levelToControllerIds[value];
        }
    }
}
