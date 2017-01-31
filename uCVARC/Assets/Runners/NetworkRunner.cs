using System;
using System.Collections.Generic;
using System.Linq;
using CVARC.V2;
using Infrastructure;

namespace Assets
{
    public class NetworkRunner : IRunner
    {
        readonly List<MultiplayingPlayer> players;
        readonly int requiredCountOfPlayers;
        readonly Configuration configuration;
        readonly string[] controllerIds;
        public readonly IWorldState worldState;
        readonly string logFileName;
        NetTournamentControllerFactory factory;
        bool startedSuccessfully;
        public readonly NetworkServer server;
        readonly JsonSerializer json = new JsonSerializer();

        public IWorld World { get; set; }
        public string Name { get; set; }

        public bool CanStart
        {
            get
            {
                if (requiredCountOfPlayers != players.Count) return false;
                if (players.All(p => p.Client.IsAlive())) return true;
                Dispose(); //TODO: Clean the method - why have dispose there?
                return false;
            }
        }
        public bool CanInterrupt { get; set; }
        public bool Disposed { get; set; }


        public NetworkRunner(IWorldState worldState, Configuration configuration, NetworkServer server)
        {
            this.worldState = worldState;
            this.configuration = configuration;
            this.server = server;
            players = new List<MultiplayingPlayer>();

            //log section
            logFileName = Guid.NewGuid() + ".cvarclog";
            configuration.Settings.EnableLog = true;
            configuration.Settings.LogFile = UnityConstants.LogFolderRoot + logFileName;
            //log section end

            var competitions = Dispatcher.Loader.GetCompetitions(configuration.LoadingData);
            var clientControllers = configuration.Settings.Controllers.Where(x => x.Type == ControllerType.Client);
            controllerIds = clientControllers.Select(x => x.ControllerId).ToArray();
            requiredCountOfPlayers = clientControllers.Count();

            Debugger.Log(DebuggerMessageType.Unity, "network runner created for " + requiredCountOfPlayers + " players");

            Name = configuration.LoadingData.AssemblyName + configuration.LoadingData.Level;//"Tournament";
            CanInterrupt = false;
        }

        public void AddPlayer(MultiplayingPlayer player)
        {
            if (players.Count == requiredCountOfPlayers)
                throw new Exception("already full");
            var curPlayers = players.ToArray();
            foreach (var pl in curPlayers)
                if (!pl.Client.IsAlive())
                {
                    Debugger.Log(DebuggerMessageType.Unity, "one of players disconnected before game started");
                    players.Remove(pl);
                }
            players.Add(player);
        }

        public void PrepareStart()
        {
            var controllersMap = new Dictionary<string, IMessagingClient>();
            for (var i = 0; i < requiredCountOfPlayers; i++)
                controllersMap.Add(controllerIds[i], players[i].Client);
            factory = new NetTournamentControllerFactory(controllersMap);
        }

        public void InitializeWorld()
        {
            if (World == null)
                World = Dispatcher.Loader.CreateWorld(configuration, factory, worldState);
            startedSuccessfully = true;
        }

        public void Dispose()
        {
            Debugger.Log(DebuggerMessageType.Unity, "dispose tournament...");
            foreach (var cvarcClient in players)
                cvarcClient.Client.Close();

            if (World != null)
                World.OnExit();

            if (startedSuccessfully)
                SendResultsToServer();

            Disposed = true;
        }

        private void SendResultsToServer()
        {
            if (World == null)
                return;

            var ids = controllerIds.ToArray();
            var scores = World.Scores.GetAllScores().ToDictionary(p => p.Item1, p => p.Item2);

            var playersInfo = Enumerable.Range(0, players.Count)
                .Select(
                    num =>
                        new PlayerInfo
                        {
                            ControllerId = ids[num],
                            CvarcTag = players[num].CvarcTag,
                            Score = scores[ids[num]]
                        })
                .ToArray();
            
            server.messagingClient.WriteLine(json.Serialize(playersInfo));
            server.messagingClient.WriteLine(json.Serialize(logFileName));
            server.CloseGame();
        }
    }
}
