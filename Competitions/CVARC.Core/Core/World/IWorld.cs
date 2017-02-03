using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CVARC.V2
{
    public interface IWorld
    {
        bool DebugMode { get; }
        T GetEngine<T>() where T : IEngine;
        void Initialize(Competitions competitions, GameSettings configuration, ControllerFactory controllerFactory, WorldState state);
        WorldClocks Clocks { get; }
        IdGenerator IdGenerator { get; }
        Scores Scores { get; }
        IEnumerable<IActor> Actors { get; }
        void OnExit();
        event Action Exit;
        GameSettings Configuration { get; }
        Competitions Competitions { get; }
        IKeyboard Keyboard { get; }
        LogWriter Logger { get; }
        void CreateWorld();
        WorldState WorldState { get;  }
        List<string> LoggingPositionObjectIds { get; }
        double LoggingPositionTimeInterval { get; }
    }

}
