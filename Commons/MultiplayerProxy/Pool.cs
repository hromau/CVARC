using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
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
        private static readonly ILog log = LogManager.GetLogger(nameof(Pool));

        static Pool()
        {
            Task.Factory.StartNew(GameChecker, TaskCreationOptions.LongRunning);
            log.Info("Checker task started");
        }

        public static async Task CreatePlayerInPool(TcpClient client)
        {
            log.Debug("CreatePlayerInPool call");
            var settings = await client.ReadJsonAsync<GameSettings>();
            //await client.ReadJobject();
            var levelName = settings.LoadingData;
            var errorMessage = CheckForErrors(settings);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                log.Warn($"Player got error: {errorMessage}");
                PlayerMessageHelper.SendMessage(client, MessageType.Error, errorMessage);
                return;
            }
            var actorSettings = settings.ActorSettings.Single(x => !x.IsBot);

            pool.TryAdd(levelName, new ConcurrentQueue<ClientWithSettings>());

            pool[levelName].Enqueue(new ClientWithSettings
            {
                Client = client,
                Settings = actorSettings.PlayerSettings
            });

            //PlayerMessageHelper.SendMessage(client, MessageType.Info, 
            //    PlayerMessageHelper.GetQueueMessage(pool[levelName].Count, MultiplayerProxyConfigurations.LevelToControllerIds[levelName].Length));

            CheckGame();
        }

        public static void CheckGame() => needToCheck = true;

        private static async Task TryStartGame()
        {
            log.Debug("TryStartGame call");
            foreach (var levelName in pool.Keys)
            {
                var controllerIdsLength = MultiplayerProxyConfigurations.LevelToControllerIds[levelName].Length;
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
            if (!MultiplayerProxyConfigurations.LevelToControllerIds.ContainsKey(settings.LoadingData))
                return $"This LoadingData doesn't exists in proxy settings: {settings.LoadingData}";
            var actorSettings = settings.ActorSettings.Where(x => !x.IsBot).ToArray();
            if (actorSettings.Length != 1)
                return "All players except one must be bots.";
            var playerSettings = actorSettings[0].PlayerSettings;
            if (playerSettings == null)
                return "Player settings must be not null";
            if (!WebServer.CvarcTagExists(playerSettings.CvarcTag))
                return "Unknown cvarctag! try to wait few minutes, if u changed it or create new account";
            return null;
        }

        private static async void GameChecker()
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
    }
}
