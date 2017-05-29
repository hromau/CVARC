using CVARC.V2;
using HoMM.Robot.ArmyInterface;
using HoMM.Robot.HexagonalMovement;
using HoMM.Robot.ScoutInterface;
using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace HoMM.Robot
{
    [Serializable]
    [DataContract]
    public class HommCommand : ICommand, 
        IHexMovCommand, IArmyInterfaceCommand, IGarrisonCommand, IScoutCommand
    {
        [DataMember]
        public HexMovement Movement { get; set; }

        [DataMember]
        public HireOrder HireOrder { get; set; }

        [DataMember]
        public ScoutOrder ScoutOrder { get; set; }

        [DataMember]
        public Dictionary<UnitType, int> WaitInGarrison { get; set; }
    }
}
