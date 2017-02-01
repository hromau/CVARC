using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AIRLab.Mathematics;
using CVARC.Basic;
using CVARC.Basic.Sensors;

namespace CVARC.V2
{
    public interface IEngine
    {
        LogWriter LogWriter { get; set; }
    }

    public static class IEngineExtensions
    {
        public static void Log(this IEngine engine, string methodName, params object[] arguments)
        {
            if (engine.LogWriter != null)
                engine.LogWriter.AddMethodInvocation(engine.GetType(), methodName, arguments);
        }
    }

}
