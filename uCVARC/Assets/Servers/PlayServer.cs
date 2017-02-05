using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Assets.Tools;
using CVARC.V2;
using Infrastructure;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Assets.Servers
{
    public class PlayServer : IDisposable
    {
        private readonly TcpListener listener;
        private TcpClient proxyConnection;
        private WorldCreationParams worldCreationParams;

        public PlayServer(int port)
        {
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
        }

        public bool HasGame()
        {
            if (worldCreationParams != null)
                return true;
            if (!listener.Pending())
                return false;

            proxyConnection = listener.AcceptTcpClient();
            GameSettings gameSettings;
            JObject worldStateObj;
            if (!proxyConnection.TryReadJson(TimeSpan.FromSeconds(1), out gameSettings) ||
                !proxyConnection.TryReadJson(TimeSpan.FromSeconds(1), out worldStateObj))
            {
                proxyConnection.Close();
                Debug.Log("Failed");
                return false;
            }
            var competitions = Dispatcher.Loader.GetCompetitions(gameSettings.LoadingData);
            var worldState = (WorldState)worldStateObj.ToObject(competitions.Logic.WorldStateType);
            var players = new Dictionary<string, TcpClient>();
            if (worldState.Undefined)
            {
                Debug.Log("default");
                worldState = DefaultWorldInfoCreator.GetDefaultWorldState(gameSettings.LoadingData);
            }
            foreach (var settings in gameSettings.ActorSettings.Where(x => !x.IsBot))
                players[settings.ControllerId] = listener.AcceptTcpClient();

            Debugger.Log("Accepted " + players.Count + " connections");

            worldCreationParams = new WorldCreationParams(gameSettings, 
                new PlayControllerFactory(players), 
                worldState);
            return true;
        }

        public GameSession StartGame()
        {
            if (worldCreationParams == null)
                throw new Exception("Game was not ready!");

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
