using AIRLab;
using CVARC.V2;
using HoMM.Robot;
using HoMM.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HoMM.World
{
    class CombatResolver
    {
        // хранит время, когда ребята освободятся от битвы
        private Dictionary<IHommRobot, double> combatQueue = 
            new Dictionary<IHommRobot, double>();

        private HashSet<Tuple<IHommRobot, IHommRobot>> currentCombats = 
            new HashSet<Tuple<IHommRobot, IHommRobot>>();
        
        public double Resolve(IHommRobot actor)
        {
            var others = actor.World.Actors
                .Where(x => x is IHommRobot)
                .Cast<IHommRobot>()
                .Where(x => x != actor && x.Player.Location == actor.Player.Location)
                .Where(x => !currentCombats.Contains(Tuple.Create(actor, x)));

            double timeInCombat = 0;

            foreach (var other in others)
            {
                currentCombats.Add(Tuple.Create(actor, other));
                timeInCombat += HommRules.Current.CombatDuration;

                if (!currentCombats.Contains(Tuple.Create(other, actor)))
                    Resolve(actor.World, actor, other);
            }

            return timeInCombat;
        }

        private void Resolve(HommWorld world, IHommRobot first, IHommRobot second)
        {
            var currentTime = world.Clocks.CurrentTime;
            var combatDuration = HommRules.Current.CombatDuration;

            var firstAvailable = combatQueue.ContainsKey(first) ? combatQueue[first] : currentTime;
            var otherAvailable = combatQueue.ContainsKey(second) ? combatQueue[second] : currentTime;

            var thisCombatStart = Math.Max(firstAvailable, otherAvailable);
            var thisCombatEnd = combatQueue[first] = combatQueue[second] = thisCombatStart + combatDuration;

            world.Clocks.AddTrigger(new OneTimeTrigger(thisCombatStart, () =>
            {
                if (first.IsDisabled || second.IsDisabled) return;

                // TODO: play combat animation

                world.Clocks.AddTrigger(new OneTimeTrigger(thisCombatEnd, () =>
                {
                    currentCombats.Remove(Tuple.Create(first, second));
                    currentCombats.Remove(Tuple.Create(second, first));

                    if (combatQueue[first] == thisCombatEnd)
                        combatQueue.Remove(first);

                    if (combatQueue[second] == thisCombatEnd)
                        combatQueue.Remove(second);

                    Combat.ResolveBattle(first.Player, second.Player);

                    if (first.Player.HasNoArmy())
                        first.Die();

                    if (second.Player.HasNoArmy())
                        second.Die();
                }));
            }));
        }
    }
}
