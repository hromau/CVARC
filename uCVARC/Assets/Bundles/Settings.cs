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
    [Serializable]
    public class Settings
    {
        public string CurrentBundle { get; private set; }
        public string CurrentLevel { get; private set; }
        public List<string> DebugTypes = new List<string>();

        private Settings()
        {
            CurrentBundle = "pudge";
            CurrentLevel = "Level1";
            DebugTypes = new List<string> { "XXX" };
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
                SaveSettingsJson(settings);
            }

            return settings;
        }
    }


}
