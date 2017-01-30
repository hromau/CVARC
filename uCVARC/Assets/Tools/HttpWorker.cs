using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using CVARC.V2;
using CVARC.Infrastructure;

namespace Assets.Tools
{
    public static class HttpWorker
    {
        public static void SendGameResultsIfNeed(PlayerInfo[] playersInfo, string logGuid)
        {
            var results = new GameResultInfo
            {
                LogFileName = logGuid,
                Players = playersInfo
            };
            if (UnityConstants.TournamentToolMode)
            {
                try
                {
                    var tcpClient = new TcpClient("127.0.0.1", UnityConstants.TournamentToolPort);
                    var stream = tcpClient.GetStream();
                    Serializer.Serialize(results, stream);
                }
                catch (Exception e)
                {
                    Debugger.Log(DebuggerMessageType.Error,
                    "LOCAL server error. Error message: " + e.Message);
                }
                return;
            }

            if (!WebInfo.NeedToSendToWeb || playersInfo.Any(i => !CheckForForbiddenSymbols(i.CvarcTag)))
                return;
            results.PushPassword = WebInfo.PasswordToWeb;
            results.Tag = "training";

            try
            {
                var request =
                    WebRequest.Create(string.Format("http://{0}:{1}/{2}", WebInfo.WebIp, WebInfo.WebPort, WebInfo.Method));
                request.Method = "POST";
                var ms = new MemoryStream();
                Serializer.Serialize(results, ms);
                var data = ms.ToArray();
                request.ContentLength = data.Length;
                request.ContentType = "application/json; charset=utf-8";
                request.GetRequestStream().Write(data, 0, data.Length);
                var responseString = Encoding.Default.GetString(request.GetResponse().GetResponseStream().ReadToEnd());
                if (!string.IsNullOrEmpty(responseString))
                    Debugger.Log(DebuggerMessageType.Unity, "Game result sent. answer: " + responseString);
            }
            catch (Exception e)
            {
                Debugger.Log(DebuggerMessageType.Unity, "Web server error. Error message: " + e.Message);
            }


            if (!WebInfo.NeedToSendToWeb)
                return;
            var nvc = new NameValueCollection {{"password", WebInfo.PasswordToWeb}};
            var answer = HttpUploadFile(string.Format(
                "http://{0}:{1}/{2}", WebInfo.WebIp, WebInfo.WebPort, WebInfo.LogMethod), 
                UnityConstants.LogFolderRoot + logGuid, "file", "multipart/form-data", nvc);

            if (answer != null)
                Debugger.Log(DebuggerMessageType.Unity, "Game log sent. answer: " + answer);

        }

        public static void SendInfoToLocal(string value)
        {
            try
            {
                var tcpClient = new TcpClient("127.0.0.1", UnityConstants.TournamentToolPort);
                
                var stream = tcpClient.GetStream();
                var bytesToSend = ASCIIEncoding.ASCII.GetBytes(value);
                stream.Write(bytesToSend, 0, bytesToSend.Length);
                Debugger.Log(DebuggerMessageType.Error,
                "LOCAL: player connected sent");
            }
            catch (Exception e)
            {
                Debugger.Log(DebuggerMessageType.Error,
                "LOCAL server error. Error message: " + e.Message);
            }
        }

        // сообщаем веб серверу о своем состоянии.
        public static void SayStatus(bool isReady)
        {
            if (!WebInfo.NeedToSendToWeb)
                return;
            var requestUri = string.Format(
                "http://{0}:{1}/{2}?password={3}&isOnline={4}",
                WebInfo.WebIp, WebInfo.WebPort, WebInfo.StatusMethod, WebInfo.PasswordToWeb, isReady);

            var responseString = SendDirectRequest(requestUri);
            if (responseString != null)
                Debugger.Log(DebuggerMessageType.Workflow, "Status sent. answer: " + responseString);
        }

        private static string SendDirectRequest(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            try
            {
                var response = request.GetResponse();
                return Encoding.Default.GetString(response.GetResponseStream().ReadToEnd());
            }
            catch (Exception e)
            {
                Debugger.Log(DebuggerMessageType.Error,
                    "Web server error. Error message: " + e.Message);
                return null;
            }
        }

        private static bool CheckForForbiddenSymbols(string value)
        {
            return value.All(ch => char.IsLetterOrDigit(ch) || ch == '-');
        }

        // это я скопировал со стек овер флоу. код не мой. заработало -- и ладно. не трогать, желательно. Рефакторить не хочу.
        private static string HttpUploadFile(string url, string file, string paramName, string contentType, NameValueCollection nvc)
        {
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
            wr.ContentType = "multipart/form-data; boundary=" + boundary;
            wr.Method = "POST";
            wr.KeepAlive = true;
            wr.Credentials = System.Net.CredentialCache.DefaultCredentials;
            Stream rs;
            try
            {
                 rs = wr.GetRequestStream();
            }
            catch (Exception e)
            {

                Debugger.Log(DebuggerMessageType.Error,
                    "Web server error. Error message: " + e.Message);
                return null;
            }
            string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
            foreach (string key in nvc.Keys)
            {
                rs.Write(boundarybytes, 0, boundarybytes.Length);
                string formitem = string.Format(formdataTemplate, key, nvc[key]);
                byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                rs.Write(formitembytes, 0, formitembytes.Length);
            }
            rs.Write(boundarybytes, 0, boundarybytes.Length);

            string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
            string header = string.Format(headerTemplate, paramName, file, contentType);
            byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
            rs.Write(headerbytes, 0, headerbytes.Length);

            FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[4096];
            int bytesRead = 0;
            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                rs.Write(buffer, 0, bytesRead);
            }
            fileStream.Close();

            byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            rs.Write(trailer, 0, trailer.Length);
            rs.Close();

            WebResponse wresp;
            try
            {
                wresp = wr.GetResponse();
            }
            catch (Exception e)
            {
                Debugger.Log(DebuggerMessageType.Error, 
                    "Web server error. Error message: " + e.Message);
                return null;
            }
            return Encoding.Default.GetString(wresp.GetResponseStream().ReadToEnd());
        }
    }

    public static class WebInfo
    {
        public static bool NeedToSendToWeb;
        public static string WebIp;
        public static int WebPort;
        public static string Method;
        public static string LogMethod;
        public static string StatusMethod;
        public static string PasswordToWeb; // top defence ever.

        public static void InitWebConfigsFromFile(string pathToConfigFile)
        {
            try
            {
                var lines = File.ReadAllLines(pathToConfigFile);
                var configDict = lines.ToDictionary(line => line.Split(':')[0].Trim(' '),
                    line => line.Split(':')[1].Trim(' '));
                WebIp = configDict["web_ip_or_address"];
                WebPort = int.Parse(configDict["web_port"]);
                Method = configDict["method"];
                LogMethod = configDict["log_method"];
                StatusMethod = configDict["status_method"];
                PasswordToWeb = configDict["secret_to_web"];
                NeedToSendToWeb = bool.Parse(configDict["need_to_communicate_with_web"]);
            }
            catch
            {
                Debugger.Log(DebuggerMessageType.Unity, "Unable to load web settings from file. Load default (off)");
                NeedToSendToWeb = false;
            }
        }
    }
}
