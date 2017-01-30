using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using AIRLab;
using AIRLab.Mathematics;
using CVARC.Core;
using CVARC.V2;
using Ionic.Zlib;

namespace Assets
{
    public static class NewLogIO
    {
        public static NewLog Load(string fileName)
        {

            //using (var stream = File.Open(fileName, FileMode.Open, FileAccess.Read))
            //    return (NewLog)(new BinaryFormatter().Deserialize(stream));

            using (var stream = new Ionic.Zlib.GZipStream(File.Open(fileName, FileMode.Open, FileAccess.Read), CompressionMode.Decompress))            
            {
                return (NewLog) CVARC.Infrastructure.Serializer.Deserialize(typeof (NewLog), stream);
            }
        }

        public static void Save(NewLog log, string fileName)
        {
            
            //using (var stream = File.Open(fileName, FileMode.Create, FileAccess.Write))
            //    new BinaryFormatter().Serialize(stream, this);
            using (var stream = new Ionic.Zlib.GZipStream(File.Open(fileName, FileMode.Create, FileAccess.Write), CompressionMode.Compress))
            {
                CVARC.Infrastructure.Serializer.Serialize(log, stream);

            } //ToTextFile(fileName + ".txt");
        }
    }



    [Serializable]
    public class NewLog
    {

        void ToTextFile(string fileName)
        {
            var text = new StringBuilder();
            var counter = 0;
            foreach (var segment in Segments)
            {
                text.Append("Segment ");
                text.Append(counter);
                text.Append(":\n");
                foreach (var command in segment.Commands)
                {
                    text.Append(command.CommandName);
                    foreach (var arg in command.CommandArgs)
                    {
                        text.Append(" ");
                        text.Append(arg.ToString());
                    }
                    text.Append("\n");
                }
                text.Append("\n");
                counter++;
            }
            File.WriteAllText(fileName, text.ToString());
        }

        public readonly List<LogSegment> Segments;

        LogSegment currentSegment;

        public NewLog()
        {
            Segments = new List<LogSegment>();
            currentSegment = new LogSegment();
        }

        public void Log(string commandName, params object[] commandArgs)
        {
            currentSegment.Commands.Add(new LogCommand(commandName,
                commandArgs
                .Select(z => z.ToString())
                .ToArray()));
        }

        public void LogPositions(Tuple<string, Frame3D>[] positions)
        {
            currentSegment.Positions = positions;
        }

        public void LogScores(string leftScore, string rightScore)
        {
            currentSegment.Scores = new ScoresData { leftScores = leftScore, rightScores = rightScore };
        }

        public void EndSegment()
        {
            Segments.Add(currentSegment);
            currentSegment = new LogSegment();
        }

    }

    public class LogCommand
    {
        public readonly string CommandName;

        public readonly string[] CommandArgs;

        public LogCommand(string commandName, string[] commandArgs)
        {
            CommandName = commandName;
            CommandArgs = commandArgs;
        }
    }

    [Serializable]
    public class LogSegment
    {
        public readonly List<LogCommand> Commands;
        public Tuple<string, Frame3D>[] Positions;
        public ScoresData Scores;

        public LogSegment()
        {
            Commands = new List<LogCommand>();
        }
    }

    [Serializable]
    public class ScoresData
    {
        public string leftScores;
        public string rightScores;

    }

}
