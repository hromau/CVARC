using System;
using System.Net.Sockets;
using CVARC.V2;
using Infrastructure;

namespace Assets.Tools
{
    public class GameSession
    {
        public readonly IWorld World;
        private readonly TcpClient proxyConnection;

        public GameSession(IWorld world, TcpClient proxyConnection)
        {
            World = world;
            this.proxyConnection = proxyConnection;
        }

        public void EndSession(GameResult result)
        {
            // на случай спидапа сетим обратно
            Dispatcher.SetTimeScale(Constants.TimeScale);

            try
            {
                proxyConnection.WriteJson(result);
            }
            catch (Exception) { }
            
            proxyConnection.Close();
        }
    }
}
