using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CVARC.V2
{

    public class ScoreRecord
    {
        public readonly double Time; 
        public readonly string Reason; 
        public readonly int Count;
        public readonly string Type;
        
        public ScoreRecord(int count, string reason, double time, string type=null )
        {
            Count = count;
            Reason = reason;
            Time = time;
            Type = type;
        }
    }
}
