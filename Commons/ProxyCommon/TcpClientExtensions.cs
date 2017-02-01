using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Infrastructure;

namespace ProxyCommon
{
    public static class TcpClientExtensions
    {
        const byte EndLine = (byte)'\n';

        public static async Task<byte[]> ReadAsync(this TcpClient client)
        {
            var data = new byte[1024];
            var read = await client.GetStream().ReadAsync(data, 0, data.Length);
            if (read == 0) // метод Read у стрима возвращают 0 только тогда, когда данных больше не ожидается, т.е. сокет закрыт. Это блокирующий метод.
                throw new Exception("socket closed before line accepted"); // пруф: https://msdn.microsoft.com/ru-ru/library/system.io.stream.read(v=vs.110).aspx комментарии.
            return data.Take(read).ToArray();
        }

        public static async Task WriteAsync(this TcpClient client, byte[] data)
        {
            await client.GetStream().WriteAsync(data, 0, data.Length);
        }

        public static async Task<T> ReadJsonAsync<T>(this TcpClient client)
        {
            var result = new List<byte>();
            while (true)
            {
                var read = await client.ReadAsync();
                result.AddRange(read);
                if (read[read.Length - 1] == EndLine)
                    break;
            }
            result.RemoveAt(result.Count - 1);
            return Serializer.Deserialize<T>(Encoding.UTF8.GetString(result.ToArray()));
        }

        public static async Task WriteJsonAsync<T>(this TcpClient client, T obj)
        {
            var line = Encoding.UTF8.GetBytes(Serializer.Serialize(obj));
            line = line.Where(x => x != EndLine).Concat(new[] { EndLine }).ToArray();
            await client.WriteAsync(line);
        }
    }
}
