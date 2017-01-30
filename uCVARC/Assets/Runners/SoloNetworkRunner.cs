using CVARC.V2;
using UnityEngine;


namespace Assets
{
    public class SoloNetworkRunner : IRunner
    {
        readonly CvarcClient client;
        readonly Configuration configuration;
        readonly ControllerFactory factory;
        readonly IWorldState worldState;

        public string Name { get; private set; }
        public IWorld World { get; private set; }
        public bool CanStart { get; private set; }
        public bool CanInterrupt { get; private set; }
        public bool Disposed { get; private set; }

        public SoloNetworkRunner(CvarcClient client)
        {
            Debugger.Log(DebuggerMessageType.Unity, "Runner setup: started");
            this.client = client;
            Debugger.Log(DebuggerMessageType.Unity, "Runner setup: client loaded");
            factory = new SoloNetworkControllerFactory(client);
            Debugger.Log(DebuggerMessageType.Unity, "Runner setup: factory done");
            var configProposal = client.Read<ConfigurationProposal>();
            Debugger.Log(DebuggerMessageType.Unity, "Runner setup: configProposal has been read");
            var loadingData = configProposal.LoadingData;
            var competitions = Dispatcher.Loader.GetCompetitions(loadingData);
            var settings = competitions.Logic.CreateDefaultSettings();
            Debugger.Log(DebuggerMessageType.Unity, "Runner setup: settings set");
            if (configProposal.SettingsProposal != null)
                configProposal.SettingsProposal.Push(settings, true);
            configuration = new Configuration
            {
                LoadingData = loadingData,
                Settings = settings
            };
            Debugger.Log(DebuggerMessageType.Unity, "Runner setup: Config created");

            //configuration.Settings.EnableLog = true;
            //configuration.Settings.LogFile = UnityConstants.LogFolderRoot + "CvarcTestLog";

            var worldSettingsType = competitions.Logic.WorldStateType;
            worldState = (IWorldState)client.Read(worldSettingsType);
            Debugger.Log(DebuggerMessageType.Unity, "Runner setup: world state has been read");
            Name = loadingData.AssemblyName + loadingData.Level;
            CanInterrupt = true;
            CanStart = true;
            Debugger.Log(DebuggerMessageType.Unity, "Runner setup: done");
        }

        public void InitializeWorld()
        {
            if (configuration.Settings.SpeedUp)
            {
                Time.timeScale = 2;
                Debugger.Log(DebuggerMessageType.Unity, "LOL");
            }
            if (World == null)
                World = Dispatcher.Loader.CreateWorld(configuration, factory, worldState);
            
        }

        public void Dispose()
        {
            client.Close();
            if (configuration.Settings.SpeedUp)
                Time.timeScale = 1;
            if (World != null)
                World.OnExit();
            Disposed = true;
        }
    }
}
