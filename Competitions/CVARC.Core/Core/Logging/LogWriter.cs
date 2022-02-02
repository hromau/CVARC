
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Infrastructure;
using Ionic.Zip;
using Newtonsoft.Json.Linq;

namespace CVARC.V2
{

    public class LogNames
    {
        public const string GameSettings = "GameSettings";
        public const string WorldState = "WorldState";
        public const string Replay = "Replay";
        public const string Extension = ".cvarcreplay";
    }


    public class LogWriter
    {
        
        IWorld world;
        private bool enableLog;
        private string logFileName;
        private Infrastructure.GameSettings configuration;
        private object worldState;

        public LogWriter(IWorld world, bool enableLog, string logFile, Infrastructure.GameSettings configuration, object worldState) 
        {
            this.world = world;
            this.enableLog = enableLog;
            this.logFileName = logFile;
            this.configuration = configuration;
            this.worldState = worldState;
           
            world.Clocks.AddTrigger(new TimerTrigger(LogPositions, world.LoggingPositionTimeInterval));

            world.Scores.ScoresChanged += Scores_ScoresChanged;
            world.Exit += World_Exit;

            
        }

        private void LogPositions(double time)
        {
            if (world.LoggingPositionObjectIds.Count == 0) return;


            Debugger.Log("Logging positions at " + CurrentTime);
            var engine = world.GetEngine<ICommonEngine>();
            var data = world.LoggingPositionObjectIds
                .Where(z=>engine.ContainBody(z))
                .ToDictionary(z => z, z => engine.GetAbsoluteLocation(z));

            var entry = new GameLogEntry
            {
                Time = CurrentTime,
                Type = GameLogEntryType.LocationCorrection,
                LocationCorrection = new LocationCorrectionLogEntry
                {
                    Locations = data
                }
            };
            AddEntry(entry);
        }

        private void World_Exit()
        {
            Debugger.Log("Entering log save");
            if (enableLog)
            {
                Debugger.Log("Log is enabled, filename is " + logFileName + ", working directory is " + Environment.CurrentDirectory);

                try
                {
                    using (var zip = new ZipFile())
                    {
                        zip.AddEntry(LogNames.Replay, (name, stream) =>
                         {
                             var writer = new StreamWriter(stream, System.Text.Encoding.UTF8);
                             foreach (var e in log)
                             {
                                 writer.WriteLine(Serializer.Serialize(e));
                             }
                             writer.Close();

                         });
                        zip.AddEntry(LogNames.GameSettings, Serializer.Serialize(configuration));
                        zip.AddEntry(LogNames.WorldState, Serializer.Serialize(worldState));
                        zip.Save(logFileName);
                    }
                }
                catch(Exception e)
                {
                    Debugger.AddException(e);
                }
                Debugger.Log("Log is saved");
            }                
        }

        private void Scores_ScoresChanged(string controllerId, int count, string reason, string type, int total)
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
                    Total = total,
                    Type=type,
                }
            };
            AddEntry(entry);
        }
        

        List<object> log = new List<object>();

        public double CurrentTime { get { return world.Clocks.CurrentTime; } }

        public void AddMethodInvocation(Type engine, string method, params object[] arguments)
        {
             

            var entry = new GameLogEntry
            {
                Time = CurrentTime,
                Type = GameLogEntryType.EngineInvocation,
                EngineInvocation = new EngineInvocationLogEntry
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
            log.Add(entry);
        }

        public void AddIncomingCommand(string controllerId, object command)
        {
            var entry = new GameLogEntry
            {
                Time = CurrentTime,
                Type = GameLogEntryType.IncomingCommand,
                IncomingCommand = new IncomingCommandLogEntry
                {
                    Command = new JObject(command),
                    ControllerId = controllerId
                }
            };
            AddEntry(entry);
        }

        public void AddOutgoingSensorData(string controllerId, object sensorData)
        {
            var entry = new GameLogEntry
            {
                Time = CurrentTime,
                Type = GameLogEntryType.OutgoingSensorData,
                OutgoingSensorData = new OutgoingSensorDataCommandLogEntry
                {
                    ControllerId = controllerId,
                    SensorData = new JObject(sensorData)
                }
            };
            AddEntry(entry);
        }

    }
}
