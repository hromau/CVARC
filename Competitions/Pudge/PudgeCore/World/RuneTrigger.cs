using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using AIRLab.Mathematics;
using CVARC.V2;
using Pudge.Player;

namespace Pudge.World
{
    class RuneTrigger : Trigger //TimerTriggers
    {
        private PudgeWorld world;

        public double Interval{ get; private set; }
        private readonly Dictionary<RuneType, Action<InternalRuneData,string>> BuffActions;
        public RuneTrigger(PudgeWorld world, double interval = 0.5)
        {
            this.world = world;
            Interval = ScheduledTime = interval;
            BuffActions = new Dictionary<RuneType, Action<InternalRuneData, string>>
            {
                {RuneType.GoldXP, AddScores}, 
                {RuneType.Haste, HandleHasteRune},
                {RuneType.Invisibility, HandleInvisRune},
                {RuneType.DoubleDamage, HandleDoubleDamageRune }
            };
        }

        private void HandleDoubleDamageRune(InternalRuneData data, string id)
        {
            var pudge = world.Actors.OfType<PudgeRobot>()
                .First(a => a.ControllerId == id);
            pudge.ActivateBuff(PudgeEvent.DoubleDamage, PudgeRules.Current.BuffDurations[data]);

        }

        private void AddScores(InternalRuneData data, string id)
        {
            if (PudgeRules.Current.RuneScores.ContainsKey(data))
            {
                world.Scores.Add(id, PudgeRules.Current.RuneScores[data],
                    string.Format("Picked {0} {1} rune", data.Size, data.Type));}
        }

        private void HandleHasteRune(InternalRuneData data, string id)
        {
            AddScores(data, id);
            world.Actors.OfType<PudgeRobot>()
                .First(a => a.ControllerId == id)
                .ActivateBuff(PudgeEvent.Hasted, PudgeRules.Current.BuffDurations[data]);
        }

        private void HandleInvisRune(InternalRuneData data, string id)
        {
            AddScores(data, id);

            var pudge = world.Actors.OfType<PudgeRobot>()
                .First(a => a.ControllerId == id);
            var buffDuration = PudgeRules.Current.BuffDurations[data];
            var engine = pudge.World.GetEngine<IPudgeWorldEngine>();

            pudge.ActivateBuff(PudgeEvent.Invisible, buffDuration);
            engine.SetTransparent(pudge.ObjectId, true);

            world.Clocks.AddTrigger(new OneTimeTrigger(world.Clocks.CurrentTime + buffDuration + 0.1, () =>
            {
                if (!pudge.IsBuffActivated(PudgeEvent.Invisible))
                    engine.SetTransparent(pudge.ObjectId, false);
            }));
        }

        public override TriggerKeep Act(double time)
        {
            CheckRunes();
            ScheduledTime += Interval;
            return TriggerKeep.Keep;
        }

        private void CheckRunes()
        {
            var engine = world.GetEngine<ICommonEngine>();
            var controllerIdToLocation = world.Actors
                .Where(actor => !actor.IsDisabled)
            	.Where(actor => actor is PudgeRobot)
                .ToDictionary(a => a.ControllerId, a => engine.GetAbsoluteLocation(a.ObjectId));
            foreach (var controllerId in controllerIdToLocation.Keys)
            {
                if (world.SpawnedRunes.Count == 0)
                    return;

                var nearestRune = world.SpawnedRunes
                    .Select(rune => new
                    {
                        rune,
                        distance = Geometry.Distance(
                            rune.Location.ToPoint3D(),
                            controllerIdToLocation[controllerId].ToPoint3D())
                    })
                    .Where(z => z.distance < PudgeRules.Current.RunePickRange)
                    .Select(z=>z.rune)
                    .FirstOrDefault();

                if (nearestRune!=null)
                {
                    var id = world.IdGenerator.GetIdByObject(nearestRune);
                    if (id == null)
                        return;
                    engine.DeleteObject(id);

                    if (BuffActions.ContainsKey(nearestRune.Type))
                        BuffActions[nearestRune.Type](nearestRune,controllerId);
                    world.SpawnedRunes.Remove(nearestRune);
                }
            }
        }
    }
}
