using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class Constants
{
    public const string PathToAssetBundles = "Dlc/Bundles";
    public const string PathToDlcAssemblies = "Dlc/Assemblies";
    public const string PathToSettingsJson = "settings.json";

    public const int ServicePort = 14002;
    public const int LogPort = 14003;
    public const int NetworkPort = 15000;
    public static readonly int TimeScale = Settings.Current.ServerBuild ? 3 : 1;
    public const string LogFolderRoot = "GameLogs";
}
