using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;

namespace Infrastructure
{
    public static class TcpClientExtensions
    {
        const byte EndLine = (byte)'\n';

        public static bool IsAlive(this TcpClient client) =>
            !(client.Client.Poll(0, SelectMode.SelectRead) && client.Client.Available == 0);

        private static byte[] ReadLine(this TcpClient client)
        {
            var data = new byte[1024];
            var result = new List<byte>();
            var isLastPart = false;
            while (!isLastPart)
            {
                var read = client.GetStream().Read(data, 0, data.Length); // в случае, если будет закрыт клиент с ЭТОЙ стороны, то бросится исключение IO че-то там.
                if (read == 0) // метод Read у стрима возвращают 0 только тогда, когда данных больше не ожидается, т.е. сокет закрыт С ТОЙ стороны. Это блокирующий метод.
                    throw new Exception("socket closed before line accepted"); // пруф: https://msdn.microsoft.com/ru-ru/library/system.io.stream.read(v=vs.110).aspx комментарии.
                isLastPart = data[read - 1] == EndLine;
                result.AddRange(data.Take(isLastPart ? read - 1 : read));
            }
            return result.ToArray();
        }

        private static void WriteLine(this TcpClient client, byte[] line)
        {
            line = line.Where(x => x != EndLine).Concat(new[] {EndLine}).ToArray();
            client.GetStream().Write(line, 0, line.Length);
        }

        public static T ReadJson<T>(this TcpClient client)
        {
            return Serializer.Deserialize<T>(Encoding.UTF8.GetString(client.ReadLine()));
        }

        public static object ReadJson(this TcpClient client, Type type)
        {
            return Serializer.Deserialize(Encoding.UTF8.GetString(client.ReadLine()),type);
        }



        public static void WriteJson(this TcpClient client, object obj)
        {
            client.WriteLine(Encoding.UTF8.GetBytes(Serializer.Serialize(obj)));
        }
    }
}
