using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Infrastructure
{
    public class Json
    {
        public static T Read<T>(string filename)
        {
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(filename));
        }

        public static void Write(string filename, object t)
        {
            File.WriteAllText(filename, JsonConvert.SerializeObject(t, Newtonsoft.Json.Formatting.Indented));
        }
    }
}
