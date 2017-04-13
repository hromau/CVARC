using System;
using System.Collections.Generic;

namespace CVARC.V2
{
    public class LogScoreProvider : IScoreProvider
    {
        private readonly Scores scores;

        public LogScoreProvider()
        {
            scores = new Scores(null);
        }

        public void UpdateScores(ScoresUpdate scoresUpdate)
        {
            if (scoresUpdate.Type == null)
                throw new ArgumentNullException("scoresUpdate.Type");

            if (!scores.Records.ContainsKey(scoresUpdate.ControllerId))
                scores.Records[scoresUpdate.ControllerId] = new List<ScoreRecord>();

            scores.Records[scoresUpdate.ControllerId].Add(new ScoreRecord(scoresUpdate.Added, scoresUpdate.Reason,
                                                                          0, scoresUpdate.Type));
        }

        public Scores GetScores()
        {
            return scores;
        }
    }
}