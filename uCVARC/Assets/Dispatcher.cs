using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CVARC.V2;
using UnityEngine;
using Assets;
using Assets.Dlc;
using Assets.Servers;
using Assets.Tools;
using CVARC.Core;
using Infrastructure;
using UnityEngineInternal;
using UnityCommons;

public static class Dispatcher
{
    public const int TimeScale = UnityConstants.TimeScale;
    public static Loader Loader { get; private set; }
    public static GameManager GameManager { get; private set; }
    public static IWorld CurrentWorld { get; private set; }
    public static LogModel LogModel { get; set; }

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
    //    Debugger.AlwaysOn = true;

        if (!Directory.Exists(UnityConstants.LogFolderRoot))
            Directory.CreateDirectory(UnityConstants.LogFolderRoot);
        

        Loader = new Loader();
        Debugger.Log("Loader ready. Starting: adding levels");
        Debugger.Log("======================= Tutorial competition:" + Settings.Current.TutorialCompetitions);

        GameManager = new GameManager();
    }

    public static void FillLoader(IDlcEntryPoint entryPoint)
    {
        foreach (var level in entryPoint.GetLevels())
        {
            Loader.AddLevel(level.CompetitionsName, level.LevelName, () => level);
            Debugger.Log(level.CompetitionsName + " " + level.LevelName + " is loaded");
        }
    }

    public static void IntroductionTick()
    {
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
    {;
        isGameOver = false;
        CurrentWorld = GameManager.StartGame();
    }

    public static void RoundTick()
    {
        if (GameManager.CheckGame() != RunType.NotReady)
            SetGameOver();

        if (isGameOver)
        {
            Debug.Log("game over. disposing");
            GameManager.EndGame(new GameResult());
            LogModel = null;
            SwitchScene("Intro");
        }
    }

    // самый ГЛОБАЛЬНЫЙ выход, из всей юнити. Вызывается из сцен.
    public static void OnDispose()
    {
        if (CurrentWorld != null)
            CurrentWorld.OnExit();
        if (switchingScenes)
        {
            switchingScenes = false;
            return;
        }

        Debugger.Log("GLOBAL EXIT");
        GameManager.Dispose();
    }

    public static void SetGameOver()
    {
        isGameOver = true;
    }

    public static void SwitchScene(string sceneName) // public очень плохо
    {
        switchingScenes = true;
        Application.LoadLevel(sceneName);
    }
   
}
