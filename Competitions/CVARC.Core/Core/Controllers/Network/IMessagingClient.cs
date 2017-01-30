using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CVARC.V2
{
    public interface IMessagingClient
    {
        void WriteLine(byte[] bytes);
        byte[] ReadLine();
        void Close();
		bool EnableDebug { get; set; }
        bool IsAlive();
    }

    public static class IMessagingClientExtensions
    {
        static readonly ISerializer Serializer = new JsonSerializer();

        public static object Read(this IMessagingClient client, Type type)
        {
            var line = client.ReadLine();
          //  Debugger.Log(DebuggerMessageType.Unity, "READ: "+Encoding.ASCII.GetString(line));
            return Serializer.Deserialize(type, line);
        }

        public static T Read<T>(this IMessagingClient client)
        {
            return (T)client.Read(typeof(T));
        }
        public static void Write(this IMessagingClient client, object obj)
        {
            client.WriteLine(Serializer.Serialize(obj));
        }
    }
}
