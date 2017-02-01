using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CVARC.V2
{

    public static class Debugger
    {
        static object lockObject = new object();
        static HashSet<Type> enabledTypes = new HashSet<Type>();
        static HashSet<MethodBase> enabledMethods = new HashSet<MethodBase>();

        public class SettingsClass
        {
            public SettingsClass EnableType<T>()
            {
                Debugger.enabledTypes.Add(typeof(T));
                return this;
            }

            public SettingsClass EnabledMethod<T>(string methodName)
            {
                Debugger.enabledMethods.Add(typeof(T).GetMethod(methodName));
                return this;
            }

            public SettingsClass EnableType(Type t)
            {
                enabledTypes.Add(t);
                return this;
            }
        }

        public static readonly SettingsClass Settings = new Debugger.SettingsClass();
        




        public static void Log(object message)
        {
            lock (lockObject)
            {
                var stack = new System.Diagnostics.StackTrace().GetFrame(1);
                var method = stack.GetMethod();
                var type = method.DeclaringType;
                if (Logger != null && (enabledTypes.Contains(type) || enabledMethods.Contains(method)))
                {
                    var str = message == null ? "null" : message.ToString();
                    Logger(str);
                    File.AppendAllText("log.txt", str + "\n");
                }
            }
        }

        public static Action<string> Logger;
    }
}
