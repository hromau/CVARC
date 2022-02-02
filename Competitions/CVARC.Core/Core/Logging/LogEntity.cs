

using System.Collections.Generic;
using AIRLab.Mathematics;
using Newtonsoft.Json.Linq;

namespace CVARC.V2
{

    public enum GameLogEntryType
    {
        IncomingCommand,
        EngineInvocation,
        LocationCorrection,
        ScoresUpdate,
        OutgoingSensorData
    }

    public class IncomingCommandLogEntry
    {
        public string ControllerId { get; set; }
        public JObject Command { get; set; }
    }

    public class OutgoingSensorDataCommandLogEntry
    {
        public string ControllerId { get; set; }
        public JObject SensorData { get; set; }
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
        // некоторые поля были internal, из-за этого jsonconvert просто херачил в них null

        public int Added { get; set; }
        public string ControllerId { get; set; }
        public string Reason { get; set; }
        public int Scores { get; set; }
        public object Total { get; set; }
        public string Type { get; set; }
    }

    public class GameLogEntry
    {
        public double Time { get; set; }
        public GameLogEntryType Type { get; set; }
        public IncomingCommandLogEntry IncomingCommand { get; set; }
        public EngineInvocationLogEntry EngineInvocation { get; set; }
        public LocationCorrectionLogEntry LocationCorrection { get; set; }
        public ScoresUpdate ScoresUpdate { get; set; }
        public OutgoingSensorDataCommandLogEntry OutgoingSensorData { get; set; }
    }
}