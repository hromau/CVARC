using UnityEngine;
using System;
using Assets;
using Infrastructure;
using CVARC.V2;
using Assets.Dlc;

public class IntroductionStript : MonoBehaviour
{
    static bool serverIsRunned = false;

    void Start()
    {
        Application.targetFrameRate = 30;
        if (serverIsRunned) return;
        StartCoroutine(Dispatcher.Start(StartCoroutine));
        serverIsRunned = true;
    }

    void Update()
    {
        Dispatcher.IntroductionTick();
    }

    void OnDisable()
    {
        Dispatcher.OnDispose();
    }

    const float buttonWidth = 150.0f;
    const float buttonHeight = 30.0f;

    public Texture ButtonTexture;

    public void OnGUI()
    {
        var windowRect = new Rect(
            Screen.width * 0.2f, Screen.height * 0.2f,
            Screen.width * 0.6f, Screen.height * 0.6f
       );

        if (!Dispatcher.isStarted)
        {
            GUI.Window(0, windowRect, _ => { }, "Loading...");
            return;
        }

        var competitionsName = Dlc.FullCompetitionsName[Settings.Current.TutorialCompetitions];
        GUI.Window(0, windowRect, _ => MainMenuWindow(windowRect), "CVARC - " + competitionsName);
    }

    void MainMenuWindow(Rect windowRect)
    {
        var menuBackground = Dlc.MenuBackgroundForCompetitions[Settings.Current.TutorialCompetitions];

        Rect backgroundRect = new Rect(3, 17, windowRect.width-6, windowRect.height-20);
        GUI.DrawTexture(backgroundRect, menuBackground, ScaleMode.ScaleAndCrop);

        Rect menuRect = new Rect(50, 50, 200, 200);
        GUILayout.BeginArea(menuRect);

        MenuButton(ButtonTexture, "Tutorial", Color.white, () => Dispatcher.GameManager.RequestTutorial(new LoadingData
        {
            AssemblyName = Settings.Current.TutorialCompetitions,
            Level = Settings.Current.TutorialLevel
        }));

        GUILayout.EndArea();
    }

    public static void MenuButton(Texture icon, string text, Color color, Action pressAction)
    {
        Rect rect = GUILayoutUtility.GetRect(buttonWidth, buttonHeight, GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight));
        switch (Event.current.type)
        {
            case EventType.MouseUp:
                if (rect.Contains(Event.current.mousePosition))
                {
                    pressAction();
                }
                break;
            case EventType.Repaint:
                GUI.DrawTexture(rect, icon);
                var col = GUI.color;
                GUI.color = color;
                rect.position = new Vector2(rect.position.x + 10, rect.position.y); // все нормально. это GUI, сынок.
                GUI.Label(rect, text);
                GUI.color = col;
                break;
        }
    }

    public static string TextField(string startText)
    {
        Rect rect = GUILayoutUtility.GetRect(buttonWidth, buttonHeight, GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight));
        return GUI.TextField(rect, startText);
    }
}
