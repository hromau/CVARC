using CVARC.V2;
using HoMM.Units.ArmyInterface;
using HoMM.Units.HexagonalMovement;
using System;
using System.Runtime.Serialization;

namespace HoMM.Robot
{
    [Serializable]
    [DataContract]
    class HommCommand : ICommand, IHexMovCommand, IArmyInterfaceCommand
    {
        [DataMember]
        public IMovement Movement { get; set; }

        [DataMember]
        public IOrder Order { get; set; }
    }
}
