using CVARC.V2;
using Infrastructure;
using Ionic.Zip;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SingleplayerProxy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ReplayDebugger
{
    class Program
    {
        

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Pass the cvarcreplay filename as the first argument");
                Console.ReadKey();
                return;
            }

            var zip = ZipFile.Read(args[0]);
            var settingsFile = zip.Where(z => z.FileName == LogNames.GameSettings).SingleOrDefault();
            if (settingsFile == null)
                throw new Exception("No " + LogNames.GameSettings + " file is inside archive");
            var GameSettings = JsonConvert.DeserializeObject<GameSettings>(Encoding.UTF8.GetString(settingsFile.OpenReader().ReadToEnd()));

            var worldStateFile = zip.Where(z => z.FileName == LogNames.WorldState).SingleOrDefault();
            if (worldStateFile == null)
                throw new Exception("No " + LogNames.WorldState + " file is inside archive");
            var worldState = JsonConvert.DeserializeObject<JObject>(Encoding.UTF8.GetString(settingsFile.OpenReader().ReadToEnd()));

            var replayFile = zip.Where(z => z.FileName == LogNames.Replay).SingleOrDefault();
            if (replayFile==null)
                throw new Exception("No " + LogNames.Replay + " file is inside archive");

            var commands = ParseCommands(replayFile);

            var client = new TcpClient();
            client.Connect(SingleplayerProxyConfigurations.UnityEndPoint);
            client.WriteJson(GameSettings);
            client.WriteJson(worldState);
            foreach(var e in GameSettings.ActorSettings.Select(z=>z.ControllerId))
            {
                var commandClient = new TcpClient();
                commandClient.Connect(SingleplayerProxyConfigurations.UnityEndPoint);
                new Thread(() => RunClient(commandClient, commands[e])).Start();
            }
            client.ReadJson<GameResult>();
        }

        private static void RunClient(TcpClient client, List<JObject> list)
        {
            foreach(var e in list)
            {
                var  q= client.ReadJson<PlayerMessage>();
                if (q.MessageType == MessageType.Error)
                    Console.WriteLine(q.Message);
                client.WriteJson(e);
                Console.Write(".");
            }
            Console.WriteLine("Done");
            client.Close();
        }

        private static Dictionary<string,List<JObject>> ParseCommands(ZipEntry replayFile)
        {
            var reader = new StreamReader(replayFile.OpenReader(), Encoding.UTF8);
            var result = new Dictionary<string, List<JObject>>();
            while(true)
            {
                var line = reader.ReadLine();
                if (line == null) break;
                var entry = JsonConvert.DeserializeObject<GameLogEntry>(line);
                if (entry.Type != GameLogEntryType.IncomingCommand) continue;
                if (!result.ContainsKey(entry.IncomingCommand.ControllerId))
                    result[entry.IncomingCommand.ControllerId] = new List<JObject>();
                result[entry.IncomingCommand.ControllerId].Add(entry.IncomingCommand.Command);
            }
            return result;
        }
    }
}