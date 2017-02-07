using System;
using System.Linq;
using CVARC.V2;
using HoMM.Engine;
using Infrastructure;
using HoMM.Rules;

namespace HoMM.World
{
    public sealed class HommWorld : World<HommWorldState>
    {
        public HommEngine HommEngine { get; private set; }
        public ICommonEngine CommonEngine { get; private set; }
        public Round Round { get; private set; }
        public Random Random { get; private set; }

        private RoundToUnityConnecter connecter;

        public Player[] Players { get; private set; }

        private string[] players;

        public HommWorld(params string[] players) : base()
        {
            this.players = players;
        }

        public override void CreateWorld()
        {
            Debugger.Log(WorldState.Seed);

            CommonEngine = GetEngine<ICommonEngine>();
            HommEngine = GetEngine<HommEngine>();

            Random = new Random(WorldState.Seed);

            var map = new MapGeneratorHelper().CreateMap(Random);
            Players = players.Select(pid => CreatePlayer(pid, map)).ToArray();
            Round = new Round(map, Players);

            connecter = new RoundToUnityConnecter(HommEngine, CommonEngine);
            connecter.Connect(Round);

            Clocks.AddTrigger(new TimerTrigger(_ => Round.DailyTick(), HommRules.Current.DailyTickInterval));
        }

        public Location GetRespawnLocation(string controllerId, Map map)
        {
            return controllerId == TwoPlayersId.Left
                ? Location.Zero
                : new Location(map.Size.Y - 1, map.Size.X - 1);
        }

        private Player CreatePlayer(string controllerId, Map map)
        {
            var player = new Player(controllerId, map);
            player.Location = GetRespawnLocation(controllerId, map);
            return player;
        }
    }
}
