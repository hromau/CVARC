using CVARC.V2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using Infrastructure;


[Serializable]
public class Settings
{
    public string TutorialCompetitions { get; set; }
    public string TutorialLevel { get; set; }
    public List<string> DlcBundles { get; set; }
    public List<string> DlcAssemblies { get; set; }
    public List<string> DebugTypes { get; set; }

    static string defaultDlcBundle = "pudge";
    static string defaultDlcAssembly = "Pudge.dll";
    static string defaultDebugType = "XXX";

    private Settings()
    {
        TutorialCompetitions = "Pudge";
        TutorialLevel = "Level2";
        DlcBundles = new List<string>();
        DlcAssemblies = new List<string>();
        DebugTypes = new List<string>();
    }

    public static readonly Settings Current;

    static Settings()
    {
        // загрузка settings.json перед всей дальнейшей работой
        Current = TryLoadSettingsJson();
    }

    private static Settings LoadSettingsJson()
    {
        return Serializer.Deserialize<Settings>(File.ReadAllText(Constants.PathToSettingsJson));
    }

    private static void SaveSettingsJson(Settings settings)
    {
        File.WriteAllText(Constants.PathToSettingsJson, Serializer.Serialize(settings));
    }

    private static Settings TryLoadSettingsJson()
    {
        Settings settings;

        try
        {
            settings = LoadSettingsJson();
        }
        catch (FileNotFoundException)
        {
            settings = new Settings();

            settings.DebugTypes.Add(defaultDebugType);
            settings.DlcAssemblies.Add(defaultDlcAssembly);
            settings.DlcBundles.Add(defaultDlcBundle);

            SaveSettingsJson(settings);
        }

        return settings;
    }
}
