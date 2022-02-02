using System;
using System.Collections.Generic;
using Infrastructure;

namespace CVARC.V2
{
    public class LogicPart
    {
   
        public readonly Dictionary<string, Func<IController>> Bots = new Dictionary<string, Func<IController>>();

        public readonly Dictionary<string, ActorFactory> Actors = new Dictionary<string, ActorFactory>();

        public readonly List<Infrastructure.Tuple<string, ActorFactory, Func<IActor, IController>>> NPC =
            new List<Infrastructure.Tuple<string, ActorFactory, Func<IActor, IController>>>();

        public readonly List<string> PredefinedWorldStates = new List<string>();

        public Func<string, WorldState> CreateWorldState { get; set; }

        public Func<Infrastructure.GameSettings> CreateDefaultSettings { get; set; }

        public Func<IWorld> CreateWorld { get; set; }

        public Type WorldStateType { get; set; }

        public LogicPart()
        {
        }
    }

    
}
