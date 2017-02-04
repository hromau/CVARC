using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Assets.Tools;
using CVARC.V2;
using Infrastructure;

namespace Assets.Servers
{
    public class PlayServer : IDisposable
    {
        private TcpListener listener;
        private TcpClient proxyConnection;
        private WorldCreationParams worldCreationParams;
        public bool GameStarted;

        public PlayServer(int port)
        {
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
        }

        public bool HasGame()
        {
            if (worldCreationParams != null)
                return true;
            if (!listener.Pending() || GameStarted)
                return false;

            proxyConnection = listener.AcceptTcpClient();
            var gameSettings = proxyConnection.ReadJson<GameSettings>();
            var players = new Dictionary<string, TcpClient>();
                
            foreach (var settings in gameSettings.ActorSettings.Where(x => !x.IsBot))
                players[settings.ControllerId] = listener.AcceptTcpClient();

            Debugger.Log("Accepted " + players.Count + " connections");

            var competitions = Dispatcher.Loader.GetCompetitions(gameSettings.LoadingData);

            foreach (var player in players.Values)
                player.ReadJson(competitions.Logic.WorldStateType);


            Debugger.Log("Has Game method ends");

            worldCreationParams = new WorldCreationParams(gameSettings, 
                new PlayControllerFactory(players), 
                DefaultWorldInfoCreator.GetDefaultWorldState(gameSettings.LoadingData));
            return true;
        }

        public GameSession StartGame()
        {
            if (worldCreationParams == null)
                throw new Exception("Game was not ready!");

            GameStarted = true;
            var world = Dispatcher.Loader.CreateWorld(
                worldCreationParams.GameSettings,
                worldCreationParams.ControllerFactory,
                worldCreationParams.WorldState);
            worldCreationParams = null;

            return new GameSession(world, proxyConnection);
        }

        public void Dispose()
        {
            listener.Stop();
        }
    }
}
