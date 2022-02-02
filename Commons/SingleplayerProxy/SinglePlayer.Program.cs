using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure;
using Ionic.Zip;
using Newtonsoft.Json.Linq;
using ProxyCommon;

namespace SingleplayerProxy
{
    class Program
    {
        private static int currentVersion;

        static void Main(string[] args)
        {
            if (args.Length == 1 && args[0] == "-d")
            {
                SingleplayerProxyConfigurations.DebugMode = true;
                SingleplayerProxyConfigurations.UpdateEnabled = false;
            }

            ReloadVersion();

            UpdateUnityIfNeeded();
            if (!IsUnityUp())
            {
                StartUnity();
                Console.WriteLine("Unity started");
            }

            var listener = new TcpListener(SingleplayerProxyConfigurations.ProxyEndPoint);
            listener.Start();

            var tasks = new[] {WaitUntillUnityClosed(), null};
            Console.WriteLine("Wait connections...");
            while (true)
            {
                tasks[1] = listener.AcceptTcpClientAsync();
                var index = Task.WaitAny(tasks);
                if (index == 0)
                {
                    Console.WriteLine("Unity closed. wait connections to restart unity");
                    tasks[1].Wait();
                    StartUnity();
                    tasks[0] = WaitUntillUnityClosed();
                }

                var client = ((Task<TcpClient>) tasks[1]).Result;
                PlayGame(client);
            }
        }

        static bool IsUnityUp() =>
            SingleplayerProxyConfigurations.DebugMode ||
            !string.IsNullOrEmpty(TrySendUnityCommand<string>(ServiceUnityCommand.Ping,
                TimeSpan.FromMilliseconds(500)));

        static void KillUnity() =>
            TrySendUnityCommand<string>(ServiceUnityCommand.Shutdown, TimeSpan.FromSeconds(5));

        static bool IsUpdateAvailable() =>
            SingleplayerProxyConfigurations.UpdateEnabled &&
            WebHelper.ReadFromUrlAsync<int>(SingleplayerProxyConfigurations.UrlToGetVersion).Result > currentVersion;

        static void StartUnity()
        {
            if (!SingleplayerProxyConfigurations.DebugMode)
                Process.Start(SingleplayerProxyConfigurations.UnityExePath);
            while (string.IsNullOrEmpty(TrySendUnityCommand<string>(ServiceUnityCommand.Ping,
                       TimeSpan.FromSeconds(20))))
            {
                Console.WriteLine("Cant get answer from unity! Timeout. If its actually started, close it");
                if (!SingleplayerProxyConfigurations.DebugMode)
                    Process.Start(SingleplayerProxyConfigurations.UnityExePath);
            }
        }

        static void UpdateUnityIfNeeded()
        {
            Console.WriteLine("Checking update...");
            try
            {
                if (!IsUpdateAvailable())
                {
                    Console.WriteLine("You use actual version");
                    return;
                }

                Console.WriteLine("Update available! Start to download...");
                if (IsUnityUp())
                    KillUnity();
                Thread.Sleep(1000);
                WebHelper.DownloadFile(SingleplayerProxyConfigurations.UrlToGetUpdate, "update.zip").Wait();
                InstallUpdate("update.zip");
                Console.WriteLine("Update successful!");
            }
            catch (Exception e)
            {
                Console.WriteLine("error when try update");
                Console.WriteLine(e.ToString());
            }
        }

        static async Task WaitUntillUnityClosed()
        {
            while (IsUnityUp())
                await Task.Delay(500);
        }

        static void PlayGame(TcpClient client)
        {
            try
            {
                var gameSettings = client.ReadJson<GameSettings>();
                var worldState = client.ReadJson<JObject>();
                client.WriteJson(new PlayerMessage
                {
                    MessageType = MessageType.Info,
                    Message = new JObject("Hello, hero! welcome to the grand tournament")
                });
                var mainConnection = ConnectToServer();
                mainConnection.WriteJson(gameSettings);
                mainConnection.WriteJson(worldState);
                var server = ConnectToServer();
                Proxy.CreateChainAndStart(server, client);
                var resultTask = mainConnection.ReadJsonAsync<GameResult>();
                resultTask.ContinueWith(x =>
                {
                    Console.WriteLine(x.IsFaulted
                        ? $"Cant get game results. Reason: {x.Exception}"
                        : "The game was finished with the following results:" + Environment.NewLine +
                          x.Result.ToString());
                });
            }
            catch (Exception e)
            {
                Console.WriteLine("something went wrong...");
                Console.WriteLine(e.ToString());
            }
        }

        static TcpClient ConnectToServer()
        {
            var client = new TcpClient();
            client.Connect(SingleplayerProxyConfigurations.UnityEndPoint);
            return client;
        }

        public static void InstallUpdate(string pathToZip)
        {
            using (var zip = ZipFile.Read(pathToZip))
                zip.ExtractAll(SingleplayerProxyConfigurations.PathToUnityDir,
                    ExtractExistingFileAction.OverwriteSilently);
        }

        public static void ReloadVersion()
        {
            if (!File.Exists(SingleplayerProxyConfigurations.PathToVersionFile))
                File.WriteAllText(SingleplayerProxyConfigurations.PathToVersionFile, "0");
            currentVersion = int.Parse(File.ReadAllText(SingleplayerProxyConfigurations.PathToVersionFile));
        }

        static T TrySendUnityCommand<T>(ServiceUnityCommand command, TimeSpan timeout = default(TimeSpan))
        {
            if (timeout == default(TimeSpan))
                timeout = TimeSpan.FromSeconds(2);
            var task = Task.Run(async () =>
            {
                var client = new TcpClient();
                for (var i = 0; i < 20; i++)
                {
                    try
                    {
                        client.Connect(SingleplayerProxyConfigurations.UnityServiceEndPoint);
                    }
                    catch (SocketException)
                    {
                        continue;
                    }

                    break;
                }

                await client.WriteJsonAsync(command);

                return await client.ReadJsonAsync<T>();
            }).ContinueWith(t => t.IsFaulted ? default(T) : t.Result);

            return task.Wait(timeout) ? task.Result : default(T);
        }
    }
}