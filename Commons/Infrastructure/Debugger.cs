using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Infrastructure
{

    public static class Debugger
    {
        public static event Action<string> Logger;
        static object lockObject = new object();

        public static DebuggerConfig Config { get; set; }


        public static void AddException(Exception e)
        {
            string message = "";
            Exception e1 = e;
            while(e1!=null)
            {
                message += e1.GetType().Name+" "+e1.Message + "\n";
                e1 = e1.InnerException;
            }
            message += e.StackTrace;
            lock (lockObject)
            {
                if (Logger != null)
                    Logger(message);
            }
        }

        public static void Log(object message)
        {
            if (Config == null) return;
            if (Config.AlwaysOff) return;
            if (Logger == null) return;

            var stack = new System.Diagnostics.StackTrace().GetFrames();
            var method = stack[1].GetMethod();
            var type = method.DeclaringType;

            var print = false;

            if (Config.AlwaysOn) print = true;

            if (!print && Config.EnabledTypes!=null && Config.EnabledTypes.Count>0)
            {
                if (Config.EnabledTypes.Any(z => type.Name.StartsWith(z)))
                    print = true;
            }

            if (!print && type.Namespace!=null && Config.EnabledNamespaces!=null &&  Config.EnabledNamespaces.Count>0)
            {
                if (Config.EnabledNamespaces.Any(z => type.Namespace.StartsWith(z)))
                    print = true;
            }

            if (!print && Config.CallStackRoots!=null &&  Config.CallStackRoots.Count>0)
            {
                for (int i = 0; i < stack.Length; i++)
                {
                    if (Config.CallStackRoots.Any(z => z.Match(stack[i].GetMethod())))
                    {
                        print = true;
                    }
                }
            }

            if (!print) return;

            var str = message == null ? "null" : message.ToString();
            str = type.Name + "." + method.Name + ": " + str;

            lock (lockObject)
            {
                if (Logger!=null)
                    Logger(str);
            }
        }
    }
}
