using System;
using System.IO;
using System.Net.Sockets;
using Infrastructure;

namespace ReplayRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("USAGE:");
                Console.WriteLine("LogRunner.exe PATH_TO_LOG.cvarcreplay");
                Environment.Exit(1);
            }

            if (!File.Exists(args[0]))
            {
                Console.WriteLine("Replay file not found");
                Environment.Exit(2);
            }

            var tcpClient = new TcpClient();
            try
            {
                tcpClient.Connect("127.0.0.1", 14003);
            }
            catch (SocketException)
            {
                Console.WriteLine("Cant connect to server. Do you run it?");
                Console.ReadKey();
                Environment.Exit(3);
            }

            tcpClient.WriteJson(Path.GetFullPath(args[0]));
            tcpClient.Close();
        }
    }
}
