using System.Net.Sockets;
using System.Threading.Tasks;
using log4net;
using Infrastructure;
using System;
using ProxyCommon;

namespace MultiplayerProxy
{
    public class Program
    {
        private static readonly ILog log = LogManager.GetLogger(nameof(Program));

        public static void Main()
        {

            Debugger.Config = new DebuggerConfig { AlwaysOn=true };
            Debugger.Logger += Console.WriteLine;

            Task.Factory.StartNew(WebServer.UpdateCvarcTagList, TaskCreationOptions.LongRunning);

            var listener = new TcpListener(MultiplayerProxyConfigurations.ProxyEndPoint);
            listener.Start();
            log.Debug("Listener started");

            while (true)
            {
                var client = listener.AcceptTcpClient();
                log.Info("Client accepted!");
                Task.Run(() => Pool.CreatePlayerInPool(client));
            }
        }
    }
}
