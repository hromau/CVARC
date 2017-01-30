using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using CVARC.V2;


namespace Assets
{
    public abstract class UnityServer
    {
        readonly int port;
        bool exitRequest;

        protected abstract void HandleClient(CvarcClient client);
        protected abstract void Print(string str);

        protected UnityServer(int port)
        {
            this.port = port;
        }

        public void RequestExit()
        {
            exitRequest = true;
        }

        public void StartThread()
        {
            var listner = new TcpListener(IPAddress.Any, port);
            listner.Start();
            Print("Server started");
            while (true)
            {
                while (!listner.Pending())
                {
                    if (exitRequest)
                    {
                        listner.Stop();
                        Print("Server Exited");
                        return;
                    }
                    Thread.Sleep(10);
                }
                try
                {
                    var client = listner.AcceptTcpClient();
                    Print("Client accepted");
                    var cvarcClient = new CvarcClient(client);
                    HandleClient(cvarcClient);
                }
                catch (Exception e)
                {
                    Print("error while handle player: " + e.Message);
                }
            }
        }
    }
}
