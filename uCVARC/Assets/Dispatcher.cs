using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CVARC.V2;
using UnityEngine;
using Assets;
using Assets.Dlc;
using Assets.Servers;
using CVARC.Core;
using Infrastructure;
using UnityEngineInternal;
using UnityCommons;

public static class Dispatcher
{
    public const int TimeScale = UnityConstants.TimeScale;
    public static Loader Loader { get; private set; }
    public static GameManager GameManager { get; private set; }
    public static bool UnityShutdown { get; private set; }
    public static IWorld CurrentWorld { get; private set; }

    static bool logPlayRequested;
    static bool isGameOver;
    static bool switchingScenes;


    public static void Start()
    {
        Time.timeScale = TimeScale;

        Debug.Log("Logging types...");
        foreach(var e in Settings.Current.DebugTypes)
        {
            Debug.Log(e);
            Debugger.Settings.EnableType(e);
        }
        Debugger.Logger = Debug.Log;

        if (!Directory.Exists(UnityConstants.LogFolderRoot))
            Directory.CreateDirectory(UnityConstants.LogFolderRoot);
        

        Loader = new Loader();
        Debugger.Log("Loader ready. Starting: adding levels");
        Debugger.Log("======================= Tutorial competition:" + Settings.Current.TutorialCompetitions);

        GameManager = new GameManager();
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

      
    }

    public static void FillLoader(IDlcEntryPoint entryPoint)
    {
        foreach (var level in entryPoint.GetLevels())
        {
            Loader.AddLevel(level.CompetitionsName, level.LevelName, () => level);
        }
    }

    public static void IntroductionTick()
    {
        if (GameManager.CheckGame())
            SwitchScene("Round");
    }

    public static void RoundStart()
    {;
        isGameOver = false;
        CurrentWorld = GameManager.StartGame();
    }

    public static void RoundTick()
    {
        // конец игры
        if (isGameOver)
        {
            Debug.Log("game over. disposing");
            GameManager.EndGame(new GameResult());
            CurrentWorld = null;
            SwitchScene(GameManager.CheckGame() ? "Round" : "Intro");
        }

        // прерывание
        if (GameManager.CheckGame())
            SetGameOver();
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
        UnityShutdown = true;
    }

    public static void SetGameOver()
    {
        isGameOver = true;
    }

    static void SwitchScene(string sceneName)
    {
        switchingScenes = true;
        Application.LoadLevel(sceneName);
    }
   
}
