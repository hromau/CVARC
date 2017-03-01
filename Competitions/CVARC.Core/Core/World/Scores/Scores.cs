using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure;

namespace CVARC.V2
{

    public delegate void ScoresAddEventHandler(string controllerId, int count, string reason, string type, int total);

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
        
        public void Add(string controllerId, int count, string reason, params string[] types)
        {
            if (!Records.ContainsKey(controllerId))
                Records[controllerId] = new List<ScoreRecord>();

            foreach (var type in types)
            {
                Records[controllerId].Add(new ScoreRecord(count, reason, world.Clocks.CurrentTime, type));
                if (ScoresChanged != null) ScoresChanged(controllerId, count, reason, type, GetTotalScore(controllerId));
            }
        }
        
        public IEnumerable<Tuple<string, int>> GetAllScores()
        {
            return Records.Keys.Select(z => new Tuple<string, int>(z, Records[z].Sum(x => x.Count)));
        }
        

        public Dictionary<string, Dictionary<string, int>> GetSumByType()
        {
            return Records
                .ToDictionary(
                    x => x.Key,
                    x => x.Value
                        .GroupBy(z => z.Type)
                        .ToDictionary(z => z.Key ?? "Undefined", z => z.Sum(r => r.Count))
                );
        }

        public int GetTotalScore(string controllerId)
        {
            if (!Records.ContainsKey(controllerId))
                return 0;
            return Records[controllerId].Sum(x => x.Count);
        }
    }
}
