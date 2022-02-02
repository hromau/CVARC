using System;

namespace Infrastructure
{
    public static class Serializer
    {
        public static string Serialize(object obj)
        {
            return Serializer.Serialize(obj);
        }

        public static T Deserialize<T>(string obj)
        {
            return Serializer.Deserialize<T>(obj);
        }

        public static object Deserialize(string obj, Type type)
        {
            return Serializer.Deserialize(obj, type);
        }
    }
}
