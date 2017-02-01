using AIRLab;
using HoMM.Engine;
using HoMM.Rules;
using System;

namespace HoMM.Units.HexagonalMovement
{
    [Serializable]
    class Movement : IMovement
    {
        public Direction MovementDirection { get; }

        public Movement(Direction direction)
        {
            MovementDirection = direction;
        }

        public Tuple<Location, double> TryMoveHero(IHommEngine engine, Player player, Map map)
        {
            var newLocation = player.Location.NeighborAt(MovementDirection);
            var turnDuration = GetTravelDuration(player, map);

            var travelIsPossible = newLocation.IsInside(map.Size) &&
                (map[newLocation].tileObject?.IsPassable ?? false);

            if (travelIsPossible)
            {
                engine.Move(player.Name, MovementDirection, turnDuration);
                return Tuple.Create(newLocation, turnDuration);
            }

            return Tuple.Create(player.Location, turnDuration);
        }

        private double GetTravelDuration(Player player, Map map)
        {
            var velocityModifier = map[player.Location].tileTerrain.TravelCost;
            return HommRules.Current.MovementDuration * velocityModifier;
        }
    }

}
