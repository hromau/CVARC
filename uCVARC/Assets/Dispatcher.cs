using System.IO;
using System.Threading;
using CVARC.V2;
using UnityEngine;
using Assets;
using Assets.Servers;
using Assets.Tools;
using Infrastructure;
using UnityCommons;

public static class Dispatcher
{
    public static Loader Loader { get; private set; }
    public static GameManager GameManager { get; private set; }
    public static IWorld CurrentWorld { get; private set; }
    public static string LogModel { get; set; }

    static ServiceServer serviceServer;
    
    static bool isGameOver;
    static bool switchingScenes;
    static bool shutdown;


    

    public static void Start()
    {


        Time.timeScale = Constants.TimeScale;
        Debugger.Config = Settings.Current.Debugging;
        Debugger.Logger += str =>
        {
            Debug.Log(str);
            File.AppendAllText("log.txt", str+"\n");
        };

        if (!Directory.Exists(Constants.LogFolderRoot))
            Directory.CreateDirectory(Constants.LogFolderRoot);

        Loader = new Loader();
        Debugger.Log("Loader ready. Starting: adding levels");
        Debugger.Log("======================= Tutorial competition:" + Settings.Current.TutorialCompetitions);

        GameManager = new GameManager();

        if (Constants.NeedToOpenServicePort)
        {
            serviceServer = new ServiceServer(Constants.ServicePort);
            new Thread(serviceServer.Work).Start();
        }
    }

    public static void FillLoader(IDlcEntryPoint entryPoint)
    {
        foreach (var level in entryPoint.GetLevels())
        {
            var currentLevel = level; // это здесь не просто так: http://stackoverflow.com/questions/14907987/access-to-foreach-variable-in-closure-warning
            Loader.AddLevel(level.CompetitionsName, level.LevelName, () => currentLevel);
            Debugger.Log(level.CompetitionsName + " " + level.LevelName + " is loaded");
        }
    }

    public static void IntroductionTick()
    {
        if (shutdown)
        {
            OnDispose();
            Application.Quit();
        }

        switch (GameManager.CheckGame())
        {
            case RunType.Play:
            case RunType.Tutorial:
                SwitchScene("Round");
                break;
            case RunType.Log:
                SwitchScene("LogRound");
                break;
        }
    }

    public static void RoundStart()
    {
        CurrentWorld = GameManager.StartGame();
    }

    public static void RoundTick()
    {
        if (shutdown)
        {
            OnDispose();
            Application.Quit();
        }

        if (GameManager.CheckGame() != RunType.NotReady)
            SetGameOver();

        if (!isGameOver)
            return;

        Debug.Log("game over. disposing");
        GameManager.EndGame(new GameResult());
        LogModel = null;
        SwitchScene("Intro");
    }

    // самый ГЛОБАЛЬНЫЙ выход, из всей юнити. Вызывается из сцен.
    public static void OnDispose()
    {
        isGameOver = false;
        if (CurrentWorld != null)
        {
            CurrentWorld.OnExit();
            CurrentWorld = null;
        }
        if (switchingScenes)
        {
            switchingScenes = false;
            return;
        }

        Debugger.Log("GLOBAL EXIT");
        GameManager.Dispose();
        if (serviceServer != null)
            serviceServer.Dispose();
    }

    public static void SetGameOver()
    {
        isGameOver = true;
    }

    public static void SetShutdown()
    {
        shutdown = true;
    }

    public static void SwitchScene(string sceneName) // public очень плохо
    {
        switchingScenes = true;
        Application.LoadLevel(sceneName);
    }

    public static void SetTimeScale(int timeScale)
    {
        Time.timeScale = timeScale;
    }
}
