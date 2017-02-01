using System.Net.Sockets;
using Infrastructure;

namespace ProxyCommon
{
    public class ClientWithSettings
    {
        public TcpClient Client { get; set; }
        public PlayerSettings Settings { get; set; }
    }
}
