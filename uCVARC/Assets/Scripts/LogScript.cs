﻿using System;
using System.Linq;
using UnityEngine;
using Assets;
using CVARC.V2;
using AIRLab;
using Infrastructure;
using UnityCommons;
using System.IO;

public partial class LogScript : PlayScript
{
    private float timeOnStartSession;
    private float curWorldTime;
    LogPlayer reader;
    CommonEngine commonEngine;

    protected override void Initialization()
    {
        scoreProvider = new LogScoreProvider();
        reader = new LogPlayer(Dispatcher.LogModel, (LogScoreProvider)scoreProvider);

        var engines = Dispatcher.Loader
            .GetCompetitions(reader.GameSettings.LoadingData)
            .EnginesFactory(reader.GameSettings); // если я правильно понимаю, можно и так

        reader.StartEngines(engines);
        commonEngine = engines.OfType<CommonEngine>().SingleOrDefault();
        if (commonEngine == null)
            throw new Exception("No common engine is found in competitions. Replay is not possible");
        //var engines = Dispatcher.Loader.Levels["Pudge"]["Final"]().EnginesFactory();
        timeOnStartSession = Time.fixedTime;
        curWorldTime = 0;
    }

    void Update()
    {
        //этот метод нужно вызывать отсюда, потому что FixedUpdate вызывается еще много раз после SwitchScene, 
        //что повлечет за собой многократный вызов SwitchScene, что все поломает.
        Dispatcher.RoundTick();
        UpdateScores();
    }

    void FixedUpdate() //только физика и строгие расчеты. вызывается строго каждые 20 мс
    {
        curWorldTime = Time.fixedTime - timeOnStartSession;

        // если после того, как этот метод вернет фолс, позвать его еще раз -- вылетит исключение
        // может быть стоит сделать так, чтоб он всегда возвращал фолс. 
        // FixedUpdate может быть вызван еще несколько раз, несмотря на вызов SetGameOver.

        Debugger.Log("Calling reader.Play");
        if (!reader.Play(curWorldTime))
            Dispatcher.SetGameOver();

        Debugger.Log("Calling update speeds");
        commonEngine.UpdateSpeeds();

        Debugger.Log("Completed");
    }

    void OnDisable()
    {
        Dispatcher.OnDispose();
    }
}
