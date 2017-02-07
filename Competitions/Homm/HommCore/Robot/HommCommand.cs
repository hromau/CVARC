using CVARC.V2;
using HoMM.Robot.ArmyInterface;
using HoMM.Robot.HexagonalMovement;
using System;
using System.Runtime.Serialization;

namespace HoMM.Robot
{
    [Serializable]
    [DataContract]
    public class HommCommand : ICommand, IHexMovCommand, IArmyInterfaceCommand
    {
        [DataMember]
        public HexMovement Movement { get; set; }

        [DataMember]
        public PurchaseOrder Order { get; set; }
    }
}
