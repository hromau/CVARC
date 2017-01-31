using System.Linq;
using AIRLab.Mathematics;
using Infrastructure;

namespace CVARC.V2
{
    public class LocatorSensor : Sensor<LocatorItem[],IActor>
    {
        public override LocatorItem[] Measure()
        {
            return Actor.World.Actors
                .OfType<IActor>()
                .Where(z=>Actor.World.GetEngine<ICommonEngine>().ContainBody(z.ObjectId))
                .Select(z => new Tuple<string, Frame3D>(
                    z.ControllerId,
                    Actor.World.GetEngine<ICommonEngine>().GetAbsoluteLocation(z.ObjectId)))
                .Select(z => new LocatorItem
                {
                    Id = z.Item1,
                    X = z.Item2.X,
                    Y = z.Item2.Y,
                    Angle = z.Item2.Yaw.Grad
                })
                .ToArray();
        }
    }
}
