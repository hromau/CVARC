using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using CVARC.V2;
using Infrastructure;

namespace Assets.Servers
{
    public class PlayServer : IDisposable
    {
        private TcpListener listener;
        private TcpClient proxyConnection;
        private bool gameStarted;
        private Dictionary<string, TcpClient> players;

        public PlayServer(int port)
        {
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
        }

        public Dictionary<string, TcpClient> CheckGame()
        {
            if (!listener.Pending() || gameStarted)
                return null;
            proxyConnection = listener.AcceptTcpClient();
            var gameSettings = proxyConnection.ReadJson<GameSettings>();
            players = new Dictionary<string, TcpClient>();
                
            foreach (var settings in gameSettings.ActorSettings.Where(x => !x.IsBot))
                players[settings.ControllerId] = listener.AcceptTcpClient();

            gameStarted = true;

            return players;
        }

        public void EndGame(GameResult gameResult)
        {
            proxyConnection.WriteJson(gameResult);
            proxyConnection.Close();
            foreach (var player in players.Values)
                player.Close();
            gameStarted = false;
        }

        public void Dispose()
        {
            listener.Stop();
        }
    }
}
