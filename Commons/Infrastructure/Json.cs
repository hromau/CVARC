using System.IO;

namespace Infrastructure
{
    public class Json
    {
        public static T Read<T>(string filename)
        {
            return Serializer.Deserialize<T>(File.ReadAllText(filename));
        }

        public static void Write(string filename, object t)
        {
            File.WriteAllText(filename, Serializer.Serialize(t));
        }
    }
}
