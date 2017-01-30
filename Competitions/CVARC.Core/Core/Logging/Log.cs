using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using AIRLab;
using AIRLab.Mathematics;

namespace CVARC.V2
{
    [Serializable]
    public class Log
    {
        public Configuration Configuration { get; set; }
        public List<Tuple<string, List<PositionLogItem>>> Positions = new List<Tuple<string, List<PositionLogItem>>>();
        public List<Tuple<string, List<ICommand>>> Commands = new List<Tuple<string, List<ICommand>>>();
        public double LoggingDeltaTime { get; set; }
        public IWorldState WorldState { get; set; }

        public void Save(string filename)
        {
            using (var stream = File.Open(filename, FileMode.Create, FileAccess.Write))
            {
                new BinaryFormatter().Serialize(stream, this);
            }
        }

        public static Log Load(string filename)
        {
            using (var stream = File.Open(filename, FileMode.Open, FileAccess.Read))
            {
                return (Log)(new BinaryFormatter().Deserialize(stream));
            }
        }
    }
}
