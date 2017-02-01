using CVARC.V2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using Infrastructure;


namespace Assets.Bundles
{
    public static class Settings
    {
        public static string CurrentBundle { get; private set; } // lowerCase, bundle file name 
        public static string CurrentLevel { get; private set; } // competitions name, template is "Level{0}" by default

        private static SettingsPreloader SettinsPreloader { get; set; }

        public static List<string> DebugTypes { get; private set; }

        static Settings()
        {
            // загрузка settings.json перед всей дальнейшей работой
            SettinsPreloader = TryLoadSettingsJson();

            CurrentBundle = SettinsPreloader.CurrentBundle;
            CurrentLevel = SettinsPreloader.CurrentLevel;
            DebugTypes = SettinsPreloader.DebugTypes;
        }

        private static SettingsPreloader LoadSettingsJson()
        {
            return Serializer.Deserialize<SettingsPreloader>(File.ReadAllText(Constants.PathToSettingsJson));
        }

        private static void SaveSettingsJson(SettingsPreloader settings)
        {
            File.WriteAllText(Constants.PathToSettingsJson, Serializer.Serialize(settings));
        }

        private static SettingsPreloader TryLoadSettingsJson()
        {
            SettingsPreloader settings;

            try
            {
                settings = LoadSettingsJson();
            }
            catch(FileNotFoundException)
            {
                settings = new SettingsPreloader();
                SaveSettingsJson(settings);
            }

            return settings;
        }
    }

    [Serializable]
    public class SettingsPreloader
    {
        public string CurrentBundle = "pudge";
        public string CurrentLevel = "Level1";
        public List<string> DebugTypes = new List<string>();
    }
}
