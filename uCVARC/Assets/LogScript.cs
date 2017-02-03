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
        var data = File.ReadAllLines(UnityConstants.LogFolderRoot+"\\playlog.cvarclog");
        var engines = Dispatcher.Loader.Levels["Pudge"]["Final"]().EnginesFactory();
        reader = new LogPlayer(data, engines);
        timeOnStartSession = Time.fixedTime;
        curWorldTime = 0;
    }

    void Update()
    {

    }

    void FixedUpdate() //только физика и строгие расчеты. вызывается строго каждые 20 мс
    {
        curWorldTime = Time.fixedTime - timeOnStartSession;
        if (!reader.Play(curWorldTime))
        {
            //все закончилось
        }

    }

    void OnDisable()
    {
        Dispatcher.OnDispose();
    }
}
