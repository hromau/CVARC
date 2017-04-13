using System.Linq;

namespace CVARC.V2
{
    public abstract class ScoresSensor : Sensor<int, IActor>
    {
        public readonly bool MeasureSelf;

        public ScoresSensor(bool measureSelf)
        {
            MeasureSelf = measureSelf;
        }

        public override int Measure()
        {
            var id = Actor.World.Actors
                .Where(z => z.ControllerId == Actor.ControllerId == MeasureSelf)
                .Select(z => z.ControllerId)
                .FirstOrDefault();
            return Actor.World.Scores.GetTotalScore(id);
        }
    }

    public class SelfScoresSensor : ScoresSensor
    {
        public SelfScoresSensor() : base(true) { }
    }
}
