using CVARC.V2;
using HoMM.ClientClasses;
using HoMM.Engine;
using HoMM.Robot;
using Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace HoMM.Robot.ArmyInterface
{
    class GarrisonBuilderUnit : IUnit
    {
        private static int lastId;

        private IHommRobot actor;

        public GarrisonBuilderUnit(IHommRobot actor)
        {
            this.actor = actor;
        }

        public UnitResponse ProcessCommand(object command)
        {
            var armyForGarrison = Compatibility.Check<IGarrisonCommand>(this, command).WaitInGarrison;
            if (armyForGarrison == null) return UnitResponse.Denied();

            if (actor.IsDead) return UnitResponse.Accepted(HommRules.Current.GarrisonBuildDuration);

            actor.World.HommEngine.Freeze(actor.ControllerId);
            actor.World.HommEngine.SetAnimation(actor.ControllerId, Animation.Idle);

            if (!CheckCommandCorrect(armyForGarrison))
                return UnitResponse.Accepted(HommRules.Current.GarrisonBuildDuration);

            TakeUnitsAway(actor.Player, armyForGarrison);

            var garrison = FindGarrison(actor.Player.Location);

            if (garrison == null)
                garrison = CreateGarrison(armyForGarrison, actor.Player);
            else
                garrison.Pupulate(armyForGarrison);

            Debug(garrison);

            return UnitResponse.Accepted(HommRules.Current.GarrisonBuildDuration);
        }

        private void TakeUnitsAway(Player player, Dictionary<UnitType, int> armyForGarrison)
        {
            foreach (var kv in armyForGarrison)
            {
                var unitType = kv.Key;
                var amount = kv.Value;

                player.SetUnitsCount(kv.Key, player.Army[kv.Key] - kv.Value);
            }
        }

        private void Debug(Garrison garrison)
        {
            Debugger.Log($"Garrison guards:");

            foreach (var kv in garrison.Army)
                Debugger.Log($"{kv.Key} - {kv.Value}");

            Debugger.Log($"{actor.ControllerId} army:");

            foreach (var kv in actor.Player.Army)
                Debugger.Log($"{kv.Key} - {kv.Value}");
        }

        private Garrison CreateGarrison(Dictionary<UnitType, int> armyForGarrison, Player owner)
        {
            var location = owner.Location;

            var garrison = new Garrison(armyForGarrison, location, owner);
            garrison.UnityId = "Garrison " + lastId++;

            actor.World.Round.Map[location].AddObject(garrison);
            actor.World.HommObjectsCreationHelper.CreateArmy(garrison, garrison.location);
            actor.World.HommEngine.SetFlag(garrison.UnityId, owner.Name);

            return garrison;
        }

        private Garrison FindGarrison(Location location)
        {
            return actor.World.Round.Map[location].Objects
                .Where(x => x is Garrison)
                .Cast<Garrison>()
                .FirstOrDefault();
        }

        private bool CheckCommandCorrect(Dictionary<UnitType, int> armyForGarrison)
        {
            var playerArmy = actor.Player.Army;

            foreach (var kv in armyForGarrison)
            {
                var unitType = kv.Key;
                var amount = kv.Value;

                if (amount < 0 || playerArmy.GetOrDefault(unitType, 0) < amount)
                    return false;
            }

            return true;
        }
    }
}
