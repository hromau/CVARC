using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace CVARC.V2
{
    public class PlayControllerFactory : ControllerFactory
    {
        private readonly Dictionary<string, TcpClient> players;
        public PlayControllerFactory(Dictionary<string, TcpClient> players)
        {
            this.players = players;
        }

        public override IController Create(string controllerId, IActor actor)
        {
            if (GetSettings(controllerId).IsBot)
                return CreateBot(controllerId);

            var controller = World.Competitions.Logic.Actors[controllerId].CreateNetworkController();
            controller.InitializeClient(players[controllerId]);
            return controller;
        }

        
    }
}
