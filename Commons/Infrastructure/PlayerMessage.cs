using System.Net.Sockets;
using Newtonsoft.Json.Linq;

namespace Infrastructure
{
    public class PlayerMessage
    {
        public MessageType MessageType { get; set; }
        public JObject Message { get; set; }
    }

    public enum MessageType
    {
        SensorData,
        Info,
        Error
    }

    public static class PlayerMessageHelper
    {
        public static string GetQueueMessage(int length, int needToPlay) =>
            needToPlay <= length
                ? "You ready to start. Wait other people or current game ends..."
                : $"You {length} in queue.";

        public static void SendMessage(TcpClient client, MessageType type, object message)
        {
            client.WriteJson(new PlayerMessage
            {
                MessageType = type,
                Message = new JObject(message)
            });
            if (type == MessageType.Error)
                client.Close();
        }
    }
}