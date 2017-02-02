using System;

namespace Infrastructure
{
    public class GameSettings
    {
        public LoadingData LoadingData { get; set; }
        public double TimeLimit { get; set; }
        public double OperationalTimeLimit { get; set; }
        public ActorSettings[] ActorSettings { get; set; }
    }
}
