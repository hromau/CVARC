using CVARC.V2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using Infrastructure;
using Newtonsoft.Json;

[Serializable]
public class Settings
{
    public string TutorialCompetitions { get; set; }
    public string TutorialLevel { get; set; }
    public List<string> DlcBundles { get; set; }
    public List<string> DlcAssemblies { get; set; }
    public int Version { get; set; }

    const int DueVersion = 2;

    public DebuggerConfig Debugging { get; set; }


    private Settings()
    {
    }

    private Settings(bool def)
     {
        TutorialCompetitions = "Homm";
        TutorialLevel = "Level1";
        DlcBundles = new List<string>() { "pudge", "homm" };
        DlcAssemblies = new List<string>() { "Pudge.dll", "HoMM.dll" };
        Debugging = new DebuggerConfig()
        {
            AlwaysOff = false,
            AlwaysOn = false,
            CallStackRoots = new List<MethodName>
               {
                   new MethodName
                   {
                        Type="ExampleType",
                         Method="ExampleMethod"
                   }
               },
            EnabledNamespaces = new List<string> { "ExampleNamespace" },
            EnabledTypes = new List<string> { "ExampleType" }
        };
        Version = DueVersion;

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
        File.WriteAllText(Constants.PathToSettingsJson, JsonConvert.SerializeObject(settings, Formatting.Indented));
    }

    private static Settings TryLoadSettingsJson()
    {
        Settings settings;

        try
        {
            settings = LoadSettingsJson();
            Debug.Log("Loaded settings, version=" + settings.Version);
            if (settings.Version != DueVersion)
                throw new Exception("Wrong version");
        }
        catch
        {
            settings = new Settings(true);
            SaveSettingsJson(settings);
        }

        return settings;
    }
}
