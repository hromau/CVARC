using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure;

namespace CVARC.V2
{

    public delegate void ScoresAddEventHandler(string controllerId, int count, string reason, int total);

    public class Scores
    {
        IWorld world;
        
        public Scores(IWorld world)
        {
            this.world = world;
            Records = new Dictionary<string, List<ScoreRecord>>();
        }

        public Dictionary<string, List<ScoreRecord>> Records { get; private set; } 
        public event ScoresAddEventHandler ScoresChanged;
        
        public void Add(string controllerId, int count, string reason)
        {
            if (!Records.ContainsKey(controllerId))
                Records[controllerId] = new List<ScoreRecord>();
            Records[controllerId].Add(new ScoreRecord(count, reason, world.Clocks.CurrentTime));
            if (ScoresChanged != null) ScoresChanged(controllerId,count,reason, GetTotalScore(controllerId));
        }
        
        public IEnumerable<Tuple<string, int>> GetAllScores()
        {
            return Records.Keys.Select(z => new Tuple<string, int>(z, Records[z].Sum(x => x.Count)));
        }
        
        public int GetTotalScore(string controllerId)
        {
            if (!Records.ContainsKey(controllerId))
                return 0;
            return Records[controllerId].Sum(x => x.Count);
        }
    }
}
