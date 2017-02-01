using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using CVARC.Core.Controllers.Network;

namespace CVARC.V2
{
    public class CvarcClient : IMessagingClient
    {

		public bool EnableDebug { get; set; }
        const byte EndLine = (byte)'\n';
        public readonly TcpClient client;
        public CvarcClient(TcpClient client)
        {
            this.client = client;
        }

		string BytesToArray(byte[] bytes)
		{
			return System.Text.Encoding.UTF8.GetString(bytes);
		}

        public void WriteLine(byte[] bytes)
        {
            bytes = bytes.Where(z => z != EndLine).Concat(new[] { EndLine }).ToArray();
			if (EnableDebug) Debugger.Log(BytesToArray(bytes));
			client.Client.Send(bytes);
        }

        bool SocketConnected(Socket s)
        {
            bool part1 = s.Poll(0, SelectMode.SelectRead);
            bool part2 = (s.Available == 0);
            if (part1 & part2)
            {//connection is closed
                return false;
            }
            return true;
        }

        public byte[] ReadLine()
        {
            var buffer = new byte[1];
            var read = new List<byte>();
            while (true)
            {
                while (client.Available == 0)
                {
                    Thread.Sleep(1);
                    if (!SocketConnected(client.Client))
                        throw new KickOutException(); // не уверен, можно ли передать null пользователю в метод Handle sensor data, если соединение закрыто.
                }
                if (externallyClosed)
                    throw new KickOutException();
                var length = client.Client.Receive(buffer);
                if (length == 0)
                {
                    //Это устанавливает таймаут в одну секунду, и выбрасывает исключение, даже если соединение открыто, но временно молчит
                    //if (!SocketConnected(client.Client)) throw new IOException("The connection was terminated");
                    continue;
                }
                if (buffer[0] == EndLine) break;
                read.Add(buffer[0]);
            }
			if (EnableDebug)
				Debugger.Log(BytesToArray(read.ToArray()));
            return read.ToArray();
        }

		bool externallyClosed = false;

        public void Close()
        {
			externallyClosed = true;
            client.Close();
        }

        public bool IsAlive()
        {
            try
            {
                return !externallyClosed && SocketConnected(client.Client);
            }
            catch (Exception)
            {
                return false;
            }
            
        }
    }
}
