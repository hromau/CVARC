
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CVARC.V2
{


    public abstract class World<TWorldState> : IWorld
        //where TWorldManager : IWorldManager
        where TWorldState : IWorldState
    {
        List<IActor> actors;
        public bool DebugMode { get; protected set; }
        List<IEngine> Engines { get; set; }

        public IdGenerator IdGenerator { get; private set; }
        public WorldClocks Clocks { get; private set; }
        public Scores Scores { get; private set; }
        public Logger Logger { get; private set; }
        public Configuration Configuration { get; private set; }
        public Competitions Competitions { get; private set; }
        public TWorldState WorldState { get; private set; }
		IWorldState IWorld.WorldState { get { return WorldState;  } }
        public IKeyboard Keyboard { get; private set; }
        public List<string> LoggingPositionObjectIds { get; private set; }
        public abstract void CreateWorld();

        public void OnExit()
        {
            if (Exit != null) Exit();
        }

        public event Action Exit;

        public IEnumerable<IActor> Actors
        {
            get { return actors; }
        }

        public TEngine GetEngine<TEngine>()
            where TEngine : IEngine
        {
            return (TEngine)Engines.Where(e => e is TEngine).First();
        }

        public virtual void AdditionalInitialization()
        {
        }


        public void Initialize(Competitions competitions, Configuration configuration, ControllerFactory controllerFactory, IWorldState worldState)
        {
            Debugger.Log(DebuggerMessageType.Initialization, "World initialization");
            Debugger.Log(DebuggerMessageType.Initialization, "Starting basic fields");

            Competitions = competitions;
            Configuration = configuration;
            WorldState = Compatibility.Check<TWorldState>(this, worldState);

            Clocks = new WorldClocks();
            IdGenerator = new IdGenerator();
            Scores = new Scores(this);
            Logger = new Logger(this);
            Keyboard = competitions.KeyboardFactory();
            LoggingPositionObjectIds = new List<string>();

            // setting up the parameters
            Logger.SaveLog = Configuration.Settings.EnableLog;

            Logger.LogFileName = Configuration.Settings.LogFile;
            Logger.Log.Configuration = Configuration;
            Logger.Log.WorldState = WorldState;

            Clocks.TimeLimit = Configuration.Settings.TimeLimit;


            Debugger.Log(DebuggerMessageType.Initialization, "About to init engines");
            //Initializing world
            this.Engines = competitions.EnginesFactory();
            Debugger.Log(DebuggerMessageType.Initialization, "Init engines OK");

            Debugger.Log(DebuggerMessageType.Initialization, "Complete: basic fields. Starting engine");
            GetEngine<ICommonEngine>().Initialize(this);
            Debugger.Log(DebuggerMessageType.Initialization, "Complete: engine. Starting controller factory");
            controllerFactory.Initialize(this);
            Debugger.Log(DebuggerMessageType.Initialization, "Complete: controller factory. Creating world");
            CreateWorld();
            Debugger.Log(DebuggerMessageType.Initialization, "World created");
            

            //Initializing actors
            actors = new List<IActor>();
            foreach (var id in competitions.Logic.Actors.Keys)
            {
                Logger.AddId(id);
                InitializeActor(
                    competitions,
                    id,
                    competitions.Logic.Actors[id],
                    controllerFactory.Create
                    );
            }

            foreach (var l in competitions.Logic.NPC)
            {
                var f = l.Item3;
                InitializeActor(competitions, l.Item1, l.Item2, (cid, a) => f(a));
                  
            }

            Debugger.Log(DebuggerMessageType.Initialization, "Additional world initialization");
            AdditionalInitialization();
        }


        void InitializeActor(Competitions competitions, string id, ActorFactory factory, Func<string,IActor,IController> controllerFactory )
        {
            Debugger.Log(DebuggerMessageType.Initialization, "Actor " + id + " initialization");
            Debugger.Log(DebuggerMessageType.Initialization, "Creating actor");
            //var factory = competitions.Logic.Actors[id];
            var e = factory.CreateActor();
            var actorObjectId = IdGenerator.CreateNewId(e);
            Debugger.Log(DebuggerMessageType.Initialization, "Complete: actor. Creating manager");
            //var manager = competitions.Manager.CreateActorManagerFor(e);
            var rules = factory.CreateRules();
            var preprocessor = factory.CreateCommandFilterSet();
            e.Initialize(/*manager, */this, rules, preprocessor, actorObjectId, id);
            Debugger.Log(DebuggerMessageType.Initialization, "Comlete: manager creation. Initializing manager");
            Compatibility.Check<IActor>(this, e);
            Debugger.Log(DebuggerMessageType.Initialization, "Comlete: manager initialization. Creating actor body");

            Debugger.Log(DebuggerMessageType.Initialization, "Complete: body. Starting controller");

            var controller = controllerFactory(e.ControllerId, e);
            controller.Initialize(e);

            Clocks.AddTrigger(new ControlTrigger(controller, e, preprocessor));
            actors.Add(e);
            Debugger.Log(DebuggerMessageType.Initialization, "Actor " + id + " is initialized");
        }


    }
}
