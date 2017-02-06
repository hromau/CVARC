using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CVARC.V2;
using Infrastructure;

namespace Assets.Tools
{
    public static class DefaultWorldInfoCreator
    {
        public static WorldState GetDefaultWorldState(LoadingData loadingData)
        {
            var competitions = Dispatcher.Loader.GetCompetitions(loadingData);
            return competitions.Logic.CreateWorldState(competitions.Logic.PredefinedWorldStates.First());
        }

        public static GameSettings GetDefaultGameSettings(LoadingData loadingData)
        {
            var settings = Dispatcher.Loader.GetCompetitions(loadingData).Logic.CreateDefaultSettings();
            settings.LoadingData = loadingData;
            settings.EnableLog = true;
            settings.LogFile = UnityConstants.LogFolderRoot+"\\"+Guid.NewGuid().ToString() + LogNames.Extension;

            return settings;
        }
    }
}

