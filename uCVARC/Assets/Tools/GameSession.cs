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
            proxyConnection.WriteJson(result);
            proxyConnection.Close();
        }
    }
}
