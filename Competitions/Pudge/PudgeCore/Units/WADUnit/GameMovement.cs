using System;
using System.Runtime.Serialization;

namespace Pudge.Units.WADUnit
{
    [Serializable]
    public enum WADCommandType
    {
        Move,
        RotateClockwise,
        RotateCounterClockwise,
        Wait
    }

    [Serializable]
    [DataContract]
    public class GameMovement
    {
        [DataMember]
        public double WaitTime{ get; set; }

        [DataMember]
        public double Range{ get; set; }

        [DataMember]
        public double Angle{ get; set; }
    }

    public interface IGameCommand
    {
        GameMovement GameMovement{ get; set; }
    }
}