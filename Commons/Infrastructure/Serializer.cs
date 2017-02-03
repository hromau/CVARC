using Newtonsoft.Json;
using System;

namespace Infrastructure
{
    public static class Serializer
    {
        public static string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static T Deserialize<T>(string obj)
        {
            return JsonConvert.DeserializeObject<T>(obj);
        }

        public static object Deserialize(string obj, Type type)
        {
            return JsonConvert.DeserializeObject(obj, type);
        }
    }
}
