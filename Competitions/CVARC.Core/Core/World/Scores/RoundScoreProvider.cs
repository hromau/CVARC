namespace CVARC.V2
{
    public class RoundScoreProvider : IScoreProvider
    {
        private readonly IWorld world;

        public RoundScoreProvider(IWorld world)
        {
            this.world = world;
        }

        public Scores GetScores()
        {
            return world.Scores;
        }
    }
}