using AIRLab.Mathematics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Infrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CVARC.V2
{



    public class LogWriter
    {
        
        IWorld world;
        private bool enableLog;
        private string logFile;
        private Configuration configuration;
        private object worldState;

        public LogWriter(IWorld world, bool enableLog, string logFile, Configuration configuration, object worldState) 
        {
            this.world = world;
            this.enableLog = enableLog;
            this.logFile = logFile;
            this.configuration = configuration;
            this.worldState = worldState;

            world.Scores.ScoresChanged += Scores_ScoresChanged;
            
        }

        private void Scores_ScoresChanged(string controllerId, int count, string reason, int total)
        {
            var entry = new GameLogEntry
            {
                Time = CurrentTime,
                Type = GameLogEntryType.ScoresUpdate,
                ScoresUpdate = new ScoresUpdate
                {
                    ControllerId = controllerId,
                    Added = count,
                    Reason = reason,
                    Total = total
                }
            };
            AddEntry(entry);
        }
        

        List<string> log = new List<string>();

        public double CurrentTime { get; set; }

        public void AddMethodInvocation(Type engine, string method, params object[] arguments)
        {
            var entry = new GameLogEntry
            {
                Time = CurrentTime,
                Type = GameLogEntryType.EngineInvocation,
                EngineCommand = new EngineInvocationLogEntry
                {
                    EngineName = engine.Name,
                    MethodName = method,
                    Arguments = arguments.Select(z => z.ToString()).ToArray()
                }
            };
            AddEntry(entry);
        }

        private void AddEntry(GameLogEntry entry)
        {
            log.Add(JsonConvert.SerializeObject(entry, Formatting.None));
        }

        public void AddIncomingCommand(string controllerId, object command)
        {
            var entry = new GameLogEntry
            {
                Time = CurrentTime,
                Type = GameLogEntryType.IncomingCommand,
                IncomingCommand = new IncomingCommandLogEntry
                {
                    Command = JObject.FromObject(command),
                    ControllerId = controllerId
                }
            };
            AddEntry(entry);
        }

        public void AddLocationCorrection(Dictionary<string, Frame3D> correction)
        {
            var entry = new GameLogEntry
            {
                Time = CurrentTime,
                Type = GameLogEntryType.LocationCorrection,
                LocationCorrection = new LocationCorrectionLogEntry
                {
                    Locations = correction
                }
            };
            AddEntry(entry);
        }

        public void Write(string filename)
        {
            File.WriteAllLines(filename, log.ToArray());
        }
    }
}
