using AIRLab.Mathematics;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CVARC.V2
{

    public enum GameLogEntryType
    {
        IncomingCommand,
        EngineInvocation,
        LocationCorrection,
        ScoresUpdate
    }

    public class IncomingCommandLogEntry
    {
        public string ControllerId { get; set; }
        public JObject Command { get; set; }
    }

    public class EngineInvocationLogEntry
    {
        public string EngineName { get; set; }
        public string MethodName { get; set; }
        public string[] Arguments { get; set; }
    }

    public class LocationCorrectionLogEntry
    {
        public Dictionary<string, Frame3D> Locations { get; set; }
    }

    public class ScoresUpdate
    {
        public int Added { get; internal set; }
        public string ControllerId { get; set; }
        public string Reason { get; internal set; }
        public int Scores { get; set; }
        public object Total { get; internal set; }
        public string Type { get; internal set; }
    }

    public class GameLogEntry
    {
        public double Time { get; set; }
        public GameLogEntryType Type { get; set; }
        public IncomingCommandLogEntry IncomingCommand { get; set; }
        public EngineInvocationLogEntry EngineInvocation { get; set; }
        public LocationCorrectionLogEntry LocationCorrection { get; set; }
        public ScoresUpdate ScoresUpdate { get; set; }
    }
}