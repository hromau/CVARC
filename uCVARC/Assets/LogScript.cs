using System;
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

    protected override void Initialization()
    {
        var model = Dispatcher.LogModel; // теперь всю инфу о логах мы получаем ТАК.

        var data = File.ReadAllLines(model.LogFile);
        var engines = Dispatcher.Loader.GetCompetitions(model.LoadingData).EnginesFactory(); // если я правильно понимаю, можно и так
        //var engines = Dispatcher.Loader.Levels["Pudge"]["Final"]().EnginesFactory();
        reader = new LogPlayer(data, engines);
        timeOnStartSession = Time.fixedTime;
        curWorldTime = 0;
    }

    void Update()
    {
        //этот метод нужно вызывать отсюда, потому что FixedUpdate вызывается еще много раз после SwitchScene, 
        //что повлечет за собой многократный вызов SwitchScene, что все поломает.
        Dispatcher.RoundTick();
    }

    void FixedUpdate() //только физика и строгие расчеты. вызывается строго каждые 20 мс
    {
        curWorldTime = Time.fixedTime - timeOnStartSession;

        // если после того, как этот метод вернет фолс, позвать его еще раз -- вылетит исключение
        // может быть стоит сделать так, чтоб он всегда возвращал фолс. 
        // FixedUpdate может быть вызван еще несколько раз, несмотря на вызов SetGameOver.
        if (!reader.Play(curWorldTime))
            Dispatcher.SetGameOver();
    }

    void OnDisable()
    {
        Dispatcher.OnDispose();
    }
}
