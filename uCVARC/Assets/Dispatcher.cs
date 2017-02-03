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
            CurrentWorld.OnExit();
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

    public static void SwitchScene(string sceneName) // public очень плохо
    {
        switchingScenes = true;
        Application.LoadLevel(sceneName);
    }
   
}
