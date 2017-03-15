using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure
{
    public class GameResult
    {
        public Dictionary<string, Dictionary<string, int>> ScoresByPlayer { get; set; }
        public string PathToLogFile { get; set; }

        [Obsolete("Parameterless constructor is left for serializer, do not call it directly.")]
        public GameResult() { }

        public GameResult(Dictionary<string, Dictionary<string, int>> scores)
        {
            ScoresByPlayer = scores;
        }

        public override string ToString()
        {
            return ScoresByPlayer.Count == 0 
                ? "Empty GameResult"
                : ScoresByPlayer
                    .Select(x => $"{x.Key}: {x.Value.Select(z => $"{z.Key} - {z.Value}").Aggregate((u, v) => $"{u}, {v}")}")
                    .Aggregate((u, v) => u + Environment.NewLine + v);
        }
    }
}
