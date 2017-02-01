using System;
using System.Linq;
using CVARC.V2;
using HoMM.Engine;
using HoMM.Rules;

namespace HoMM.World
{
    sealed class HommWorld : World<HommWorldState>
    {
        public HommEngine HommEngine { get; private set; }
        public ICommonEngine CommonEngine { get; private set; }
        public Round Round { get; private set; }
        public Random Random { get; private set; }
        public CombatResolver CombatResolver { get; private set; }

        public Player[] Players { get; private set; }

        private string[] players;

        public HommWorld(params string[] players) : base()
        {
            this.players = players;
        }

        public override void CreateWorld()
        {
            Random = new Random(WorldState.Seed);
            CombatResolver = new CombatResolver();
            CommonEngine = GetEngine<ICommonEngine>();
            HommEngine = GetEngine<HommEngine>();

            var map = MapHelper.CreateMap(Random);
            Players = players.Select(pid => CreatePlayer(pid, map)).ToArray();            
            Round = new Round(map, Players);

            MapUnityConnecter.Connect(Round, HommEngine);

            Clocks.AddTrigger(new TimerTrigger(_ => Round.DailyTick(), HommRules.Current.DailyTickInterval));
        }

        public Location GetRespawnLocation(string controllerId)
        {
            return controllerId == TwoPlayersId.Left
                ? Location.Zero
                : new Location(Round.Map.Size.Y - 1, Round.Map.Size.X - 1);
        }

        private Player CreatePlayer(string controllerId, Map map)
        {
            var player = new Player(controllerId, map);
            player.Location = GetRespawnLocation(controllerId);
            return player;
        }
    }
}
