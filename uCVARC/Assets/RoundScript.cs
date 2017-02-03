using System;
using System.Linq;
using UnityEngine;
using Assets;
using CVARC.V2;
using AIRLab;
using Infrastructure;
using UnityCommons;

public partial class RoundScript : PlayScript
{
    public static Tuple<string, string, int> CollisionInfo { get; set; }
    IWorld world;
    float curWorldTime;
    float timeOnStartSession;
    private long lastStart;
    bool gameOver;
    private double timeLimit = 0; // in seconds

    protected override void Initialization()
    {
        Dispatcher.RoundStart();

        timeOnStartSession = Time.fixedTime;
        curWorldTime = 0;

        world = Dispatcher.CurrentWorld;
        timeLimit = Dispatcher.CurrentWorld.Configuration.TimeLimit;
        //Debugger.Log(DebuggerMessageType.Unity,timeLimit);
        if (world != null)
            Debugger.Log("World loaded");
        else
            Debugger.Log("Fail. World not loaded");

        CollisionInfo = new Tuple<string, string, int>(null, null, 0);
        //Time.timeScale = 1; // переехало в диспатчер
        gameOver = false;
    }

    void Update()
    {
        Dispatcher.RoundTick();

        if (gameOver)
            return;

        if (curWorldTime > timeLimit)
        {
            Debugger.Log("Time is Up");
            Dispatcher.SetGameOver();
            gameOver = true;
            return;
        }

        if (CollisionInfo.Item3 == 2)
        {
            ((CommonEngine)world.GetEngine<ICommonEngine>()).CollisionSender(CollisionInfo.Item1, CollisionInfo.Item2);
            CollisionInfo.Item3 = 0;
        }

        foreach (var player in world.Scores.GetAllScores())
        {
            if (player.Item1 == "Left")
                scoresTextLeft.text = "Left Scores: " + player.Item2;
            if (player.Item1 == "Right")
                scoresTextRight.text = "Right Scores: " + player.Item2;
        }
    }

    void FixedUpdate() //только физика и строгие расчеты. вызывается строго каждые 20 мс
    {
        var engine = world.GetEngine<ICommonEngine>();
        curWorldTime = Time.fixedTime - timeOnStartSession;

        world.Clocks.Tick(curWorldTime);
        ((CommonEngine)world.GetEngine<ICommonEngine>()).UpdateSpeeds();
    }

    void OnDisable()
    {
        Dispatcher.OnDispose();
    }

    void OnGUI()
    {
        var rect = new Rect(new Vector2(10, 20), new Vector2(100, 30));
        switch (Event.current.type)
        {
            case EventType.MouseUp:
                if (rect.Contains(Event.current.mousePosition))
                    Dispatcher.SetGameOver();
                break;
            case EventType.Repaint:
                GUI.DrawTexture(rect, button);
                var col = GUI.color;
                GUI.color = Color.white;
                GUI.Label(rect, "Back to menu");
                GUI.color = col;
                break;
        }
    }

    public Texture button;
}