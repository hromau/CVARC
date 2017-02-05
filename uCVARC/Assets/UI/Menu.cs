using System;
using System.Linq;
using Assets;
using CVARC.V2;
using Infrastructure;
using UnityEngine;
using UI;
using Canvas = UI.Canvas;
using Button = UI.Button;

public class Menu : MonoBehaviour
{
    const string AssemblyName = "Pudge";
    const string Level = "Level3";

    private bool openWindowTests = false;
    static bool serverIsRunned = false;
    static string[] tests;
    static DateTime startedAt;

    public Sprite Sprite;
    public Sprite Avatar;

    const float
       kMenuWidth = 400.0f, // ширина меню то, куда кнопочки натыканы
       kMenuHeight = 241.0f,
       kButtonWidth = 175.0f,
       kButtonHeight = 30.0f;

    public Sprite MenuBackground, SpriteButton;
    private Sprite Background; //то, что будет на заднем фоне
    private int competitionIndex;
    private bool isPressedTests = false;
    public string logFile;


    // Use this for initialization
    void Start()
    {
        if (!serverIsRunned)
        {
            Server();
            serverIsRunned = true;
        }
        LoadingData data = new LoadingData();
        data.AssemblyName = AssemblyName;
        data.Level = Level;

        Element.InitUi();
        var c = new Canvas(new Vector2(100, 100));
        var w = new Window(Sprite, "CVARC Pudge Wars", new Rect(0, 0, kMenuWidth, kMenuHeight));
        var buttonOpenTutorial = new Button(
            () => Dispatcher.GameManager.RequestTutorial(data), SpriteButton, new Rect(50, 50, kButtonWidth, kButtonHeight), "Tutorial");
        w.AddElement(buttonOpenTutorial);
        c.AddElement(w.Head);
    }

    void Update()
    {
        Dispatcher.IntroductionTick();
    }

    void Server()
    {
        startedAt = DateTime.Now;
        Dispatcher.Start();
        
    }

    void OnDisable(){
        Dispatcher.OnDispose();
    }
}
