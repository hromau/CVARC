using CVARC.V2;
using HoMM.Robot.ArmyInterface;
using HoMM.Robot.HexagonalMovement;
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
