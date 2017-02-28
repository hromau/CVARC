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

    const int buttonWidth = 150;
    const int buttonHeight = 30;

    const int windowWidth = 400;
    const int windowHeight = 400;

    const int menuHeight = 200;
    const int menuWidth = buttonWidth;

    public Texture ButtonTexture;

    public void OnGUI()
    {


        var windowRect = new Rect((Screen.width - windowWidth) / 2, (Screen.height - windowHeight) / 2, windowWidth, windowHeight);

        if (!Dispatcher.isStarted)
        {
            GUI.Window(0, windowRect, _ => { }, "Loading...");
            return;
        }

        var competitionsName = Dlc.FullCompetitionsName[Settings.Current.TutorialCompetitions];
        var backgroundRect = new Rect(0, 0, Screen.width, Screen.height);

        GUI.DrawTexture(backgroundRect, Dlc.MenuBackgroundForCompetitions[Settings.Current.TutorialCompetitions], ScaleMode.ScaleAndCrop);
        GUI.Window(0, windowRect, _ => MainMenuWindow(windowRect), "CVARC - " + competitionsName);
    }

    void MainMenuWindow(Rect windowRect)
    {

        Rect menuRect = new Rect((windowRect.width - menuWidth) / 2, (windowRect.height - menuHeight) / 2, menuWidth, menuHeight);
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
