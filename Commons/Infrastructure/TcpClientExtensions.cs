
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Infrastructure
{
    public static class TcpClientExtensions
    {
        const byte EndLine = (byte)'\n';

        public static bool IsAlive(this TcpClient client) =>
            !(client.Client.Poll(0, SelectMode.SelectRead) && client.Client.Available == 0);

        private static byte[] ReadLine(this TcpClient client)
        {
            var data = new byte[1];
            var result = new List<byte>();
            while (true)
            {
                var read = client.GetStream().Read(data, 0, data.Length); // в случае, если будет закрыт клиент с ЭТОЙ стороны, то бросится исключение IO че-то там.
                if (read == 0) // метод Read у стрима возвращают 0 только тогда, когда данных больше не ожидается, т.е. сокет закрыт С ТОЙ стороны. Это блокирующий метод.
                    throw new Exception("socket closed before line accepted"); // пруф: https://msdn.microsoft.com/ru-ru/library/system.io.stream.read(v=vs.110).aspx комментарии.
                if (data[0] == EndLine)
                    break;
                result.Add(data[0]);
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
            var str = Encoding.UTF8.GetString(client.ReadLine());
            Debugger.Log(str);
            return Serializer.Deserialize<T>(str);
        }

        public static object ReadJson(this TcpClient client, Type type)
        {
            var str = Encoding.UTF8.GetString(client.ReadLine());
            Debugger.Log(str);
            return Serializer.Deserialize(str,type);
        }

        public static bool TryReadJson<T>(this TcpClient client, TimeSpan timeout, out T result)
        {
            var res = default(T);

            var thread = new Thread(() =>
            {
                res = client.ReadJson<T>();
            });
            thread.Start();

            var success = thread.Join(timeout);
            result = res;
            return success;
        }

        public static void WriteJson(this TcpClient client, object obj)
        {
            var str = Serializer.Serialize(obj);
            Debugger.Log(str);
            client.WriteLine(Encoding.UTF8.GetBytes(str));
        }
    }
}
