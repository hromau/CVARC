using System.Collections.Generic;

namespace HoMM.Robot.ArmyInterface
{
    interface IGarrisonCommand
    {
        Dictionary<UnitType, int> WaitInGarrison { get; }
    }
}
