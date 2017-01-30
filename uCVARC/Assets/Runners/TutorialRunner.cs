using System;
using CVARC.V2;


namespace Assets
{
    public class TutorialRunner : IRunner
    {
        readonly ControllerFactory factory;
        readonly Configuration configuration;
        readonly IWorldState worldState;

        public IWorld World { get; private set; }
        public string Name { get; private set; }
        public bool CanStart { get; private set; }
        public bool CanInterrupt { get; private set; }
        public bool Disposed { get; private set; }


        public TutorialRunner(LoadingData loadingData, Configuration configuration = null, IWorldState worldState = null)
        {
            factory = new TutorialControllerFactory();

            var competitions = Dispatcher.Loader.GetCompetitions(loadingData);
            if (configuration == null)
            {
                this.configuration = new Configuration
                {
                    LoadingData = loadingData,
                    Settings = competitions.Logic.CreateDefaultSettings()
                };
            }
            else
                this.configuration = configuration;

            this.configuration.Settings.EnableLog = true;
            this.configuration.Settings.LogFile = UnityConstants.LogFolderRoot + "tuto" + Guid.NewGuid() + ".cvarclog";

            this.worldState = worldState ?? competitions.Logic.CreateWorldState(competitions.Logic.PredefinedWorldStates[0]);
            //this.worldState = worldState ?? competitions.Logic.CreateWorldState("0"); // lol

            Name = "Tutorial";
            CanStart = true;
            CanInterrupt = true;
        }

        public void InitializeWorld()
        {
            if (World == null)
                World = Dispatcher.Loader.CreateWorld(configuration, factory, worldState);
        }

        public void Dispose()
        {
            if (World != null)
                World.OnExit();
            Disposed = true;
        }
    }
}
