using System;
using System.Net;
using System.Net.Sockets;
using Infrastructure;

namespace Assets.Servers
{
    public class LogServer : IDisposable
    {
        private TcpListener listener;
        private LogModel logModel;


        public LogServer(int port)
        {
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
        }

        public bool HasGame()
        {
            if (logModel != null)
                return true;
            if (!listener.Pending())
                return false;

            var connection = listener.AcceptTcpClient();
            logModel = connection.ReadJson<LogModel>();
            connection.Close();
            return true;
        }

        public void StartGame()
        {
            Dispatcher.LogModel = logModel;
            logModel = null;
        }

        public void Dispose()
        {
            listener.Stop();
        }
    }
}
