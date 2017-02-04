using System;
using System.Collections.Generic;
using System.IO;
using CVARC.V2;
using UnityEngine;
using Assets;
using Assets.Tools;
using Infrastructure;
using UnityCommons;

public static class Dispatcher
{
    public const int TimeScale = UnityConstants.TimeScale;
    public static Loader Loader { get; private set; }
    public static GameManager GameManager { get; private set; }
    public static IWorld CurrentWorld { get; private set; }
    public static LogModel LogModel { get; set; }
    
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
