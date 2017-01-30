using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Tools;
using CVARC.Infrastructure;
using CVARC.V2;

namespace Assets
{
    public class MultiplayerRunner : IRunner
    {
        readonly List<MultiplayingPlayer> players;
        readonly int requiredCountOfPlayers;
        readonly Configuration configuration;
        readonly string[] controllerIds;
        readonly IWorldState worldState;
        readonly string logFileName;
        NetTournamentControllerFactory factory;
        bool startedSuccessfully;

        public IWorld World { get; set; }
        public string Name { get; set; }

        public bool CanStart
        {
            get
            {
                if (requiredCountOfPlayers != players.Count) return false;
                if (players.All(p => p.Client.IsAlive())) return true;
                Dispose();
                return false;
            }
        }
        public bool CanInterrupt { get; set; }
        public bool Disposed { get; set; }


        public MultiplayerRunner(IWorldState worldState, Configuration configuration)
        {
            this.worldState = worldState;
            this.configuration = configuration;
            players = new List<MultiplayingPlayer>();

            //log section
            logFileName = Guid.NewGuid() + ".cvarclog";
            configuration.Settings.EnableLog = true;
            configuration.Settings.LogFile = UnityConstants.LogFolderRoot + logFileName;
            //log section end

            var competitions = Dispatcher.Loader.GetCompetitions(configuration.LoadingData);
            controllerIds = competitions.Logic.Actors.Keys.ToArray();

            configuration.Settings.Controllers.Clear();

            foreach (var controller in controllerIds
                .Select(x => new ControllerSettings
                {
                    ControllerId = x,
                    Type = ControllerType.Client,
                    Name = configuration.Settings.Name
                }))
                configuration.Settings.Controllers.Add(controller);

            requiredCountOfPlayers = controllerIds.Length;

            Debugger.Log(DebuggerMessageType.Unity, "t.runner created. count: " + requiredCountOfPlayers);
            if (requiredCountOfPlayers == 0)
                throw new Exception("requiered count of players cant be 0");

            Name = configuration.LoadingData.AssemblyName + configuration.LoadingData.Level;//"Tournament";
            CanInterrupt = false;
        }

        public bool AddPlayerAndCheck(MultiplayingPlayer player)
        {
            if (players.Count == requiredCountOfPlayers)
                throw new Exception("already started");
            var curPlayers = players.ToArray();
            foreach (var pl in curPlayers)
                if (!pl.Client.IsAlive())
                {
                    Debugger.Log(DebuggerMessageType.Unity, "one of player disconnected before game started");
                    players.Remove(pl);
                }
            players.Add(player);
            if (players.Count != requiredCountOfPlayers)
                return false;
            PrepareStart();
            return true;
        }

        void PrepareStart()
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
            HttpWorker.SendGameResultsIfNeed(playersInfo, logFileName);
        }
    }
}
