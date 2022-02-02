
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using AIRLab.Mathematics;
using Infrastructure;
using Ionic.Zip;

namespace CVARC.V2
{
    public class LogPlayer
    {
        public LogScoreProvider ScoreProvider;
        Dictionary<string, IEngine> engines = new Dictionary<string, IEngine>();

        ICommonEngine commonEngine;

        IEnumerator<string> lines;

        public GameSettings GameSettings { get; private set; }

        ZipFile zip;


        ~LogPlayer()
        {
            zip.Dispose();
        }

        public LogPlayer(string pathToZipFile, LogScoreProvider scoreProvider)
        {
            zip = ZipFile.Read(pathToZipFile);
            Debugger.Log("replay file found");
            var settings = zip.Where(z => z.FileName == LogNames.GameSettings).SingleOrDefault();
            if (settings == null)
                throw new Exception("No " + LogNames.GameSettings + " file is inside archive");
            GameSettings = Serializer.Deserialize<GameSettings>(Encoding.UTF8.GetString(settings.OpenReader().ReadToEnd()));
            Debugger.Log("Settings are read");
            var replay = zip.Where(z => z.FileName == LogNames.Replay).SingleOrDefault();
            if (replay == null)
                throw new Exception("No " + LogNames.Replay + " file is inside the archive");
            lines = ReadEntry(replay).GetEnumerator();
            if (!lines.MoveNext())
            {
                finished = true;
                throw new Exception("Replay is empty");
            }

            ScoreProvider = scoreProvider;

            Debugger.Log("Replay is opened to reading");
        }

        IEnumerable<string> ReadEntry(ZipEntry entry)
        {
            var stream = entry.OpenReader();
            var reader = new StreamReader(stream, Encoding.UTF8);
            while (true)
            {
                var str = reader.ReadLine();
                if (str == null)
                    break;
                yield return str;
            }
            reader.Close();
            stream.Close();
        }

        public void StartEngines(List<IEngine> engines)
        {
            this.engines = engines.ToDictionary(z => z.GetType().Name, z => z);
            commonEngine = engines.OfType<ICommonEngine>().Single();
            Debugger.Log("Engines started");
        }

        bool finished;

        public bool Play(double currentTime)
        {
            if (finished) return false;
            while (true)
            {

                var obj = Serializer.Deserialize<GameLogEntry>(lines.Current);
                if (obj.Time <= currentTime)
                {
                    Play(obj);
                    if (!lines.MoveNext())
                    {
                        finished = true;
                        return false;
                    }
                }
                else
                    return true;
            }
        }

        private void Play(GameLogEntry obj)
        {
            if (obj.Type == GameLogEntryType.EngineInvocation)
            {
                if (obj.EngineInvocation == null)
                    throw new Exception("Line type is EngineInvocation, but the section EngineInvokation was empty");
                if (!engines.ContainsKey(obj.EngineInvocation.EngineName))
                    throw new Exception("Can not find engine with a name " + obj.EngineInvocation.EngineName);
                var method = engines[obj.EngineInvocation.EngineName].GetType().GetMethod(obj.EngineInvocation.MethodName);
                if (method == null)
                    throw new Exception("Can not find method " + obj.EngineInvocation.MethodName + " in the engine " + obj.EngineInvocation.EngineName);
                object[] arguments = ParseArguments(obj.EngineInvocation.MethodName, obj.EngineInvocation.Arguments, method.GetParameters());
                method.Invoke(engines[obj.EngineInvocation.EngineName], arguments);
            }

            if (obj.Type == GameLogEntryType.LocationCorrection)
            {
                if (obj.LocationCorrection == null)
                    throw new Exception("Line type is LocationCorrection, but the section LocationCorrection was empty");
                foreach (var e in obj.LocationCorrection.Locations)
                    if (!commonEngine.ContainBody(e.Key))
                        throw new Exception("Location update is defined for " + e.Key + " but it is absent at the map");
                    else
                    {
                        commonEngine.SetAbsoluteLocation(e.Key, e.Value);
                    }
            }

            if (obj.Type == GameLogEntryType.ScoresUpdate)
            {
                if (obj.ScoresUpdate == null)
                    throw new Exception("Line type is ScoresUpdate, but the section ScoresUpdate was empty");

                ScoreProvider.UpdateScores(obj.ScoresUpdate);
            }
        }

        private object[] ParseArguments(string methodName, string[] arguments, ParameterInfo[] parameterInfo)
        {
            if (arguments.Length != parameterInfo.Length)
                throw new Exception("Wrong count of arguments for method " + methodName + ": expected " + parameterInfo.Length + " but was " + arguments.Length);
            var result = new object[arguments.Length];
            for (int i = 0; i < result.Length; i++)
                result[i] = Parse(arguments[i], parameterInfo[i].ParameterType, methodName, i);
            return result;
        }

        private object Parse(string v, Type parameterType, string methodName, int index)
        {
            if (parameterType == typeof(string)) return v;
            if (parameterType == typeof(Frame3D)) return Frame3D.Parse(v);
            if (parameterType.IsEnum) return Enum.Parse(parameterType, v);
            if (parameterType == typeof(int)) return int.Parse(v);
            if (parameterType == typeof(double)) return double.Parse(v);
            if (parameterType == typeof(Single)) return Single.Parse(v);
            if (parameterType == typeof(bool)) return bool.Parse(v);
            throw new Exception("Parameter type " + parameterType.Name + " is not supported. Method " + methodName + ", index " + index);
        }
    }
}
