using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CVARC.V2;
using UnityEngine;
using Assets;
using Assets.Bundles;
using Assets.Servers;
using CVARC.Core;
using Pudge.RunningBinding;
using UnityEngineInternal;
using Settings = Assets.Bundles.Settings;


public static class Dispatcher
{
    public const int TimeScale = UnityConstants.TimeScale;
    public static Loader Loader { get; private set; }
    public static IRunner CurrentRunner { get; private set; }
    public static bool UnityShutdown { get; private set; }
    static NewLog currentLog;
    public static bool IsAssetBundleLoaded;
    public static NewLog CurrentLog
    {
        get
        {
            if (currentLog == null)
                currentLog = new NewLog();
            return currentLog;
        }
        set { currentLog = value; }
    }
    public static NewLog LoadedLog;
    static bool logPlayRequested;
    static readonly RunnersQueue queue = new RunnersQueue();
    static readonly Dictionary<string, GameObject> objectsCache = new Dictionary<string, GameObject>();
    static bool isGameOver;
    static UnityServer soloNetworkServer;
    static UnityServer torunamentServer;
    static UnityServer serviceServer;
    static UnityServer logServer;
    static UnityServer networkServer;
    static bool switchingScenes;


    public static void Start()
    {
        Time.timeScale = TimeScale;

        Debugger.Settings.EnableType(typeof(Dispatcher));
        
        //if (Debug.isDebugBuild)

            Debugger.Logger = Debug.Log;

        if (!Directory.Exists(UnityConstants.LogFolderRoot))
            Directory.CreateDirectory(UnityConstants.LogFolderRoot);
        

        Loader = new Loader();
        Debugger.Log("Loader ready. Starting: adding levels");
        Debugger.Log("=======================" + UriConstructor.GetUriFileLocationPath(Settings.CurrentBundle));

        
        //Loader.AddLevel("Demo", "Test", () => new DemoCompetitions.Level1());
        //Loader.AddLevel("RoboMovies", "Test", () => new RMCompetitions.Level1());
        //Loader.AddLevel("Pudge", "Level1", () => new PudgeCompetitions.Level1());
        //Debugger.Log(DebuggerMessageType.Initialization, "Lvl1 ready. Starting: lvl 2");
        //Loader.AddLevel("Pudge", "Level2", () => new PudgeCompetitions.Level2());
        //Debugger.Log(DebuggerMessageType.Initialization, "Lvl2 ready. Starting: lvl 3");
        //Loader.AddLevel("Pudge", "Level3", () => new PudgeCompetitions.Level3());
        //Debugger.Log(DebuggerMessageType.Initialization, "Lvl3 ready. Starting: lvl Test");
        //Loader.AddLevel("Pudge", "Test", () => new PudgeCompetitions.TestLevel());
        //Debugger.Log(DebuggerMessageType.Initialization, "LvlTest ready. All levels ready");
        //Loader.AddLevel("Demo", "Level1", () => new DemoCompetitions.Level1());
        //Debugger.Log(DebuggerMessageType.Initialization, "Demo Lvl1 ready");
        //Loader.AddLevel("TheBeachBots", "Test", () => new TBBCompetitions.Level1());

        if (UnityConstants.NeedToOpenSoloNetworkPort)
        {
            soloNetworkServer = new SoloNetworkServer(UnityConstants.SoloNetworkPort);
            Action soloNetworkServerAction = () => soloNetworkServer.StartThread();
            soloNetworkServerAction.BeginInvoke(null, null);
        }

        torunamentServer = new MultiplayerServer(UnityConstants.TournamentPort);
        Action tournamentServerAction = () => torunamentServer.StartThread();
        tournamentServerAction.BeginInvoke(null, null);

        networkServer = new NetworkServer(UnityConstants.NetworkPort);
        Action networkServerAction = () => networkServer.StartThread();
        networkServerAction.BeginInvoke(null, null);
        

        if (UnityConstants.NeedToOpenLogPort)
        {
            logServer = new LogServer(UnityConstants.LogPort);
            Action logServerAction = () => logServer.StartThread();
            logServerAction.BeginInvoke(null, null);
        }
    }

    public static void FillLoader()
    {
        var level = new BundleEntryPoint().GetLevels()
            .Where(x => x.LevelName == Assets.Bundles.Settings.CurrentLevel)
            .ToList()[0];

        Loader.AddLevel("Pudge", Assets.Bundles.Settings.CurrentLevel, () => level);
        IsAssetBundleLoaded = true;
    }

    public static void AddRunner(IRunner runner)
    {
        queue.EnqueueRunner(runner);
    }

    public static void IntroductionTick()
    {
        if (queue.HasReadyRunner())
            SwitchScene("Round");
        if (logPlayRequested)
        {
            logPlayRequested = false;
            SwitchScene("LogRound");
        }
    }

    public static void RoundStart()
    {
        CurrentLog = new NewLog();
        CurrentRunner = queue.DequeueReadyRunner();
        isGameOver = false;
        CurrentRunner.InitializeWorld();
        CurrentLog.EndSegment();

    }

    public static void RoundTick()
    {
        // конец игры
        if (isGameOver && CurrentRunner != null)
        {
            Debug.Log("game over. disposing");
            CurrentRunner.Dispose();
            CurrentRunner = null;
        }

        if (CurrentRunner == null)
        {
            // очищение, или переход в начало
            SwitchScene(queue.HasReadyRunner() ? "Round" : "Intro");
            return;
        }

        // прерывание
        if (queue.HasReadyRunner() && CurrentRunner.CanInterrupt)
            SetGameOver();
    }

    public static void RequestLogPlay()
    {
        logPlayRequested = true;
    }

    public static void LogEnd()
    {
        SwitchScene("Intro");
    }

    // самый ГЛОБАЛЬНЫЙ выход, из всей юнити. Вызывается из сцен.
    public static void OnDispose()
    {
        if (switchingScenes)
        {
            switchingScenes = false;
            return;
        }

        Debugger.Log("GLOBAL EXIT");
        if (soloNetworkServer != null)
            soloNetworkServer.RequestExit();
        torunamentServer.RequestExit();
        networkServer.RequestExit();
        if (serviceServer != null)
            serviceServer.RequestExit();
        if (logServer != null)
            logServer.RequestExit();
        if (CurrentRunner != null)
            CurrentRunner.Dispose();
        queue.DisposeRunners();
        UnityShutdown = true;
    }

    public static void SetGameOver()
    {
        isGameOver = true;
        if (CurrentRunner.World.Configuration.Settings.EnableLog)
            NewLogIO.Save(CurrentLog,CurrentRunner.World.Configuration.Settings.LogFile);
    }

    static void SwitchScene(string sceneName)
    {
        objectsCache.Clear();
        switchingScenes = true;
        Application.LoadLevel(sceneName);
    }

    public static GameObject FindGameObject(string id)
    {
        // тут вроде все как надо.
        // для оптимизации можно вызывать этот метод, но не обязательно.
        if (objectsCache.ContainsKey(id))
            if (objectsCache[id] == null || objectsCache[id].name != id) // у GameObject перегружен оператор == так, что он возвращает true в сравнении с null, если объект уничтожен GameObject.Destroy
                objectsCache.Remove(id);
            else
                return objectsCache[id];
        var obj = GameObject.Find(id);
        if (obj != null)
            objectsCache.Add(id, obj);
        return obj;
    }
}
