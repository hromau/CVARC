using System.Collections.Generic;


namespace CVARC.V2
{
    public class NetTournamentControllerFactory : ControllerFactory
    {
        readonly Dictionary<string, IMessagingClient> players;

        public NetTournamentControllerFactory(Dictionary<string, IMessagingClient> players)
        {
            this.players = players;
        }

        public override IController Create(string controllerId, IActor actor)
        {
            if (GetSettings(controllerId).Type == ControllerType.Bot)
                return CreateBot(controllerId);

            var controller = World.Competitions.Logic.Actors[controllerId].CreateNetworkController();
            controller.InitializeClient(players[controllerId]);
            return controller;
        }
    }
}
