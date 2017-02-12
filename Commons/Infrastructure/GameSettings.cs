using System;
using System.Collections.Generic;

namespace Infrastructure
{
    public class GameSettings
    {
        public LoadingData LoadingData { get; set; }
        public double TimeLimit { get; set; }
        public double OperationalTimeLimit { get; set; }
        public List<ActorSettings> ActorSettings { get; set; }
        public bool EnableLog { get; set; }
        public string LogFile { get; set; }
        public bool SpeedUp { get; set; }
        public bool SpectacularView { get; set; }
    }
}
