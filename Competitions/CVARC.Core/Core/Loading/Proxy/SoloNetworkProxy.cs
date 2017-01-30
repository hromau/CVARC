using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace CVARC.V2
{
    public class SoloNetworkProxy : IProxy
    {
        public void PrepareConnection(string ip, int port, Configuration config, IWorldState worldState,
            string playerSide)
        {
            if (playerSide != "Left" && playerSide != "Right")
                throw new ArgumentException("Side must be either Left or Right!");

            var tcpClient = new TcpClient();
            tcpClient.Connect(ip, port);
            var cvarcClient = new CvarcClient(tcpClient);
            cvarcClient.Write(config);
            cvarcClient.Write(worldState);

            var controllers = new List<ControllerSettings>();
            controllers.Add(new ControllerSettings { ControllerId = playerSide, Type = ControllerType.Client });
            controllers.Add(new ControllerSettings
            {
                ControllerId = playerSide == "Left" ? "Right" : "Left",
                Name = "Standing",
                Type = ControllerType.Bot
            });
            cvarcClient.Write(controllers);
        }
    }
}
