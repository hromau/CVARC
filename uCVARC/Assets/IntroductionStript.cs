using UnityEngine;
using System;
using Assets;
using Infrastructure;
using CVARC.V2;

public class IntroductionStript : MonoBehaviour
{
    static bool serverIsRunned = false;

    void Start()
    {
        if (serverIsRunned) return;
        Dispatcher.Start();
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

    const float
        kMenuWidth = 400.0f, // ширина меню то, куда кнопочки натыканы
        kMenuHeight = 241.0f,
        kButtonWidth = 175.0f,
        kButtonHeight = 30.0f;

    public Texture menuBackground, button;

    public void OnGUI()
    {
        GUI.Window(
            0,
            new Rect(
                (Screen.width - kMenuWidth) * 0.5f,
                (Screen.height - kMenuHeight) * 0.5f,
                kMenuWidth,
                kMenuHeight),
            TestsWindow,
            "CVARC Pudge Wars");
    }

    void TestsWindow(int windowID)
    {
        Rect menuRect = new Rect(
            10,
            25,
            kMenuWidth - 20,
            kMenuHeight - 35
        );

        GUI.DrawTexture(menuRect, menuBackground);

        LoadingData data = new LoadingData();
        data.AssemblyName = Settings.Current.TutorialCompetitions;
        data.Level = Settings.Current.TutorialLevel;
        
        GUILayout.BeginArea(menuRect);

        MenuButton(button, "Tutorial", Color.white, () => Dispatcher.GameManager.RequestTutorial(data));
        MenuButton(button, "LOG!", Color.white, () =>
        {
            Dispatcher.LogModel = new LogModel
            {
                LoadingData = new LoadingData
                {
                    AssemblyName = Settings.Current.TutorialCompetitions,
                    Level = Settings.Current.TutorialLevel
                },
                LogFile = Constants.LogFolderRoot + "\\play" + LogNames.Extension
            };
            Dispatcher.SwitchScene("LogRound");
        });
        
        GUILayout.EndArea();
    }

    public static void MenuButton(Texture icon, string text, Color color, Action pressAction)
    {
        Rect rect = GUILayoutUtility.GetRect(kButtonWidth, kButtonHeight, GUILayout.Width(kButtonWidth), GUILayout.Height(kButtonHeight));
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
        Rect rect = GUILayoutUtility.GetRect(kButtonWidth, kButtonHeight, GUILayout.Width(kButtonWidth), GUILayout.Height(kButtonHeight));
        return GUI.TextField(rect, startText);
    }
}
