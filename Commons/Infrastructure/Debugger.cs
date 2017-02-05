using System;
using System.Collections.Generic;
using System.IO;

namespace Infrastructure
{

    public static class Debugger
    {
        public static bool AlwaysOn;
        public static Action<string> Logger;
        static object lockObject = new object();
        static HashSet<string> enabledTypes = new HashSet<string>();

        public class SettingsClass
        {
            public SettingsClass EnableType<T>()
            {
                Debugger.enabledTypes.Add(typeof(T).Name);
                return this;
            }
            
            public SettingsClass EnableType(Type t)
            {
                enabledTypes.Add(t.Name);
                return this;
            }

            public SettingsClass EnableType(string  typeName)
            {
                enabledTypes.Add(typeName);
                return this;
            }
        }

        public static readonly SettingsClass Settings = new Debugger.SettingsClass();

        public static void Log(object message)
        {
            if (Logger == null) return;

            lock (lockObject)
            {
                var stack = new System.Diagnostics.StackTrace().GetFrame(1);
                var method = stack.GetMethod();
                var type = method.DeclaringType;
                
                if (AlwaysOn || enabledTypes.Contains(type.Name))
                {
                    var str = message == null ? "null" : message.ToString();
                    str = type.Name + "." + method.Name + ": " + str;
                    Logger(str);
                    File.AppendAllText("log.txt", str + "\n");
                }
            }
        }
    }
}
