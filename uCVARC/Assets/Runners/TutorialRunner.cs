using System;
using CVARC.V2;
using Infrastructure;


namespace Assets
{
    public class TutorialRunner : IRunner
    {
        readonly ControllerFactory factory;
        readonly GameSettings configuration;
        readonly IWorldState worldState;

        public IWorld World { get; private set; }
        public string Name { get; private set; }
        public bool CanStart { get; private set; }
        public bool CanInterrupt { get; private set; }
        public bool Disposed { get; private set; }


        public TutorialRunner(LoadingData loadingData, GameSettings configuration = null, IWorldState worldState = null)
        {
            factory = new TutorialControllerFactory();

            var competitions = Dispatcher.Loader.GetCompetitions(loadingData);
            if (configuration == null)
            {
                this.configuration = competitions.Logic.CreateDefaultSettings();
                this.configuration.LoadingData = loadingData;
            }
            else
                this.configuration = configuration;

            this.configuration.EnableLog = true;
            this.configuration.LogFile = UnityConstants.LogFolderRoot + "tuto" + Guid.NewGuid() + ".cvarclog";

            this.worldState = worldState ?? competitions.Logic.CreateWorldState(competitions.Logic.PredefinedWorldStates[0]);
            

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
