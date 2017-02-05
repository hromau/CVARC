using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure;
using Ionic.Zip;
using ProxyCommon;

namespace SingleplayerProxy
{
    class Program
    {
        private static int currentVersion;

        static void Main(string[] args)
        {
            ReloadVersion();

            UpdateUnityIfNeeded();
            if (!IsUnityUp())
                StartUnity();

            var listener = new TcpListener(SingleplayerProxyConfigurations.ProxyEndPoint);
            listener.Start();

            var tasks = new[] { WaitUntillUnityClosed(), null};
            while (true)
            {
                tasks[1] = listener.AcceptTcpClientAsync();
                var index = Task.WaitAny(tasks);
                if (index == 0)
                {
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
            !string.IsNullOrEmpty(TrySendUnityCommand<string>(ServiceUnityCommand.Ping, TimeSpan.FromMilliseconds(500)));

        static void KillUnity() =>
            TrySendUnityCommand<string>(ServiceUnityCommand.Shutdown, TimeSpan.FromSeconds(5));

        static bool IsUpdateAvailable() =>
            SingleplayerProxyConfigurations.UpdateEnabled &&
            WebHelper.ReadFromUrlAsync<int>(SingleplayerProxyConfigurations.UrlToGetVersion).Result > currentVersion;

        static void StartUnity()
        {
            if (!SingleplayerProxyConfigurations.DebugMode)
                Process.Start(SingleplayerProxyConfigurations.UnityExePath);
            TrySendUnityCommand<string>(ServiceUnityCommand.Ping, TimeSpan.FromSeconds(8));
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
            var gameSettings = client.ReadJson<GameSettings>();
            // var worldState = client.ReadLine/ReadJObject()
            var mainConnection = ConnectToServer();
            mainConnection.WriteJson(gameSettings);
            //mainConnection.WriteLine/JObject(worldState)
            var server = ConnectToServer();
            Proxy.CreateChainAndStart(server, client);
            var resultTask = mainConnection.ReadJsonAsync<GameResult>();
            resultTask.ContinueWith(x =>
            {
                Console.WriteLine(x.IsFaulted ? "Cant get game results." : "Game result is " + x.Result.Hehmeh);
            });
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
                zip.ExtractAll(SingleplayerProxyConfigurations.UrlToUnityDir, ExtractExistingFileAction.OverwriteSilently);
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
                client.Connect(SingleplayerProxyConfigurations.UnityServiceEndPoint);
                await client.WriteJsonAsync(command);

                return await client.ReadJsonAsync<T>();
            });
            if (!task.Wait(timeout) || task.IsFaulted)
                return default(T);
            return task.Result;
        }
    }
}
