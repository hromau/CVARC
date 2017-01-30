using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using AIRLab.Mathematics;
using CVARC.V2;
using Pudge.Player;
using Pudge.World;
using Pudge.ClientClasses;

namespace Pudge.Sensors.Map
{
    public class MapSensor : Sensor<Map, PudgeRobot>
    {
        public override Map Measure()
        {
            if (Actor.IsDisabled) return null;

            var actorLocation = Actor.World.GetEngine<ICommonEngine>().GetAbsoluteLocation(Actor.ObjectId).ToPoint3D();
            List<RuneData> runes = GetVisibleRunes(actorLocation);
            List<HeroData> heroes = GetVisibleHeroes(actorLocation);
            List<HookData> enemyHooks = GetVisibleEnemyHooks(actorLocation);
            return new Map(heroes, runes, enemyHooks);
        }

        private List<HookData> GetVisibleEnemyHooks(Point3D actorLocation)
        {
            var engine = Actor.World.GetEngine<ICommonEngine>();
            return Actor.World.IdGenerator.GetAllPairsOfType<Hook>()
                .Where(pair => engine.ContainBody(pair.Item2) && pair.Item1.Pudge.ObjectId != Actor.ObjectId)
                .Select(pair => new HookData(engine.GetAbsoluteLocation(pair.Item2)))
                .Where(data => Geometry.Distance(data.Location.ToPoint3D(), actorLocation) < PudgeRules.Current.VisibilityRadius)
                .ToList();
        }

        private List<HeroData> GetVisibleHeroes(Point3D actorLocation)
        {
            return Actor.World.Actors
                .Where(a => a.ObjectId != Actor.ObjectId && !a.IsDisabled)
                .Where(a => !(a is PudgeRobot) || !Compatibility.Check<PudgeRobot>(this, a).IsInvisible)
                .Select(actor => new HeroData
                    (
                    actor is PudgeRobot ? HeroType.Pudge : HeroType.Slardar,
                    Actor.World.GetEngine<ICommonEngine>().GetAbsoluteLocation(actor.ObjectId)
                    ))
                .Where(h => Geometry.Distance(h.Location.ToPoint3D(), actorLocation) <
                            Actor.Rules.VisibilityRadius * Actor.DoubleDamageFactor || InWardScope(h.Location.ToPoint3D()))
                .ToList();
        }

        private List<RuneData> GetVisibleRunes(Point3D actorLocation)
        {
            return Actor.World.SpawnedRunes
                .Where(
                    r => Geometry.Distance(r.Location.ToPoint3D(), actorLocation) <
                         Actor.Rules.VisibilityRadius * Actor.DoubleDamageFactor || InWardScope(r.Location.ToPoint3D()))
                .Select(
                    rd => new RuneData(rd.Type, rd.Size, rd.Location.ToFrame2D().ToPoint2D()))
                .ToList();
        }

        private bool InWardScope(Point3D location)
        {
            return
                Actor.Wards.Any(w => 
                Geometry.Distance(location, w.Location.ToPoint3D()) < PudgeRules.Current.WardRadius);
        }
    }
}