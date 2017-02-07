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
    IWorld world;
    float curWorldTime;
    float timeOnStartSession;
    bool gameOver;
    private double timeLimit = 0; // in seconds

    protected override void Initialization()
    {
        Dispatcher.RoundStart();

        timeOnStartSession = Time.fixedTime;
        curWorldTime = 0;

        world = Dispatcher.CurrentWorld;
        timeLimit = Dispatcher.CurrentWorld.Configuration.TimeLimit;
        
        if (world != null)
            Debugger.Log("World loaded");
        else
            Debugger.Log("Fail. World not loaded");

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
        Debugger.Log("Entering fixed update");
        curWorldTime = Time.fixedTime - timeOnStartSession;
        world.Clocks.Tick(curWorldTime);
        Debugger.Log("Updating speeds");
        ((CommonEngine)world.GetEngine<ICommonEngine>()).UpdateSpeeds();
        Debugger.Log("Leaving fixed update");
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