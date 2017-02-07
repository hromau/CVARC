using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Infrastructure
{
    public class MethodName
    {
        public string Type;
        public string Method;
        public bool Match(MethodBase info)
        {
            return info.Name == Method && info.DeclaringType.Name == Type;
        }
    }

    public class DebuggerConfig
    {
        public bool AlwaysOn { get; set; }
        public bool AlwaysOff { get; set; }
        public List<string> EnabledTypes { get; set; }
        public List<MethodName> CallStackRoots { get; set; }
        public List<string> EnabledNamespaces { get; set; }
    }
}
