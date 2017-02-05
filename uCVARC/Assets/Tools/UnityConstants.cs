using System.Linq;
using CVARC.V2;

namespace Assets
{
    public static class UnityConstants
    {
        public const int ServicePort = 14002;
        public const int LogPort = 14003;
        public const int NetworkPort = 15000;
        public const int TimeScale = 1;
        public const string LogFolderRoot = "GameLogs/";

        public const bool NeedToOpenServicePort = true;
        // пользователи тру, сервер, тулза -- фолс.
        public const bool NeedToOpenLogPort = true;
    }
}
