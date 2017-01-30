using CVARC.V2;
using System.Collections.Generic;
using System.Linq;

namespace Assets
{
    public class NetworkServer : UnityServer
    {
        public CvarcClient messagingClient { get; private set; }
        NetworkRunner runner;


        public NetworkServer(int port) : base(port) { }
       
        protected override void HandleClient(CvarcClient client)
        {
            Debugger.Log(DebuggerMessageType.Unity, "started handle client");
            if (runner == null)
                InitRunner(client);
            else
                runner.AddPlayer(new MultiplayingPlayer(
                    client, client.Read<ConfigurationProposal>().SettingsProposal.CvarcTag, runner.worldState)
                    );

            if (runner.CanStart)
            {
                Debugger.Log(DebuggerMessageType.Unity, "preparing runner start");
                runner.PrepareStart();
            }
        }

        private void InitRunner(CvarcClient client)
        {
            Debugger.Log(DebuggerMessageType.Unity, "started runner init");
            Debugger.Log(DebuggerMessageType.Unity, "reading loading data");
            var loadingData = client.Read<LoadingData>();
            Debugger.Log(DebuggerMessageType.Unity, "taking competitions from dispatcher");
            var competitions = Dispatcher.Loader.GetCompetitions(loadingData);
            var worldSettingsType = competitions.Logic.WorldStateType;
            Debugger.Log(DebuggerMessageType.Unity, "reading worldState");
            var worldState = (IWorldState)client.Read(worldSettingsType);
            Debugger.Log(DebuggerMessageType.Unity, "reading controller settings");
            var clientsData = client.Read<List<ControllerSettings>>();
            var settings = competitions.Logic.CreateDefaultSettings(); // таким образом игнорируются все настйроки пользователя сейчас.
            foreach (var e in clientsData)
                settings.Controllers.Add(e);

            var configuration = new Configuration
            {
                LoadingData = loadingData,
                Settings = settings
            };
            Debugger.Log(DebuggerMessageType.Unity, "all OK, creating runner");
            messagingClient = client;
            runner = new NetworkRunner(worldState, configuration, this);
            Dispatcher.AddRunner(runner);
        }

        public void CloseGame()
        {
            runner = null;
            messagingClient.Close();
        }

        protected override void Print(string str)
        {
            Debugger.Log(DebuggerMessageType.Unity, "Network server: " + str);
        }
    }
}
