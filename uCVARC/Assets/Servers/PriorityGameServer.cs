using CVARC.V2;

namespace Assets.Servers
{
    // config reading server
    class PriorityGameServer : UnityServer
    {
        
        public PriorityGameServer(int port) : base(port) { }

        protected override void HandleClient(CvarcClient client)
        {
            var config = client.Read<Configuration>();
            var loadingData = config.LoadingData;
            var competitions = Dispatcher.Loader.GetCompetitions(loadingData);
            var worldSettingsType = competitions.Logic.WorldStateType;
            var worldState = (IWorldState)client.Read(worldSettingsType);
            var settings = config.Settings;
            var configuration = new Configuration
            {
                LoadingData = loadingData,
                Settings = settings
            };
            MultiplayerPool.AddForceGame(worldState, configuration);
            client.Close();
        }

        protected override void Print(string str)
        {
            Debugger.Log(DebuggerMessageType.Unity, "Priority Game server: " + str);
        }
    }
}
