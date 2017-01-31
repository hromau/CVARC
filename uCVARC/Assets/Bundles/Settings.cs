using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Infrastructure;

namespace Assets.Bundles
{
    public static class Settings
    {
        public static string CurrentBundle { get; private set; } // lowerCase, bundle file name 
        public static string CurrentLevel { get; private set; } // competitions name, template is "Level{0}" by default

        // class names in the bundle
        // required to be loaded !!!
        public static string BundleEntryPointClassName { get; private set; }
        public static string PudgeCoreClassName { get; private set; }

        private static SettingsPreloader SettinsPreloader { get; set; }

        static Settings()
        {
            // загрузка settings.json перед всей дальнейшей работой
            SettinsPreloader = LoadSettingsJson();

            CurrentBundle = SettinsPreloader.CurrentBundle;
            CurrentLevel = SettinsPreloader.CurrentLevel;
            BundleEntryPointClassName = SettinsPreloader.BundleEntryPointClassName;
            PudgeCoreClassName = SettinsPreloader.PudgeCoreClassName;
        }

        private static SettingsPreloader LoadSettingsJson()
        {
            return Serializer.Deserialize<SettingsPreloader>(File.ReadAllText(Constants.PathToSettingsJson));
        }
    }

    [Serializable]
    public class SettingsPreloader
    {
        public string CurrentBundle;
        public string CurrentLevel;

        public string BundleEntryPointClassName;
        public string PudgeCoreClassName;
    }
}
