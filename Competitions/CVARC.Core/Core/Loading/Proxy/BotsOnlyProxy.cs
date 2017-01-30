using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace CVARC.V2
{
    public class BotsOnlyProxy : IProxy
    {
        public void PrepareConnection(string ip, int port, Configuration config, 
            List<ControllerSettings> controllers, IWorldState worldState)
        {
            var tcpClient = new TcpClient();
            tcpClient.Connect(ip, port);
            var cvarcClient = new CvarcClient(tcpClient);
            cvarcClient.Write(config.LoadingData);
            cvarcClient.Write(worldState);
            cvarcClient.Write(controllers);
            //cvarcClient.Close();
        }
    }
}
