using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using AIRLab.Mathematics;

namespace CVARC.V2
{
    public class LogPlayer
    {
        Dictionary<string, IEngine> engines = new Dictionary<string, IEngine>();

        ICommonEngine commonEngine;

        IEnumerator<string> lines;
        string lastLine;


        bool Play(double currentTime)
        {
            while(true)
            {
                var obj = JsonConvert.DeserializeObject<GameLogEntry>(lines.Current);
                if (obj.Time <= currentTime)
                {
                    Play(obj);
                    if (!lines.MoveNext()) return false;
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
                object[] arguments = ParseArguments(obj.EngineInvocation.Arguments, method.GetParameters());
                method.Invoke(engines[obj.EngineInvocation.EngineName], arguments);
            }
            if (obj.Type == GameLogEntryType.LocationCorrection)
            {
                if (obj.LocationCorrection==null)
                    throw new Exception("Line type is LocationCorrection, but the section LocationCorrection was empty");
                foreach (var e in obj.LocationCorrection.Locations)
                    if (!commonEngine.ContainBody(e.Key))
                        throw new Exception("Location update is defined for " + e.Key + " but it is absent at the map");
                    else
                        commonEngine.SetAbsoluteLocation(e.Key, e.Value);
            }
        }

        private object[] ParseArguments(string[] arguments, ParameterInfo[] parameterInfo)
        {
            if (arguments.Length != parameterInfo.Length)
                throw new Exception("Wrong count of arguments: expected " + parameterInfo.Length + " but was " + arguments.Length);
            var result = new object[arguments.Length];
            for (int i = 0; i < result.Length; i++)
                result[i] = Parse(arguments[i], parameterInfo[i].ParameterType);
            return result;
        }

        private object Parse(string v, Type parameterType)
        {
            if (parameterType == typeof(string)) return v;
            if (parameterType == typeof(Frame3D)) return Frame3D.Parse(v);
            if (parameterType.IsEnum) return Enum.Parse(parameterType, v);
            if (parameterType == typeof(int)) return int.Parse(v);
            if (parameterType == typeof(double)) return double.Parse(v);
            throw new Exception("Parameter type " + parameterType.Name + " is not supported");
        }
    }
}
