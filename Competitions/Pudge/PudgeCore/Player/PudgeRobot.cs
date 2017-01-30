using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using AIRLab;
using AIRLab.Mathematics;
using CVARC.V2;
using Pudge.Sensors;
using Pudge.Units.DaggerUnit;
using Pudge.Units.HookUnit;
using Pudge.Units.PudgeUnit;
using Pudge.Units.WardUnit;
using Pudge.Units.WADUnit;
using Pudge.World;
using WardUnit = Pudge.Units.WardUnit.WardUnit;

namespace Pudge.Player
{
    public class PudgeRobot :
        Robot<PudgeWorld, PudgeSensorsData, PudgeCommand, PudgeRules>, 
        IHookRobot, IWADRobot, IWardRobot, IDaggerRobot
    {
        public override IEnumerable<IUnit> Units
        {
            get
            {
                yield return HookUnit;
                yield return WardUnit;
                yield return DaggerUnit;
                yield return WADUnit;
                yield return DeathUnit;
            }
        }

        public DaggerUnit DaggerUnit{ get; private set; }
        public WardUnit WardUnit{ get; private set; }
        public WADUnit WADUnit{ get; private set; }
        public HookUnit HookUnit{ get; private set; }
        public DeathUnit DeathUnit { get; private set; }

        double respawnTime;

        public override void AdditionalInitialization()
        {
            base.AdditionalInitialization();
            World.GetEngine<IPudgeWorldEngine>().CreatePudgeBody(ObjectId, ControllerId);
            DaggerUnit = new DaggerUnit(this);
            WADUnit = new WADUnit(this);
            HookUnit = new HookUnit(this);
            WardUnit = new WardUnit(this);
            DeathUnit = new DeathUnit(() => respawnTime - World.Clocks.CurrentTime);
            LastActivatingTime = new Dictionary<PudgeEvent, EventData>();
            AvailableWards = PudgeRules.Current.AvailableWardsAtStart;
            World.Clocks.AddTrigger(new TimerTrigger(_ => AvailableWards++, PudgeRules.Current.WardIncrementTime));
            Wards = new List<Ward>();
        }

        public Dictionary<PudgeEvent, EventData> LastActivatingTime;
        public void ActivateBuff(PudgeEvent type, double duration)
        {
            LastActivatingTime[type] = new EventData
            {
                Event = type,
                Duration = duration,
                Start = World.Clocks.CurrentTime
            };
        }
        

        private int AvailableWards{ get; set; }
        public List<Ward> Wards;

        public void SpawnWard()
        {
            if (Wards.Count == AvailableWards)
                return;
            var commonEngine = World.GetEngine<ICommonEngine>();
            var location = commonEngine.GetAbsoluteLocation(ObjectId);
            var ward = new Ward {Location = location.ToFrame2D().ToPoint2D()};
            Wards.Add(ward);
            var wardId = World.IdGenerator.CreateNewId(ward);
            World.GetEngine<IPudgeWorldEngine>().CreateWard(wardId, location);
            World.Clocks.AddTrigger(new OneTimeTrigger(World.Clocks.CurrentTime + PudgeRules.Current.WardDuration, () =>
            {
                Wards.Remove(ward);
                commonEngine.DeleteObject(wardId);
            }));
        }

        public double HasteFactor
        {
            get { return IsBuffActivated(PudgeEvent.Hasted) ? Rules.HasteFactor : 1; }
        }

        public double DoubleDamageFactor
        {
            get { return IsBuffActivated(PudgeEvent.DoubleDamage) ? Rules.DoubleDamageFactor : 1; }
        }


        public bool IsInvisible{ get { return IsBuffActivated(PudgeEvent.Invisible); } }
        
        public bool IsBuffActivated(PudgeEvent type)
        {
            var res = LastActivatingTime.ContainsKey(type) &&
                   World.Clocks.CurrentTime - LastActivatingTime[type].Start <
                   LastActivatingTime[type].Duration;
            return res;
        }
        
        public void SpawnHook(Frame3D startingLocation, string id)
        {
            var s = startingLocation;
            World.GetEngine<IPudgeWorldEngine>().SpawnHook(s.X, s.Y, s.Yaw.Grad, id);
        }

        public void Die()
        {
            this.Die(PudgeRules.Current.PudgeRespawnTime);

            respawnTime = PudgeRules.Current.PudgeRespawnTime + World.Clocks.CurrentTime;

            LastActivatingTime.Clear();
            World.GetEngine<IPudgeWorldEngine>().SetTransparent(ObjectId, false);
            int currentScores = World.Scores.GetTotalScore(ControllerId);
            var deathPenalty = -(currentScores == 0
                ? 0
                : currentScores > 10 
                ? currentScores/10 
                : 1);
            World.Scores.Add(ControllerId, deathPenalty, "Pudge RIP");
        }

        public void DeleteBuff(PudgeEvent type)
        {
            LastActivatingTime.Remove(type);
        }

        public void Dagger(DaggerDestinationPoint _destination)
        {
            if (IsBuffActivated(PudgeEvent.Blink)) return;
            var commonEngine = World.GetEngine<ICommonEngine>();
            var destination = new Point2D(_destination.X, _destination.Y);
            DeleteBuff(PudgeEvent.Invisible);
            World.GetEngine<IPudgeWorldEngine>().SetTransparent(ObjectId, false);
            //Manager.SetTransparent(false);
            if (!InBounds(destination))
                return;
            ActivateBuff(PudgeEvent.Blink, PudgeRules.Current.DaggerCooldown);
            var currentLocation = commonEngine.GetAbsoluteLocation(ObjectId);
            var point2DLocation = currentLocation.ToFrame2D().ToPoint2D();
            var vector = destination - currentLocation.ToFrame2D().ToPoint2D();
            var daggerDistance = Geometry.Distance(destination, point2DLocation);
            var position = daggerDistance > PudgeRules.Current.DaggerRange
                ? vector.Normalize() * PudgeRules.Current.DaggerRange + point2DLocation
                : destination;
            commonEngine.SetAbsoluteLocation(ObjectId,
                new Frame3D(
                    position.X, position.Y, currentLocation.Z, 
                    currentLocation.Pitch, currentLocation.Yaw, currentLocation.Roll));
            World.GetEngine<IPudgeWorldEngine>().MakeDaggerExplosion(currentLocation);

        }

        private bool InBounds(Point2D position)
        {
            return position.X >= -160 && position.X <= 160 && position.Y >= -160 && position.Y <= 160;
        }
    }
}