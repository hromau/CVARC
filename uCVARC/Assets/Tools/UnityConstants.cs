using System.Linq;
using CVARC.V2;

namespace Assets
{
    public static class UnityConstants
    {
        public const int SoloNetworkPort = 14000;
        public const int TournamentPort = 14001;
        public const int ServicePort = 14002;
        public const int LogPort = 14003;
        public const int NetworkPort = 15000;
        public const int TimeScale = 1;
        public const string LogFolderRoot = "GameLogs/";
        public const string PathToConfigFile = "../Polygon/config&key.txt";
        public const string AlternativeConfigPath = "config&key.txt";
        
        public const bool NeedToOpenServicePort = true;

        // так же как предыдущей. всегда тру кроме сервера.
        public const bool NeedToOpenSoloNetworkPort = true;

        // пользователи тру, сервер, тулза -- фолс.
        public const bool NeedToOpenLogPort = true;

        // в пользовательском билде и на сервере должно быть фолс
        // тру только при запуске тулзы-стравливалки
        public const bool TournamentToolMode = false;
        public const int TournamentToolPort = 14500;

        //Tests, Log play...
        public const bool ShowDevelopmentButtons = true;

        // записывать в файл позиции каждого IActor
        public const bool NeedToWritePositionsFromLogs = false;
        public const string PathToFileWithPositions = "../Polygon/difference.txt";

        // опция-костыль для сервера. перезагружает каждые ReloadingTime секунд.
        // ТОЛЬКО на сервере тру.
        public const bool Reloading = false;
        public const int ReloadingTime = 5*60*60;
    }
}
