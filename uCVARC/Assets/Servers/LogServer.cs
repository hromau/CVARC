using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CVARC.V2;

namespace Assets.Servers
{
    public class LogServer : UnityServer
    {
        public LogServer(int port) : base(port) { }

        protected override void HandleClient(CVARC.V2.CvarcClient client)
        {
            var logFileName = "lastPlayedLog.cvarclog";
            var logFilePath = UnityConstants.LogFolderRoot + logFileName;
            if (File.Exists(logFilePath))
                File.Delete(logFilePath);
            using (var stream = client.client.GetStream())
            using (var output = File.Create(logFilePath))
            {
                var buffer = new byte[1024];
                int bytesRead;
                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                    output.Write(buffer, 0, bytesRead);
            }
            client.Close();

            //new logs
            Dispatcher.LoadedLog = NewLogIO.Load(logFilePath);
            Dispatcher.RequestLogPlay();

            //old logs
            //var log = Log.Load(UnityConstants.LogFolderRoot + logFileName);
            //Dispatcher.AddRunner(new LogRunner(log));
        }

        protected override void Print(string str)
        {
            Debugger.Log(DebuggerMessageType.Unity, "Log server: " + str);
        }
    }
}
